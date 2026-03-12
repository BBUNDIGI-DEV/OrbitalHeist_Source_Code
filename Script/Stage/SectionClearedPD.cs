using UnityEngine;
using UnityEngine.Playables;

public class SectionClearedPD : SingletonClass<SectionClearedPD>
{
    public bool IsPlayed
    {
        get; private set;
    }

    [SerializeField] private PlayableDirector sfClearingPD;
    [SerializeField] private CameraActingEventData sfCameraActing;

    protected override void Awake()
    {
        base.Awake();
        sfClearingPD.stopped += onSectionClearedActingEnd;
    }
    public void PlaySectionCleared(MonsterBase lastDeadMonster)
    {
        IsPlayed = true;
        transform.position = lastDeadMonster.Translator.Trans.position;
        TimeScaleUtil.Instance.AddTimeScale("OnStageCleared", new PriorityAndTimeScalePair(eTimeScaleTrigger.StageCleared, 0.5f));
        sfClearingPD.Play();
        CameraManager.Instance.Actor.ProcessCameraActing(sfCameraActing);
        PlayerManager.Instance.IsInputEnabled = false;
    }

    private void onSectionClearedActingEnd(PlayableDirector pd)
    {
        IsPlayed = false;
        TimeScaleUtil.Instance.RemoveTimeScale("OnStageCleared");
        PlayerManager.Instance.IsInputEnabled = true;
    }
}
