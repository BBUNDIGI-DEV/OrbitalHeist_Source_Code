using UnityEngine;
using Sirenix.OdinInspector;

[CreateAssetMenu(menuName = "DataContainer/AppearanceActorConfig")]
public class AppearanceActorConfig : ActorConfigDataContainerBase
{
    private const float MAX_ACCELARTION_FACTOR = 3.0f;
    [Range(-1, 1)]
    public float AppearanceAccel;

    [ShowInInspector]
    private float DURATION
    {
        get
        {
            return GetDuration();
        }
    }

    [SerializeField] private AnimationClip sfPAppearanceClip;

    public float GetDuration()
    {
        if(sfPAppearanceClip == null)
        {
            return 0.0f;
        }

        return sfPAppearanceClip.length 
            * SkillUtils.ConvertInspectorAccelToMultiplier(AppearanceAccel, MAX_ACCELARTION_FACTOR);
    }

    public float GetCaculatedAnimSpeed()
    {
        return 1.0f / SkillUtils.ConvertInspectorAccelToMultiplier(AppearanceAccel, MAX_ACCELARTION_FACTOR);
    }


    protected override void initializeConfig()
    {
        BaseConfig.ActorType = eActorType.Appearance;
        BaseConfig.IsUpdatedActor = false;
    }
}


