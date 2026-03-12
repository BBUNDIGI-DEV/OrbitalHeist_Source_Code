using UnityEngine;

public class DashActor : ActorBase
{
    public bool IsExtraDashPressed;

    public bool IsOnDash
    {
        get
        {
            return mDashTimer.IsTaggedTimerActivated(DASH_TRANSITION_KEY);
        }
    }

    public bool IsInHolding
    {
        get
        {
            return mDashTimer.IsTaggedTimerActivated(DASH_HOLDING_KEY);
        }
    }

    public bool CanExtraDashable
    {
        get
        {
            return mCurrentDashCount < mDashableCount;
        }
    }

    public bool CanDashAttack
    {
        get
        {
            bool result = (IsOnDash || IsInHolding || mDashAttackableTimer.IsActivate) && !mHasDashAttack;
            if (result)
            {
                mHasDashAttack = true;
            }
            return result;
        }
    }

    public bool CanDash
    {
        get
        {
            return CanExtraDashable && !mDashTimer.IsTaggedTimerActivated(DASH_COOLTIME_KEY);
        }
    }

    public bool mIsDashCooltime
    {
        get
        {
            return mDashTimer.IsTaggedTimerActivated(DASH_COOLTIME_KEY) || mDashTimer.IsTaggedTimerActivated(EXTRA_DASHABLE_TIME_KEY);
        }
    }

    public int mCurrentDashCount
    {
        get; private set;
    }

    private const string DASH_HOLDING_KEY = "DashHolding";
    private const string DASH_TRANSITION_KEY = "DashTransition";
    private const string EXTRA_DASHABLE_TIME_KEY = "ExtraDashableTime";
    private const string DASH_COOLTIME_KEY = "DashCoolTime";

    private readonly DashConfig mRuntimeConfig;
    private readonly GameObjectPool mEffectPool;
    private readonly System.Action mOnDashEnd;
    private readonly ChainedTimer mDashTimer;
    private readonly BasicTimer mDashAttackableTimer;

    private int mDashableCount
    {
        get
        {
            return mRuntimeConfig.DefaultMaxDashNumber + PlayerManager.Instance.GlobalPlayerStatus.GlobalAdditionalDashableCount;
        }
    }
    private bool mHasDashAttack = false;

    public DashActor(ActorStateMachine owner, DashConfig config, System.Action onDashEndCallback)
        : base(owner, config.BaseConfig, config.name)
    {
        mRuntimeConfig = Object.Instantiate(config);
        mOnDashEnd = onDashEndCallback;

        mDashTimer = GlobalTimer.Instance.AddChainedTimer(owner.OwnerCharacterBase.gameObject, "Dash", eTimerUpdateMode.FixedUpdate);
        mDashTimer
            .AddCallback(DASH_HOLDING_KEY, config.DashHoldingTime, tryInvokeBufferedDash)
            .AddCallback(DASH_TRANSITION_KEY, config.DashDuration - config.DashHoldingTime, onDashEnd)
            .AddCallback(EXTRA_DASHABLE_TIME_KEY, config.ExtraDashableTime, null)
            .AddCallback(DASH_COOLTIME_KEY, config.DashCooltime - config.ExtraDashableTime, onDashCoolDownEnd);

        mDashAttackableTimer = GlobalTimer.Instance.AddBasicTimer(owner.OwnerCharacterBase.gameObject, "Dashattackable", eTimerUpdateMode.FixedUpdate);
        mEffectPool = GameObjectPool.TryGetGameobjectPool(owner.OwnerCharacterBase.gameObject.transform, "EffectPool");
    }

