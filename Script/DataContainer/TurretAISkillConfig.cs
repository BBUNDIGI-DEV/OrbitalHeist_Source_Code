using Sirenix.OdinInspector;
using UnityEngine;

[CreateAssetMenu(menuName = "DataContainer/TurretAISkillConfig")]
public class TurretAISkillConfig : ActorConfigDataContainerBase
{
    public SkillConfig PlayerAimingMissileConfig;
    public int PlayerAimingAttackMin;
    public int PlayerAimingAttackMax;

    public SkillConfig RandomSpreadMissileConfig;
    public int RandomSpreadMissileMin;
    public int RandomSpreadMissileMax;

    public SkillConfig FrameLauncherConfig;
    public float Duration;
    public float RotateSpeed;

    protected override void initializeConfig()
    {
        BaseConfig.ActorType = eActorType.AIAttack;
        BaseConfig.IsUpdatedActor = true;
    }

}

