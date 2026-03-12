using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.UI;

public class InteractableStage1Door : InteractableBase
{
    [SerializeField] private PlayableDirector sfPD;

    public override void InvokeInteraction()
    {
        base.InvokeInteraction();
        UIManager.Instance.UICamera.enabled = false;
        PlayerManager.Instance.IsInputEnabled = false;
        sfPD.Play();
    }

    public override void DisableInteractable()
    {
        base.DisableInteractable();
        SceneSwitchingManager.Instance.LoadOtherScene(eSceneName.Scene_Stage2, true);
        PlayerManager.Instance.CurrentPlayer.Value.gameObject.SetActive(true);
    }

    public void HidePlayer()
    {
        PlayerManager.Instance.CurrentPlayer.Value.gameObject.SetActive(false);
    }
}
