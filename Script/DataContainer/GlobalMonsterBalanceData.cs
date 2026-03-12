using UnityEngine;

[CreateAssetMenu(menuName = "DataContainer/GlobalMonsterBalanceData")]
public class GlobalMonsterBalanceData : ScriptableObject
{
    public float GlobalDamageMultiplier;
    public float GlobalAttackSpeedMultiplier;
    public float GlobalMaxHPMultiplier;
    public float GlobalDefenceMultiplier;
}  
