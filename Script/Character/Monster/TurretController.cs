
public class TurretController : MonsterBase
{
    protected override void Awake()
    {
        base.Awake();
    }

    protected override void initializeStateMachine()
    {
        SM = new ActorStateMachine(this, false);
        DamagedActor damagedActor = new DamagedActor(SM, sfConfig.DamagedConfig, onDamagedEnd);
        SM.AddActor(damagedActor);

        DeadActor deadActor = new DeadActor(SM, sfConfig.DeadConfig);
        SM.AddActor(deadActor);

        TurretAISkillActor aiAttackActor = new TurretAISkillActor(SM, sfConfig.TurretAISkillConfig, sfConfig.InitialBaseDamage);
        SM.AddActor(aiAttackActor);

        AIMovementActor aiMovementActor = new AIMovementActor(SM, sfConfig.AIMovementConfig, null);
        SM.AddActor(aiMovementActor);
    }
}