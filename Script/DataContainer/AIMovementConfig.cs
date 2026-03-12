using UnityEngine;
using Sirenix.OdinInspector;

[CreateAssetMenu(menuName = "DataContainer/AIMovementConfig")]
public class AIMovementConfig : ActorConfigDataContainerBase
{
    public bool IsSkipWandering;
    [ShowIf("@!IsSkipWandering")] public float DetectionRange;
    [HideIf("IsSkipWandering")] public MonsterWanderingOption WanderingOption;
    public MonsterCombatMovementOption CombatMovementOption;

    protected override void initializeConfig()
    {
        BaseConfig.ActorType = eActorType.AIMovement;
        BaseConfig.IsUpdatedActor = true;
    }
}

[System.Serializable]
public struct MonsterWanderingOption
{
    public eWanderingActionType Type;

    [ShowIf("Type", eWanderingActionType.RandomWalkAroundRange)] public float WanderingRange;
    [ShowIf("Type", eWanderingActionType.RandomWalkAroundRange)] public float WanderingMinRange;

    [ShowIf("Type", eWanderingActionType.RandomWalkAroundRange)] public float MinWaitTime;
    [ShowIf("Type", eWanderingActionType.RandomWalkAroundRange)] public float MaxWaitTime;

}

public enum eWanderingActionType
{
    Stop,
    RandomWalkAroundRange,
}


[System.Serializable]
public struct MonsterCombatMovementOption
{
    public eCombatMovementType MovementType;
    [ShowIf("MovementType", eCombatMovementType.RushToPlayer), Range(0.5f, 1.5f)] public float RushSpeedIncreasement;
    [ShowIf("MovementType", eCombatMovementType.RushToPlayer)] public float StopDistance;

    [ShowIf("MovementType", eCombatMovementType.EvadeFromPlayer)] public float EvadeInvokingThreshold;
    [ShowIf("MovementType", eCombatMovementType.EvadeFromPlayer)] public float EvadeDistance;

    [ShowIf("MovementType", eCombatMovementType.EvadeFromPlayer)] public float MinWaitTime;
    [ShowIf("MovementType", eCombatMovementType.EvadeFromPlayer)] public float MaxWaitTime;

    [ShowIf("MovementType", eCombatMovementType.LookPlayer)] public float LookPlayerSpeed;
}

public enum eCombatMovementType
{
    RushToPlayer,
    EvadeFromPlayer,
    LookPlayer,
    DoNothing,
}