using UnityEngine;

public class MoabAISkillActor : ActorBase
{
    public eMoabAttackState CurrentAttackState
    {
        get; private set;
    }

    protected readonly MoabAISkillConfig mConfig;
    protected readonly SkillActor mMeleeAttackActor;
    protected readonly SkillActor mRushSkillActor; 
    protected readonly SkillActor mRangeAttackActor;
    protected readonly SkillActor mSideStepActor;
    protected readonly SkillActor mRampageActor;


    private float mBaseDamage;
    private eMoabAttackState[] mComboAttackList;
    private int mComboAttackIndex = -1;
    private int mRampageAttackIndex = -1;
    private bool isRushable
    {
        get
        {
            return PlayerManager.Instance.IsPlayerInRange(mTransform.position, mConfig.RushableRange)
                && mRushSkillActor.CanAttack;
        }
    }

    private bool isMeleeAttackable
    {
        get
        {
            return PlayerManager.Instance.IsPlayerInRange(mTransform.position, mConfig.MeleeAttackRange)
                && mMeleeAttackActor.CanAttack;
        }
    }

    private bool isRangeAttackable
    {
        get
        {
            bool isInMinRange = PlayerManager.Instance.IsPlayerInRange(mTransform.position, mConfig.ProjectileMinAttackRange);
            bool isInMaxRange = PlayerManager.Instance.IsPlayerInRange(mTransform.position, mConfig.ProjectileAttackRange);

            return mRangeAttackActor.CanAttack && !isInMinRange && isInMaxRange;
        }
    }


    public MoabAISkillActor(ActorStateMachine ownerStateMachine, MoabAISkillConfig config, float baseDamage) :
        base(ownerStateMachine, config.BaseConfig, config.name)
    {
        mConfig = config;

        mMeleeAttackActor = new SkillActor(ownerStateMachine, mConfig.MeleeAttack, null,onAttackEnd);
        OWNER.AddActor(mMeleeAttackActor);
        mRushSkillActor = new SkillActor(ownerStateMachine, mConfig.RushAttack, null, onAttackEnd);
        OWNER.AddActor(mRushSkillActor);
        mRangeAttackActor = new SkillActor(ownerStateMachine, mConfig.RangeAttack, null, onAttackEnd);
        OWNER.AddActor(mRangeAttackActor);
        mSideStepActor = new SkillActor(ownerStateMachine, mConfig.SideStep, null, onAttackEnd);
        OWNER.AddActor(mSideStepActor);
        mRampageActor = new ChannelingSkillActor(ownerStateMachine, mConfig.RampageAttack[0], null, onAttackEnd);
        OWNER.AddActor(mRampageActor);
        CurrentAttackState = eMoabAttackState.NoneAttack;
        mBaseDamage = baseDamage;
    }

    public override void StopActing()
    {

    }

    public override void DestoryActor()
    {

    }

    public override void UpdateActing(float deltaTime)
    {
        SkillActor currentSkillActor = null;
        switch (CurrentAttackState)
        {
            case eMoabAttackState.NoneAttack:
                eMoabAttackState newAttack = tryAttack();
                CurrentAttackState = newAttack;
                return;
            case eMoabAttackState.MeleeAttack:
                currentSkillActor = mMeleeAttackActor;
                break;
            case eMoabAttackState.RushAttack:
                currentSkillActor = mRushSkillActor;
                break;
            case eMoabAttackState.RangeAttack:
                currentSkillActor = mRangeAttackActor;
                break;
            case eMoabAttackState.SideStep:
                currentSkillActor = mSideStepActor;
                break;
            case eMoabAttackState.RampageAttack:
                currentSkillActor = mRampageActor;
                break;
            default:
                Debug.Assert(false);
                break;
        }

        if(mComboAttackIndex == -1 && !currentSkillActor.IsOnAttack)
        {
            CurrentAttackState = eMoabAttackState.NoneAttack;
        }
        else if(mComboAttackIndex != -1 && (currentSkillActor.CurrentProgressState == eSkillProgressState.Cancelable || currentSkillActor.CurrentProgressState == eSkillProgressState.None))
        {
            CurrentAttackState = eMoabAttackState.NoneAttack;
        }
    }

