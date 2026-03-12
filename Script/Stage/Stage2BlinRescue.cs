using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.UI;

public class Stage2BlinRescue : MonoBehaviour
{
    private PlayableDirector mTimelinePD;
    [SerializeField] private Transform sfRenewPos;

    public void Awake()
    {
        mTimelinePD = GetComponent<PlayableDirector>();
    }

    public void StartEntrance()
    {
        gameObject.SetActive(true);
        PlayerManager.Instance.IsInputEnabled = false;
        UIManager.Instance.UICamera.enabled = false;
        mTimelinePD.Play();
    }

    public void InvokeDialogue()
    {
        UIManager.Instance.SFDialogueManager.PlayDialouge(eDialogueTag.Stage2_04_BlinRescue, onDialogueEnd);
        Playable root = mTimelinePD.playableGraph.GetRootPlayable(0);
        root.SetSpeed(0);
    }

    public void OnTimelineEnd()
    {
        PlayerManager.Instance.CurrentPlayer.Value.RenderToggle.Toggle(true);
        PlayerManager.Instance.IsInputEnabled = true;
        UIManager.Instance.UICamera.enabled = true;
        PlayerManager.Instance.WarpCurrentPlayer(sfRenewPos.position);
        gameObject.SetActive(false);
    }

    public void HidingPlayer()
    {
        PlayerManager.Instance.CurrentPlayer.Value.RenderToggle.Toggle(false);
    }

    private void onDialogueEnd()
    {
        Playable root = mTimelinePD.playableGraph.GetRootPlayable(0);
        root.SetSpeed(1);
    }
}
