using UnityEngine;

public class DamagedActor : ActorBase
{
    public bool IsInDamaged
    {
        get; private set;
    }

    private readonly DamagedActorConfig mConfig;
    private readonly GameObjectPool mEffectPool;
    private readonly System.Action mOnDamagedEnd;
    private readonly ChainedTimer mDamagedTimer;
    private HitEffectData mHitEffectData;

    public DamagedActor(ActorStateMachine onwer, DamagedActorConfig config, System.Action onDamagedEnd) 
        : base(onwer, config.BaseConfig, config.name)
    {
        mConfig = config;
        mOnDamagedEnd = onDamagedEnd;
        mDamagedTimer = GlobalTimer.Instance.AddChainedTimer(onwer.OwnerCharacterBase.gameObject, "DamagedTimer" + mConfig.name, eTimerUpdateMode.FixedUpdate);
        mDamagedTimer
            .AddCallback("NockBack", 0.0f, onKnockbackEnd)
            .AddCallback("DamagedEnd", 0.0f, onDamagedActingEnd);

        mEffectPool = GameObjectPool.TryGetGameobjectPool(onwer.OwnerCharacterBase.transform, "EffectPool");
    }

    public override void InovkeActing(object parameter1, object parameter2)
    {
        checkParamterValidate(typeof(HitEffectData), null , parameter1, parameter2);

        mHitEffectData = (HitEffectData)parameter1;
        if (IsInDamaged)
        {
            if (mDamagedTimer.IsActivate)
            {
                mDamagedTimer.StopTimer(true);
            }
        }

        Vector3 rotToward = Vector3.zero;

        if (mHitEffectData.KnockbackData.KnockbackType == eKnockbackType.CircularToHitPoint)
        {
            rotToward = mHitEffectData.ColliderPostiion - mTransform.position;
            if (rotToward == Vector3.zero)
            {
                rotToward = mHitEffectData.AttackerPosition - mTransform.position;
            }
        }
        else
        {
            rotToward = mHitEffectData.AttackerPosition - mTransform.position;
        }
        rotToward.y = 0.0f;
        rotToward.Normalize();
        OWNER.Translator.SwitchComponent(eTranslatorType.Rigidbody);
        if (rotToward != Vector3.zero)
        {
            mRB.EnrollLookRotationAndForceRotating(rotToward, eActorType.Damaged);
        }

        float knockbackTime = 0.0f;
        switch (mHitEffectData.HitffectType)
        {
            case eHitEffectType.None:
                Debug.LogError("You Cannot invoke Daamged actor with non tag");
                break;
            case eHitEffectType.Knockback:
                IsInDamaged = true;
                Anim.PlayDamagedAnim(mConfig.DamagedAnimClip.GetSynchronizedSpeed(mHitEffectData.DamagedDuration
                    + mHitEffectData.KnockbackData.NockbackTime));

                Vector3 knockbackDir = processNockback(mHitEffectData.KnockbackData, mHitEffectData.AttackerPosition ,mHitEffectData.ColliderPostiion);
                knockbackTime = mHitEffectData.KnockbackData.NockbackTime;

                if(mHitEffectData.KnockbackData.NockBackFX != null)
                {
                    mEffectPool.GetGameobject(mHitEffectData.KnockbackData.NockBackFX).SetFXTransformType(eFXTransformType.Local, 
                        Quaternion.LookRotation(knockbackDir, Vector3.up),
                        OWNER.Translator.Trans);
                }

                break;
            case eHitEffectType.Stunned:
                if (mHitEffectData.DamagedDuration > 0.0f)
                {
                    Anim.PlayStunAnim();
                }
                mRB.EnrollSetVelocity(Vector3.zero, eActorType.Damaged);
                break;
            case eHitEffectType.Deaccelerate:
                Anim.PlayDamagedAnim(mConfig.DamagedAnimClip.GetSynchronizedSpeed(mHitEffectData.DamagedDuration));
                mRB.GetLayeredRigidbody().SetVelocityMultiplier(mHitEffectData.Deacceleration, eSpeedMultiplierSource.GotHit);
                break;
            case eHitEffectType.Pause:
                mRB.EnrollSetVelocity(Vector3.zero, eActorType.Damaged);
                Anim.PauseAnim();
                break;
            default:
                Debug.LogError($"Default state machine");
                break;
        }

        mDamagedTimer
                .ChangeTiming("NockBack", knockbackTime)
                .ChangeTiming("DamagedEnd", mHitEffectData.DamagedDuration)
                .StartTimer();

        if(!mConfig.DamagedSound.IsNull)
        {
            mConfig.DamagedSound.TryPlay();
        }
    }

    public override void StopActing()
    {
        if (mDamagedTimer.IsActivate)
        {
            mDamagedTimer.StopTimer(true);
        }
    }

    public override void DestoryActor()
    {

    }

    protected virtual Vector3 processNockback(KnockbackData data, Vector3 attackerPos, Vector3 colliderPos)
    {
        Vector3 nockbackDir = data.KnockbackDir;
        switch (data.KnockbackType)
        {
            case eKnockbackType.PushToAttackDir:
                break;
            case eKnockbackType.CircularToHitPoint:
                nockbackDir = (mTransform.position - colliderPos);
                nockbackDir.y = 0.0f;

                if (data.IsInverseDir)
                {
                    if(nockbackDir.sqrMagnitude < 1.0f)
                    {
                        nockbackDir = Vector3.zero;
                        break;
                    }
                }
                else
                {
                    if (nockbackDir == Vector3.zero)
                    {
                        nockbackDir = (mTransform.position - attackerPos);
                        nockbackDir.y = 0.0f;
                    }
                }

                nockbackDir.Normalize();
                break;
            default:
                Debug.LogError($"Default Switch Detected [{data.KnockbackType}]");
                break;
        }

        mRB.EnrollSetVelocity(nockbackDir * data.NockbackPower,  eActorType.Damaged);

        return nockbackDir;
    }

    private void onDamagedActingEnd()
    {
        mRB.DisEnrollSetVelocity(eActorType.Damaged);
        mRB.DisEnrollLookRotatoin(eActorType.Damaged)   ;
        OWNER.CheckAndClearActor(eActorType.Damaged);
        if (mHitEffectData.HitffectType == eHitEffectType.Stunned)
        {
            Anim.PauseStunAnim();
        }
        else if(mHitEffectData.HitffectType == eHitEffectType.Pause)
        {
            Anim.ReusemeAnim();
        }
        else
        {
            mRB.GetLayeredRigidbody().SetVelocityMultiplier(1.0f, eSpeedMultiplierSource.GotHit);
        }
        IsInDamaged = false;
        mOnDamagedEnd?.Invoke();
    }

    private void onKnockbackEnd()
    {
        // Do Nothing HitEffect type is not knockback this callback only work with knockback type 
        if (mHitEffectData.HitffectType != eHitEffectType.Knockback) 
        {
            return;
        }
        mRB.EnrollSetVelocity(Vector3.zero, eActorType.Damaged);
    }
}

