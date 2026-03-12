using UnityEngine;
using Sirenix.OdinInspector;
using UnityEditor;

[CreateAssetMenu(fileName ="MonsterConfig", menuName = "DataContainer/MonsterConfig")]
public class MonsterConfig : DataConfigBase
{
    public eMonsterName MonsterName;
    public int InitialMaxHP;
    public int InitialHP;
    public float InitialSpeed;
    public float InitialDefense;
    public float InitialBaseDamage;
    public float InitialShieldAmount;
    public float InitialMaxShieldAmount;
    public bool InitialSuperArmor;
    public bool DontDestoryOnDead;

    public DeadActorConfig DeadConfig;
    public AIMovementConfig AIMovementConfig;
    public DamagedActorConfig DamagedConfig;

    [HideIf("MonsterName", eMonsterName.Moab)] [HideIf("MonsterName", eMonsterName.Turret)] public AISkillConfig AISkillConfig;
    [HideIf("MonsterName", eMonsterName.Moab)] [HideIf("MonsterName", eMonsterName.Turret)] public AppearanceActorConfig AppearanceConfig;
    [ShowIf("MonsterName", eMonsterName.Moab)] public MoabAISkillConfig MoabAISkillConfig;
    [ShowIf("MonsterName", eMonsterName.Turret)] public TurretAISkillConfig TurretAISkillConfig;


#if UNITY_EDITOR
    [Button]
    private void AutoFindConfig()
    {

    }
#endif

}
