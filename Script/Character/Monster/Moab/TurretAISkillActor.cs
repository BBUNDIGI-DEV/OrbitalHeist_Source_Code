using System.Collections;
using UnityEngine;

public class TurretAISkillActor : ActorBase
{
    protected readonly TurretAISkillConfig mConfig;
    protected readonly SkillActor mPlayerAimingMissileSkill;
    protected readonly SkillActor mRandomRangeMissileSkill;
    protected readonly SkillActor mFrameLauncherSkillActor;
    private eTurretState mTurretState;
    private float mBaseDamage;
    private int mMultiAttackCount;

    public TurretAISkillActor(ActorStateMachine ownerStateMachine, TurretAISkillConfig config, float baseDamage) :
        base(ownerStateMachine, config.BaseConfig, config.name)
    {
        mConfig = config;

        mPlayerAimingMissileSkill = new SkillActor(ownerStateMachine, mConfig.PlayerAimingMissileConfig, null,onAttackEnd);
        OWNER.AddActor(mPlayerAimingMissileSkill);
        mRandomRangeMissileSkill = new SkillActor(ownerStateMachine, mConfig.RandomSpreadMissileConfig, null, onAttackEnd);
        OWNER.AddActor(mRandomRangeMissileSkill);
        mFrameLauncherSkillActor = new ChannelingSkillActor(ownerStateMachine, mConfig.FrameLauncherConfig, null, onAttackEnd);
        OWNER.AddActor(mFrameLauncherSkillActor);

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
        switch (mTurretState)
        {
            case eTurretState.None:
                if (mFrameLauncherSkillActor.CanAttack)
                {
                    OWNER.TrySwitchActor(mFrameLauncherSkillActor.ActorType, mBaseDamage, null);
                    mTurretState = eTurretState.FrameLauncher;
                    mMultiAttackCount = 0;
                    return;
                }
                
                if (mPlayerAimingMissileSkill.CanAttack && mRandomRangeMissileSkill.CanAttack)
                {
                    int ranodmInt = Random.Range(0, 2);

                    if(ranodmInt == 0)
                    {
                        OWNER.TrySwitchActor(mPlayerAimingMissileSkill.ActorType, mBaseDamage, null);
                        mMultiAttackCount = Random.Range(mConfig.PlayerAimingAttackMin, mConfig.PlayerAimingAttackMax) - 1;
                        mTurretState = eTurretState.PlayerTrackingMisile;
                    }

                    if(ranodmInt == 1)
                    {
                        OWNER.TrySwitchActor(mRandomRangeMissileSkill.ActorType, mBaseDamage, null);
                        mTurretState = eTurretState.RandomRangeMisile;
                        mMultiAttackCount = Random.Range(mConfig.RandomSpreadMissileMin, mConfig.RandomSpreadMissileMax);
                        return;
                    }
                }
                break;
            case eTurretState.PlayerTrackingMisile:
                break;
            case eTurretState.RandomRangeMisile:
                break;
            case eTurretState.FrameLauncher:
                break;
            default:
                break;
        }
    }

    
    private void onAttackEnd()
    {
        if(mMultiAttackCount != 0)
        {
            switch (mTurretState)
            {
                case eTurretState.PlayerTrackingMisile:
                    OWNER.TrySwitchActor(mPlayerAimingMissileSkill.ActorType, mBaseDamage, null);
                    break;
                case eTurretState.RandomRangeMisile:
                    OWNER.TrySwitchActor(mRandomRangeMissileSkill.ActorType, mBaseDamage, null);
                    break;
                case eTurretState.FrameLauncher:
                    OWNER.TrySwitchActor(mFrameLauncherSkillActor.ActorType, mBaseDamage, null);
                    break;
                default:
                    Debug.LogError(mTurretState);
                    break;
            }
            mMultiAttackCount--;
            return;
        }
        OWNER.TrySwitchActor(eActorType.AIMovement);
        mTurretState = eTurretState.None;
    }

    public enum eTurretState
    {
        None,
        PlayerTrackingMisile,
        RandomRangeMisile,
        FrameLauncher,
    }
}
