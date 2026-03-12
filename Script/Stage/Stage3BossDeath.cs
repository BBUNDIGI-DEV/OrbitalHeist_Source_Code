using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.UI;

public class Stage3BossDeath : MonoBehaviour
{
    private PlayableDirector mTimelinePD;

    public void Awake()
    {
        mTimelinePD = GetComponent<PlayableDirector>();
        mTimelinePD.stopped += onTimelineEnd;
    }

    public void StartEntrance()
    {
        gameObject.SetActive(true);
        PlayerManager.Instance.IsInputEnabled = false;
        PlayerManager.Instance.CurrentPlayer.Value.gameObject.SetActive(false);
        mTimelinePD.Play();
        UIManager.Instance.UICamera.enabled = false;
    }

    private void onTimelineEnd(PlayableDirector pd)
    {
        SceneSwitchingManager.Instance.LoadOtherScene(eSceneName.Scene_EndingStoryCutscene, true);
    }
}
