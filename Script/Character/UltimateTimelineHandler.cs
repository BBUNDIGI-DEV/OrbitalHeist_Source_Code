using UnityEngine;
using UnityEngine.Playables;

public class UltimateTimelineHandler : MonoBehaviour
{
    [SerializeField] private Transform sfModelTrans;
    private PlayableDirector mTimelineDirector;
    private RenderToggle mPlayerRenderToggle;
    public void Awake()
    {
        mTimelineDirector = GetComponentInChildren<PlayableDirector>(true);
        mTimelineDirector.stopped += onUltimateSkillDone;
    }

    public void PlayUltimateSkillTimeline(Vector3 playerPos, RenderToggle playerRenderToggle)
    {
        mPlayerRenderToggle = playerRenderToggle;
        TimeScaleUtil.Instance.AddTimeScale("UltimateTimeline", new PriorityAndTimeScalePair(eTimeScaleTrigger.UltimateCinemachinePlaying, 0.01f));
        Physics.simulationMode = SimulationMode.Script;
        CameraManager.Instance.DisableUICamera();
        mTimelineDirector.Play();
        mPlayerRenderToggle.Toggle(false);
    }

    private void onUltimateSkillDone(PlayableDirector dir)
    {
        TimeScaleUtil.Instance.RemoveTimeScale("UltimateTimeline");
        Physics.simulationMode = SimulationMode.FixedUpdate;
        mPlayerRenderToggle.Toggle(true);
        CameraManager.Instance.EnableUICamera();
        PlayerManager.Instance.IsInputEnabled = true;
    }

}
