using System;
using UnityEngine;

public class DeadActor : ActorBase
{
    private readonly DeadActorConfig mConfig;
    private readonly BasicTimer mDeadActorTimer;
    private Action mOnDeadEnd;

    public DeadActor(ActorStateMachine onwerStateMachine, DeadActorConfig config)
        : base(onwerStateMachine, config.BaseConfig, config.name)
    {
        mConfig = config;
        mDeadActorTimer = GlobalTimer.Instance.AddBasicTimer(onwerStateMachine.OwnerCharacterBase.gameObject, "DeadActorTimer", eTimerUpdateMode.FixedUpdate);
    }

    public override void InovkeActing(object parameter1, object parameter2)
    {
        OWNER.OwnerCharacterBase.Hitbox.Collider.enabled = false;
        OWNER.OwnerCharacterBase.CharacterStatus.IsDead.Value = true;

        //Special case skip dead acting
        if (parameter1 == null)
        {
            mOnDeadEnd = (Action)parameter2;
            mOnDeadEnd?.Invoke();
            return;
        }

        mConfig.DeadSound.TryPlay();
        checkParamterValidate(typeof(HitEffectData), typeof(Action), parameter1, parameter2);

        HitEffectData hitEffectData = (HitEffectData)parameter1;
        Vector3 rotToward = hitEffectData.AttackerPosition - mRB.position;
        rotToward.y = 0.0f;
        mRB.EnrollLookRotationAndForceRotating(rotToward, eActorType.Damaged);
        mRB.EnrollSetVelocity(Vector3.zero, eActorType.Dead);

        mOnDeadEnd = (Action)parameter2;
        if (mConfig.GetDuration() != 0.0f)
        {
            mDeadActorTimer.ChangeTimerEndCallback(onDeadAnimEnd);
            mDeadActorTimer.ChangeDuration(mConfig.GetDuration()).StartTimer();
            Anim.PlayDeadAnim(mConfig.GetCaculatedAnimSpeed());
        }
        else
        {
            mOnDeadEnd?.Invoke();
        }

        if(!OWNER.IsPlayerActor)
        {
            OWNER.Translator.SwitchComponent(eTranslatorType.Rigidbody);
        }
    }

    public override void StopActing()
    {
        mRB.DisEnrollLookRotatoin(eActorType.Damaged);
        mRB.DisEnrollSetVelocity(eActorType.Dead);
        OWNER.CheckAndClearActor(eActorType.Dead);
    }

    public override void DestoryActor()
    {
    }

    private void onDeadAnimEnd()
    {
        if(mConfig.SkipDissolve)
        {
            onDeadEnd();
            return;
        }
        Anim.PlayDeadDissolve();
        mDeadActorTimer.ChangeTimerEndCallback(onDeadEnd).ChangeDuration(1.0f).StartTimer();
    }

    private void onDeadEnd()
    {
        mOnDeadEnd?.Invoke();
    }
}
