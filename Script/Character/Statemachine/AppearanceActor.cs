using UnityEngine;

public class AppearanceActor : ActorBase
{
    private readonly AppearanceActorConfig mConfig;
    private readonly BasicTimer mAppearanceTimer;


    public AppearanceActor(ActorStateMachine onwerStateMachine, AppearanceActorConfig config)
        : base(onwerStateMachine, config.BaseConfig, config.name)
    {
        mConfig = config;
        mAppearanceTimer = GlobalTimer.Instance.AddBasicTimer(onwerStateMachine.OwnerCharacterBase.gameObject, "AppearanceTimer", eTimerUpdateMode.FixedUpdate);
        mAppearanceTimer.ChangeTimerEndCallback(StopActing);
    }

    public override void InovkeActing(object parameter1, object parameter2)
    {
        checkParamterValidate(null, null, parameter1, parameter2);
        OWNER.Translator.SwitchComponent(eTranslatorType.Rigidbody);
        mRB.EnrollSetVelocity(Vector3.zero, eActorType.Appearance);
        mAppearanceTimer.ChangeDuration(mConfig.GetDuration()).StartTimer();
        Anim.PlayAppearanceAnim(mConfig.GetCaculatedAnimSpeed());
    }

    public override void StopActing()
    {
        OWNER.CheckAndClearActor(eActorType.Appearance);
        mRB.DisEnrollSetVelocity(eActorType.Appearance);
    }

    public override void DestoryActor()
    {
    }
}
