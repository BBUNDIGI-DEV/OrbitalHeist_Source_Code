
public class BossMoabController : MonsterBase
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
        AIMovementActor aiMovementActor = new AIMovementActor(SM, sfConfig.AIMovementConfig, wanderingOption, combatOption, null);
        SM.AddActor(aiMovementActor);

        DamagedActor damagedActor = new DamagedActor(SM, sfConfig.DamagedConfig, onDamagedEnd);
        SM.AddActor(damagedActor);

        DeadActor deadActor = new DeadActor(SM, sfConfig.DeadConfig);
        SM.AddActor(deadActor);

        MoabAISkillActor aiAttackActor = new MoabAISkillActor(SM, sfConfig.MoabAISkillConfig, sfConfig.InitialBaseDamage);
        SM.AddActor(aiAttackActor);

        AppearanceActor appreanceActor = new AppearanceActor(SM, sfConfig.AppearanceConfig);
        SM.AddActor(appreanceActor);

    }

    public override void OnHit(DamageInfo damageInfo)
    {
        MoabAISkillActor aiSkillActor = SM.GetActor<MoabAISkillActor>(eActorType.AIAttack);
        if(aiSkillActor.CurrentAttackState == eMoabAttackState.SideStep)
        {
            return;
        }

        bool result = aiSkillActor.TryEvade();
        if(result)
        {
            return;
        }
        base.OnHit(damageInfo);
    }
}