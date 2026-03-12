using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.UI;
public class Stage2Entrance : MonoBehaviour
{
    private PlayableDirector mTimelinePD;

    private void Awake()
    {
        mTimelinePD = GetComponent<PlayableDirector>();
        mTimelinePD.stopped += onTimelineEnd;
    }

    private void Start()
    {
        PlayerManager.Instance.IsInputEnabled = false;
        PlayerManager.Instance.ToggleAllPlayer(false);
        mTimelinePD.Play();
    }

    public void PlayerShowUp()
    {
        PlayerManager.Instance.IsInputEnabled = true;
        PlayerManager.Instance.ToggleAllPlayer(true);
    }

    private void onTimelineEnd(PlayableDirector pd)
    {
        UIManager.Instance.UICamera.enabled = true;
        gameObject.SetActive(false);
    }
}
