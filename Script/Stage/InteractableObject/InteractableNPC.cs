using UnityEngine;
using UnityEngine.UI;

public class InteractableNPC : InteractableBase
{
    [SerializeField] private eDialogueTag sfTag;

    public override void InvokeInteraction()
    {
        base.InvokeInteraction();
        UIManager.Instance.SFDialogueManager.PlayDialouge(sfTag, DisableInteractable);
    }
}