    public bool TryEvade()
    {
        if(!mSideStepActor.CanAttack)
        {
            return false;
        }

        if(CurrentAttackState != eMoabAttackState.NoneAttack)
        {
            return false;
        }

        CurrentAttackState = eMoabAttackState.SideStep;
        OWNER.TrySwitchActor(mSideStepActor.ActorType, mBaseDamage, null);

        mComboAttackIndex = 0;
        mComboAttackList = mConfig.PickComboAttack(getCurrentTear(), eMoabAttackState.SideStep);
        if (mComboAttackList.Length == 0)
        {
            mComboAttackIndex = -1;
        }
        OWNER.OwnerCharacterBase.CharacterStatus.IsPowerOverwalming.EnrollValue(CharacterStatus.ePowerOverwalmingSetter.SetBySkill, true);
        return true;
    }

    private eMoabAttackState tryAttack()
    {
        if (OWNER.CurrentActorType == eActorType.Damaged)
        {
            return eMoabAttackState.NoneAttack;
        }
        
        eMoabAttackState returnState = eMoabAttackState.NoneAttack;

        if(tryRampageAttack())
        {
            mRampageActor.TryChangeConfigOrStackedIn(mConfig.RampageAttack[mRampageAttackIndex].GetRuntimeSkillConfig());
            OWNER.TrySwitchActor(mRampageActor.ActorType, mBaseDamage, null);
            mComboAttackIndex = -1;
            return eMoabAttackState.RampageAttack;
        }

        if (mComboAttackIndex == -1)
        {
            if (isRushable)
            {
                OWNER.TrySwitchActor(mRushSkillActor.ActorType, mBaseDamage, null);
                returnState = eMoabAttackState.RushAttack;
            }

            if (isRangeAttackable)
            {
                OWNER.TrySwitchActor(mRangeAttackActor.ActorType, mBaseDamage, null);
                returnState = eMoabAttackState.RangeAttack;
            }

            if (isMeleeAttackable)
            {
                OWNER.TrySwitchActor(mMeleeAttackActor.ActorType, mBaseDamage, null);
                returnState = eMoabAttackState.MeleeAttack;
            }

            if (returnState != eMoabAttackState.NoneAttack)
            {
                mComboAttackIndex = 0;
                mComboAttackList = mConfig.PickComboAttack(getCurrentTear(), returnState);
                if (mComboAttackList.Length == 0)
                {
                    mComboAttackIndex = -1;
                }
            }

            return returnState;
        }

        switch (mComboAttackList[mComboAttackIndex])
        {
            case eMoabAttackState.MeleeAttack:
                OWNER.TrySwitchActor(mMeleeAttackActor.ActorType, mBaseDamage, null);
                break;
            case eMoabAttackState.RushAttack:
                OWNER.TrySwitchActor(mRushSkillActor.ActorType, mBaseDamage, null);
                break;
            case eMoabAttackState.RangeAttack:
                OWNER.TrySwitchActor(mRangeAttackActor.ActorType, mBaseDamage, null);
                break;
            case eMoabAttackState.SideStep:
                OWNER.TrySwitchActor(mSideStepActor.ActorType, mBaseDamage, null);
                break;
        }
        returnState = mComboAttackList[mComboAttackIndex];
        mComboAttackIndex++;
        if (mComboAttackIndex == mComboAttackList.Length)
        {
            mComboAttackIndex = -1;
        }

        return returnState;
    }

    private bool tryRampageAttack()
    {
        if (OWNER.CurrentActorType == eActorType.Damaged)
        {
            return false;
        }
        float normalizedHP = OWNER.OwnerCharacterBase.CharacterStatus.NormalizedHP;
        for (int i = mConfig.RampageAttackThreshold.Length - 1; i > mRampageAttackIndex; i--)
        {
            if (normalizedHP <= mConfig.RampageAttackThreshold[i])
            {
                mRampageAttackIndex = i;
                return true;
            }
        }
        return false;
    }


    private void onAttackEnd()
    {
        OWNER.TrySwitchActor(eActorType.AIMovement);
    }

    private int getCurrentTear()
    {
        float normalizedHP = OWNER.OwnerCharacterBase.CharacterStatus.NormalizedHP;

        for (int i = 0; i < mConfig.ComboTearThreshold.Length; i++)
        {
            if(normalizedHP >= mConfig.ComboTearThreshold[i])
            {
                return i;
            }
        }

        return mConfig.ComboTearThreshold.Length;
    }
}

public enum eMoabAttackState
{
    NoneAttack,
    MeleeAttack,
    RushAttack,
    RangeAttack,
    SideStep,
    RampageAttack,
}