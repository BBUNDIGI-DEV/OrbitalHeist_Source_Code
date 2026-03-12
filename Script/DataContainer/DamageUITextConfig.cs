using UnityEngine;

[CreateAssetMenu(menuName = "DataContainer/DamageUIText")]
public class DamageUITextConfig : ScriptableObject
{
    public float sfMinDamageThreshold;
    public float sfMaxDamageThreshold;

    public float sfSizeUpMultiplier;
    public float sfSizeDownMultiplier;

    public float sfAnimAtionSpeedMultiplier;
}
