using FMODUnity;
using Sirenix.OdinInspector;
using UnityEngine;

[CreateAssetMenu(menuName = "DataContainer/DeadActorConfig")]
public class DeadActorConfig : ActorConfigDataContainerBase
{
    private const float MAX_ACCELARTION_FACTOR = 3.0f;
    [Range(-1, 1)]
    public float DeadAccel;
    public EventReference DeadSound;
    public bool SkipDissolve;


    [ShowInInspector]
    private float DURATION
    {
        get
        {
            return GetDuration();
        }
    }

    [SerializeField] private AnimationClip sfDeadClip;

    public float GetDuration()
    {
        if (sfDeadClip == null)
        {
            return 0.0f;
        }

        return sfDeadClip.length
            * SkillUtils.ConvertInspectorAccelToMultiplier(DeadAccel, MAX_ACCELARTION_FACTOR);
    }

    public float GetCaculatedAnimSpeed()
    {
        return 1.0f / SkillUtils.ConvertInspectorAccelToMultiplier(DeadAccel, MAX_ACCELARTION_FACTOR);
    }


    protected override void initializeConfig()
    {
        BaseConfig.ActorType = eActorType.Dead;
        BaseConfig.IsUpdatedActor = false;
    }
}
