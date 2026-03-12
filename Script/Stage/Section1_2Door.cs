using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Playables;
using UnityEngine.UI;

public class Section1_2Door : MonoBehaviour
{
    [SerializeField] private UnityEvent sfOnActingEnd;
    private PlayableDirector mTimelinePD;

    public void Awake()
    {
        mTimelinePD = GetComponent<PlayableDirector>();
        mTimelinePD.stopped += onTimelineEnd;
    }

    public void StartEntrance()
    {
        PlayerManager.Instance.IsInputEnabled = false;
        mTimelinePD.Play();
    }

    private void onTimelineEnd(PlayableDirector pd)
    {
        PlayerManager.Instance.IsInputEnabled = true;
        UIManager.Instance.SFDialogueManager.PlayDialouge(eDialogueTag.Stage1_02_CrachGate, sfOnActingEnd.Invoke);

    }
}
