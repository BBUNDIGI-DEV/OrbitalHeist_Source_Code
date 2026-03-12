using UnityEngine;

public class ComboSkillActor : SkillActor
{
    public bool IsWaitComboAttackInput
    {
        get; private set;
    }

    public bool CanComboAttack
    {
        get
        {
            if(IsOnAttack)
            {
                return mCurrentComboCount != 0 && mCurrentComboCount < ComboSkillConfigs[0].EnabledComboCount;
            }
            else
            {
                return mCurrentComboCount != 0 && mCurrentComboCount < ComboSkillConfigs[0].EnabledComboCount && mCombableTimer.IsActivate;
            }
        }
    }

    public SkillConfig[] ComboSkillConfigs
    {
        get; private set;
    }


    private readonly BasicTimer mCombableTimer;
    private int mEnabledComboCount
    {
        get
        {
            return ComboSkillConfigs[0].EnabledComboCount;
        }
    }
    private int mCurrentComboCount = 0;

    public ComboSkillActor(ActorStateMachine ownerSM, SkillConfig firstSkillConfig, AttackBoxElement.OnAttackBoxHit onAttackBoxHit ,System.Action onAttackEnd) 
        : base(ownerSM, firstSkillConfig, onAttackBoxHit, onAttackEnd)
    {
        Debug.Assert(SkillConfig.IsComboAttack,
            $"SkillConfig for ComboSkillAactor must be set as comboattack");
        mCombableTimer =  GlobalTimer.Instance.AddBasicTimer(ownerSM.OwnerCharacterBase.gameObject, getTimerKeyString(SkillConfig, "CombableTime"), eTimerUpdateMode.FixedUpdate);
        SkillConfig[] refComboSkillConfigs = RuntimeDataLoader.ComboSkillConfigs[firstSkillConfig.ComboSkillName];
        ComboSkillConfigs = new SkillConfig[refComboSkillConfigs.Length];
        for (int i = 0; i < ComboSkillConfigs.Length; i++)
        {
            if(i == 0)
            {
                ComboSkillConfigs[0] = SkillConfig;
            }
            else
            {
                SkillConfig comboConfig = refComboSkillConfigs[i];
                comboConfig = refComboSkillConfigs[i].GetRuntimeSkillConfig();
                ComboSkillConfigs[i] = comboConfig;
            }
        }
        mCombableTimer.ChangeTimerEndCallback(resetComboInfo);
    }

    public override void InovkeActing(object parameter1, object parameter2)
    {
        checkParamterValidate(typeof(float), null, parameter1, parameter2);
        Debug.Assert(mCurrentComboCount < mEnabledComboCount,
            $"Combo Count Out Of Range[{NameID}]");
        changeConfig(ComboSkillConfigs[mCurrentComboCount]);
        if(mCombableTimer.IsActivate)
        {
            mCombableTimer.StopTimer(false);
        }
        base.InovkeActing(parameter1, parameter2);
        if (mCombableTimer.IsActivate)
        {
            mCombableTimer.StopTimer(false);
        }

        if(SkillConfig.SetWaitComboInputManually)
        {
            IsWaitComboAttackInput = false;
        }
        else
        {
            IsWaitComboAttackInput = true;
        }

        mCurrentComboCount++;
    }

    public void IncreaseTotalComboCount()
    {
        ComboSkillConfigs[0].EnabledComboCount++;
    }

    public void DecreaseTotalComboCount()
    {
        ComboSkillConfigs[0].EnabledComboCount--;
    }

    public void SwapComboConfig(SkillConfig skillConfig, int comboIndex)
    {
        SkillConfig runtimeConfig = skillConfig;
        ComboSkillConfigs[comboIndex] = runtimeConfig;
    }

    protected override void onAttackEnd()
    {
        base.onAttackEnd();
        IsWaitComboAttackInput = false;
        if (mCurrentComboCount < mEnabledComboCount)
        {
            mCombableTimer.ChangeDuration(SkillConfig.CombableTime).StartTimer();
        }
        else if(mCurrentComboCount == mEnabledComboCount)
        {
            resetComboInfo();
        }
    }

    protected override string getTimerKeyString(SkillConfig config, string baseName)
    {
        return baseName + config.ComboSkillName;
    }

    protected override void invokeSkillEvent(eSkillEventMarkerType markerType, string clipName)
    {
        base.invokeSkillEvent(markerType, clipName);

        switch (markerType)
        {
            case eSkillEventMarkerType.SetComboInputWait:
                IsWaitComboAttackInput = true;
                break;
            default:
                break;
        }
    }

    private void resetComboInfo()
    {
        mCurrentComboCount = 0;
    }
}
