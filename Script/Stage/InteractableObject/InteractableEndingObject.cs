using UnityEngine.UI;

public class InteractableEndingObject : InteractableBase
{
    public override void InvokeInteraction()
    { 
        //System.Action onDialgoueEnd = () => PlayerManager.Instance.IsInputEnabled = false;
        //onDialgoueEnd += () => UIManager.Instance.SFEndingCreditUI.SetActive(true);

        //UIManager.Instance.SFDialogueManager.PlayDialouge(eDialogueTag.Stage2Boss, onDialgoueEnd);

        base.InvokeInteraction();
        UIManager.Instance.SFGameEnd.SetActive(true);
    }
}