    public override void InovkeActing(object parameter1, object parameter2)
    {
        checkParamterValidate(null, null, parameter1, parameter2);
        SFXManager.PlayPlayerPublicDashSound();
        setDashedLayer();

        Vector3 moveDir = InputManager.Instance.MoveDir;
        Vector3 dashDir;

        if (moveDir == Vector3.zero)
        {
            dashDir = mTransform.rotation * Vector3.forward;
        }
        else
        {
            dashDir = moveDir;
        }


        mRB.EnrollLookRotationAndForceRotating(dashDir, ActorType);


        float dashDistance = mCurrentDashCount < 1 ? mRuntimeConfig.InitialDashDistance : mRuntimeConfig.SecondDashDistance;
        if(OWNER.IsPlayerActor)
        {
            dashDistance += PlayerManager.Instance.GlobalPlayerStatus.GlobalAdditionalDashDistance;
        }
        float speed = dashDistance / mRuntimeConfig.DashDuration;
        mRB.EnrollSetVelocity(dashDir * speed, ActorType);
        Anim.TriggerDashAnim(mRuntimeConfig.CaculateAnimSpeed());
        Vector3 destPos = mRB.position + dashDir * dashDistance;
        mDashTimer.StartTimer();
        CameraManager.Instance.Actor.ProcessCameraActing(mRuntimeConfig.CameraActingData);

        if (mRuntimeConfig.EffectPrefab != null)
        {
            ParticleBinder dashFXPrefab = mRuntimeConfig.EffectPrefab;
            if(OWNER.IsPlayerActor)
            {
                eCharacterName name = ((PlayerCharacterController)OWNER.OwnerCharacterBase).CharName;
                switch (name)
                {
                    case eCharacterName.Glanda:
                        dashFXPrefab = mRuntimeConfig.GlandaDashFXPrefab;
                        break;
                    case eCharacterName.Hypo:
                        dashFXPrefab = mRuntimeConfig.HypoDashFXPrefab;
                        break;
                    case eCharacterName.Shiv:
                        dashFXPrefab = mRuntimeConfig.ShivDashFXPrefab;
                        break;
                    default:
                        break;
                }
            }
            ParticleBinder dashFX = mEffectPool.GetGameobject(dashFXPrefab);
            dashFX.SetFXTransformType(eFXTransformType.PositionOnlyWorld, Quaternion.LookRotation(dashDir), OWNER.Translator.Trans);
            dashFX.gameObject.SetActive(true);
        }

        mHasDashAttack = false;
        mCurrentDashCount++;
        IsExtraDashPressed = false;
    }

    public override void StopActing()
    {
        onDashEnd();
    }

    public override void DestoryActor()
    {

    }

    private void onDashEnd()
    {
        if (mDashTimer.IsTaggedTimerActivated(DASH_TRANSITION_KEY))
        {
            mDashTimer.SkipTaggedTimer(DASH_TRANSITION_KEY, false);
        }

        mDashAttackableTimer.ChangeDuration(mRuntimeConfig.DashAttackableTime).StartTimer();
        mRB.DisEnrollSetVelocity(ActorType);
        mRB.DisEnrollLookRotatoin(ActorType);
        clearDashedLayer();
        mOnDashEnd?.Invoke();
        OWNER.CheckAndClearActor(ActorType);
    }

    private void onDashCoolDownEnd()
    {
        mCurrentDashCount = 0;
    }

    private void setDashedLayer()
    {
        Collider collider = mRB.GetComponent<Collider>();
        LayerMask excludeLayers = mRB.excludeLayers;
        excludeLayers.Add("PassableObstacle");
        excludeLayers.Add("DynamicObject");
        collider.excludeLayers = excludeLayers;

        OWNER.OwnerCharacterBase.SetHitBoxEnable(false);
    }

    private void clearDashedLayer()
    {
        Collider collider = mRB.GetComponent<Collider>();
        LayerMask excludeLayers = collider.excludeLayers;
        excludeLayers.Remove("PassableObstacle");
        excludeLayers.Remove("DynamicObject");
        collider.excludeLayers = excludeLayers;

        OWNER.OwnerCharacterBase.SetHitBoxEnable(true);
    }

    private void tryInvokeBufferedDash()
    {
        if(IsExtraDashPressed)
        {
            OWNER.TrySwitchActor(eActorType.Dash);
            IsExtraDashPressed = false;
        }
    }
}