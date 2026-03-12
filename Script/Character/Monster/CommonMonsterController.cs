using UnityEngine;
public class CommonMonsterController : MonsterBase
{
    protected override void Awake()
    {
        base.Awake();
    }

    protected override void initializeStateMachine()
    {
        SM = new ActorStateMachine(this, false);
        MonsterWanderingOption wanderingOption = sfUseManualWanderingOption ? sfOverridWanderingOption : sfConfig.AIMovementConfig.WanderingOption;
        MonsterCombatMovementOption combatOption = sfConfig.AIMovementConfig.CombatMovementOption;
        AISkillActor aiAttackActor = new AISkillActor(SM, sfConfig.AISkillConfig, sfConfig.InitialBaseDamage);
        SM.AddActor(aiAttackActor);

        AIMovementActor aiMovementActor = new AIMovementActor(SM, sfConfig.AIMovementConfig, wanderingOption, combatOption, null);
        SM.AddActor(aiMovementActor);

        DamagedActor damagedActor = new DamagedActor(SM, sfConfig.DamagedConfig, onDamagedEnd);
        SM.AddActor(damagedActor);

        DeadActor deadActor = new DeadActor(SM, sfConfig.DeadConfig);
        SM.AddActor(deadActor);

        AppearanceActor appreanceActor = new AppearanceActor(SM, sfConfig.AppearanceConfig);
        SM.AddActor(appreanceActor);
    }
}