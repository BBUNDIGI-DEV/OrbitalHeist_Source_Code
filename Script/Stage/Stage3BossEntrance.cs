using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.UI;

public class Stage3BossEntrance : MonoBehaviour
{
    [SerializeField] private Transform sfBattleEntrance;
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
        mTimelinePD.Play();
        UIManager.Instance.UICamera.enabled = false;
    }

    public void WarpPlayerToBattlePos()
    {
        PlayerManager.Instance.WarpCurrentPlayer(sfBattleEntrance.position);
    }

    private void onTimelineEnd(PlayableDirector pd)
    {
        PlayerManager.Instance.IsInputEnabled = true;
        UIManager.Instance.UICamera.enabled = true;
        gameObject.SetActive(false);
    }
}
