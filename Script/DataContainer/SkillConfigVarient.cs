using Sirenix.OdinInspector;
using UnityEngine;

[CreateAssetMenu(menuName = "DataContainer/SkillConfigVarient")]
public class SkillConfigVarient : ScriptableObject
{
    public SkillConfig Original;

    [ShowIf("Original"), ToggleGroup("OverrideDamagePercentage")] public bool OverrideDamagePercentage;
    [ShowIf("Original"), ToggleGroup("OverrideDamagePercentage")] public float DamagePercentage;

    [ShowIf("Original"), ToggleGroup("OverrideMeleeAttackData")] public bool OverrideMeleeAttackData;
    [ShowIf("Original"), ToggleGroup("OverrideMeleeAttackData")] public MeleeAttackData[] MeleeAttackData;

    [ShowIf("Original"), ToggleGroup("OverrideProjectieDataAttackData")] public bool OverrideProjectieDataAttackData;
    [ShowIf("Original"), ToggleGroup("OverrideProjectieDataAttackData")] public ProjectileAttackData[] ProjectileAttackData;

    [ShowIf("Original"), ToggleGroup("OverrideFXSpawnData")] public bool OverrideFXSpawnData;
    [ShowIf("Original"), ToggleGroup("OverrideFXSpawnData")] public FXSpawnData[] FXSpawnData;

    [ShowIf("Original"), ToggleGroup("OverrideTransitionData")] public bool OverrideTransitionData;
    [ShowIf("Original"), ToggleGroup("OverrideTransitionData")] public AttackTransitionData TransitionData;


    public SkillConfig InstantiateSkillConfigVarient()
    {
        SkillConfig instance = Original.InstantiateWithNotNameChanging();

        if(OverrideDamagePercentage)
        {
            instance.DamagePercentage = DamagePercentage;
        }
        if (OverrideMeleeAttackData)
        {
            System.Array.Copy(MeleeAttackData, instance.MeleeAttackData, MeleeAttackData.Length);
        }
        if(OverrideProjectieDataAttackData)
        {
            System.Array.Copy(ProjectileAttackData, instance.ProjectileData, ProjectileAttackData.Length);
        }
        if (OverrideFXSpawnData)
        {
            instance.FXSpawnData = FXSpawnData;
            System.Array.Copy(FXSpawnData, instance.FXSpawnData, FXSpawnData.Length);
        }
        if (OverrideTransitionData)
        {
            instance.TransitionData = TransitionData;
            System.Array.Copy(TransitionData.MoveToSpecificDest, instance.TransitionData.MoveToSpecificDest, TransitionData.MoveToSpecificDest.Length);
        }
        return instance;
    }

    [ShowIf("Original", null), Button(DirtyOnClick = true)]
    private void setSameAsOriginalData()
    {
        DamagePercentage = Original.DamagePercentage;
        MeleeAttackData = Original.MeleeAttackData;
        ProjectileAttackData = Original.ProjectileData;
        FXSpawnData = Original.FXSpawnData;
        TransitionData = Original.TransitionData;
    }
}