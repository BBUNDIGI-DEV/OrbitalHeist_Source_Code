using UnityEngine;
using Sirenix.OdinInspector;
using FMODUnity;

[CreateAssetMenu(menuName = "DataContainer/DamagedActorConfig")]
public class DamagedActorConfig : ActorConfigDataContainerBase
{
    public AnimationClip DamagedAnimClip;
    public AnimationClip StunAnimClip;
    public EventReference DamagedSound;


    protected override void initializeConfig()
    {
        BaseConfig.ActorType = eActorType.Damaged;
        BaseConfig.IsUpdatedActor = false;
    }
}

