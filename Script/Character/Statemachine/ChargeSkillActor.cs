using UnityEngine;

public class ChargeSkillActor : SkillActor
{
    public ObservedData<float> NormalizedChargingDuration;
    public bool IsOnCharging
    {
        get; private set;
    }
    private readonly SkillConfig mChargingSkillConfig;
    private readonly SkillConfig[] mUpgradedChargeSkillConfig;
    private float mChargingDuration;
    private bool mIsChargeShotInovked;
    public ChargeSkillActor(ActorStateMachine onwerStateMachine, SkillConfig config, AttackBoxElement.OnAttackBoxHit onAttackBoxHit, System.Action onAttackEnd) 
        : base(onwerStateMachine, config, onAttackBoxHit, onAttackEnd)
    {
        Debug.Assert(SkillConfig.IsChargingAttack,
            $"SkillConfig for ChargeSkillActor must be set as ChargeAttack");
        mChargingSkillConfig = config;
        mUpgradedChargeSkillConfig = new SkillConfig[SkillConfig.ChargeConfig.Length];
        for (int i = 0; i < config.ChargeConfig.Length; i++)
        {
            mUpgradedChargeSkillConfig[i] = config.ChargeConfig[i].InstantiateSkillConfigVarient();
        }

        SetEnabledUpdating(false);
    }

    public override void InovkeActing(object parameter1, object parameter2)
    {
        checkParamterValidate(typeof(float), null, parameter1, parameter2);

        Debug.Assert(!IsOnCharging, "You Cannot trying to charge when you are already in charging mode");
        IsOnCharging = true;
        base.InovkeActing(parameter1, null);
        mChargingDuration = 0.0f;
        SetEnabledUpdating(true);
        OWNER.Translator.RB.EnrollSetVelocity(Vector3.zero, BaseConfig.ActorType);
    }

    public override void UpdateActing(float deltaTime)
    {
        mChargingDuration += deltaTime;
        NormalizedChargingDuration.Value = Mathf.Clamp01(mChargingDuration / mChargingSkillConfig.ChargeStep[mChargingSkillConfig.ChargeStep.Length - 1]);


        Vector3 attackAIM = InputManager.Instance.GetAttackAim(OWNER.Translator.Trans);
        //Debug.DrawRay(OWNER.Translator.Trans.position, attackAIM, Color.red, 10.0f);
        OWNER.Translator.RB.EnrollLookRotation(InputManager.Instance.GetAttackAim(OWNER.Translator.Trans), BaseConfig.ActorType);
    }

    protected override void onAttackEnd()
    {
        IsOnCharging = false;
        if(!mIsChargeShotInovked)
        {
            mChargingDuration = 0.0f;
            SetEnabledUpdating(false);
            NormalizedChargingDuration.Value = 0.0f;
            Anim.TriggerChargingPaused(mChargingSkillConfig);
            OWNER.Translator.RB.DisEnrollLookRotatoin(eActorType.NormalChargeAttack);
        }
        else
        {
            mIsChargeShotInovked = false;
        }
        base.onAttackEnd();
        TryChangeConfigOrStackedIn(mChargingSkillConfig);
    }


    public void ChargingShot()
    {
        Anim.TriggerChargingEnd(mChargingSkillConfig);
        changeSkillConfigByCharging();
        mChargingDuration = 0.0f;
        NormalizedChargingDuration.Value = 0.0f;
        SetEnabledUpdating(false);
        IsOnCharging = false;
        mIsChargeShotInovked = true;
    }

    private void changeSkillConfigByCharging()
    {
        int chargeIndex = -1;
        for (int i = 0; i < SkillConfig.ChargeStep.Length; i++)
        {
            if (mChargingDuration <= SkillConfig.ChargeStep[i])
            {
                break;
            }
            chargeIndex++;

        }

        if (chargeIndex == -1)
        {
            return;
        }
        else
        {
            changeConfig(mUpgradedChargeSkillConfig[chargeIndex]);
        }
    }

}
