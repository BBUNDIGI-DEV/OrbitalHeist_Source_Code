using UnityEngine;

public class InteractableSceneSwitchingDoor : InteractableDoor
{
    [field:SerializeField] public eSceneName SFSwitchingScene
    {
        get; private set;
    }

    [field: SerializeField]
    public Transform SFDoorApproachPoint
    {
        get; private set;
    }

    [field: SerializeField]
    public Transform SFPlayerExitPoint
    {
        get; private set;
    }
    
    public override void InvokeInteraction()
    {
        base.InvokeInteraction();
        PlayerManager.Instance.CurrentPlayer.Value.MoveToAnotherScene(SFDoorApproachPoint.position, SFPlayerExitPoint.position, SFSwitchingScene);
    }
}
