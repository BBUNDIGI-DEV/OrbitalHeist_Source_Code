using UnityEngine;

public class AISkillActor : ActorBase
{
    protected readonly AISkillConfig mConfig;
    protected readonly SkillActor mMeleeSkillActor;
    protected readonly SkillActor mRangeSkillActor;

    protected float mBaseDamage;
    private bool mIsOnAttack;

    public AISkillActor(ActorStateMachine ownerStateMachine, AISkillConfig config, float baseDamage) :
        base(ownerStateMachine, config.BaseConfig, config.name)
    {
        mConfig = config;
        if (mConfig.CombatType.HasFlag(eCombatType.MeleeAttackInRange))
        {
            mMeleeSkillActor = new SkillActor(ownerStateMachine, mConfig.MeleeAttackConfig, null, onAttackEnd);
            OWNER.AddActor(mMeleeSkillActor);
        }

        if (mConfig.CombatType.HasFlag(eCombatType.ShootProjectileTowardPlayer))
        {
            mRangeSkillActor = new SkillActor(ownerStateMachine, mConfig.RangeAttackConfig, null, onAttackEnd);
            OWNER.AddActor(mRangeSkillActor);
        }
        SetEnabledUpdating(false); //Update will be set as true when movement ai actor search player
        mBaseDamage = baseDamage * MonsterBase.sGlobalData.GlobalDamageMultiplier;
    }

    public override void InovkeActing(object parameter1, object parameter2)
    {
        Debug.LogError("AI SkillActor not for invoke acting just only use updateActing");
    }

    public override void StopActing()
    {

    }

    public override void UpdateActing(float deltaTime)
    {
        if(mConfig.CombatType.HasFlag(eCombatType.MeleeAttackInRange))
        {
            tryMeleeAttack();
        }

        if (mConfig.CombatType.HasFlag(eCombatType.ShootProjectileTowardPlayer))
        {
            tryProjectileAttack();
        }
    }

    public override void DestoryActor()
    {

    }

    public void UpdateBaseDamage(float baseDamage)
    {
        mBaseDamage = baseDamage;
    }

    private void tryMeleeAttack()
    {
        if (PlayerManager.Instance.IsPlayerInRange(mPosition, mConfig.MeleeAttackRange) == false)
        {
            return;
        }

        if (!mMeleeSkillActor.CanAttack || mIsOnAttack)
        {
            return;
        }
        OWNER.TrySwitchActor(mMeleeSkillActor.ActorType, mBaseDamage, null);
        mIsOnAttack = true;
    }

    private void tryProjectileAttack()
    {
        if (PlayerManager.Instance.IsPlayerInRange(mPosition, mConfig.ProjectileMinRange) == true)
        {
            return;
        }

        if (PlayerManager.Instance.IsPlayerInRange(mPosition, mConfig.ProjectileMaxRange) == false)
        {
            return;
        }

        if (!mRangeSkillActor.CanAttack || mIsOnAttack)
        {
            return;
        }

        OWNER.TrySwitchActor(mRangeSkillActor.ActorType, mBaseDamage, null);
        mIsOnAttack = true;
    }

    private void onAttackEnd()
    {
        mIsOnAttack = false;
    }
}


