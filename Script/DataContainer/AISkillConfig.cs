using UnityEngine;
using Sirenix.OdinInspector;
[CreateAssetMenu(menuName = "DataContainer/AISKillConfig")]
public class AISkillConfig : ActorConfigDataContainerBase
{
    public eCombatType CombatType;
    [ShowIf("@CombatType.HasFlag(eCombatType.MeleeAttackInRange)")] public SkillConfig MeleeAttackConfig;
    [ShowIf("@CombatType.HasFlag(eCombatType.MeleeAttackInRange)")] public float MeleeAttackRange;

    [ShowIf("@CombatType.HasFlag(eCombatType.ShootProjectileTowardPlayer)")] public SkillConfig RangeAttackConfig;
    [ShowIf("@CombatType.HasFlag(eCombatType.ShootProjectileTowardPlayer)")] public float ProjectileMinRange;
    [ShowIf("@CombatType.HasFlag(eCombatType.ShootProjectileTowardPlayer)")] public float ProjectileMaxRange;


    protected override void initializeConfig()
    {
        BaseConfig.ActorType = eActorType.AIAttack;
        BaseConfig.IsUpdatedActor = true;
    }
}


[System.Flags]
public enum eCombatType
{
    None = -1,
    MeleeAttackInRange = 1 << 0,
    ShootProjectileTowardPlayer = 1 << 1,
}

