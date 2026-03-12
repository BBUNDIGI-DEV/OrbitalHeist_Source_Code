using UnityEngine;

[CreateAssetMenu(menuName = "DataContainer/PlayerConfig")]
public class PlayerConfig : ScriptableObject
{
    public eCharacterName CharacterName;
    public eRuleType RuleType;
    public float InitialMaxHP = 100;
    public float BaseDamage;
    public float BaseDefense;
    public float UltimateGuageAutoIncreasement;
    public float MaxUltimageGuage = 1.0f;
    public float UltimateGuageIncreasementPerDamage;

    public MovementConfig MovementConfig;
    public DashConfig DashConfig;
    public DamagedActorConfig DamagedActorConfig;
    public DeadActorConfig DeadActorConfig;

    public SkillConfig NormalAttack;
    public SkillConfig NormalChargeAttack;
    public SkillConfig DashAttack;
    public SkillConfig SpecialAttack;
    public SkillConfig SwitchingAttack;
    public SkillConfig UltimateAttack;
    public SkillConfig TryCounterAttack;
    public SkillConfig CounterAttack;
}

public enum eRuleType
{
    Dealer,
    Supporter,
    Healer,
}

public enum eCharacterName
{
    None = -1,
    Glanda,
    Hypo,
    Shiv,
}