using UnityEngine;
using TMPro;

[CreateAssetMenu(menuName = "DataContainer/FloatingInfoMessageConfig")]
public class FloatingInfoMessageConfig : ScriptableObject
{
    public eFloatingInfoMessageTag MessageTag;
    public TMP_ColorGradient TextColor;
    public string Contents;
    [Range(0.1f, 2)] public float FontSizeMulitplier = 1.0f;
}

public enum eFloatingInfoMessageTag
{
    GetAttackSpeedBuff,
    GetPowerUpBuff,
    GetPowerOverwalming,
    GetShivFire,
    GetUltimateGuageSmall,
    GetUltimateGuageBig,
    GetShiv,
    GetHypo,
}