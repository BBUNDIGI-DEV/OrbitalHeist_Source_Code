using UnityEngine;

public class ItemData : ExcelBasedSO
{
    public eItemNameID ItemNameIDEnum;
    public float DropRate;
    public float Power1;
    public float Power2;
    public float Power3;
    public float Duration;
    [SerializeField] private string sfItemNameID;

    public override void AutoUpdate()
    {
        tryParsingStringAndAutoSetToEnum(sfItemNameID, ref ItemNameIDEnum);
    }
}

public enum eItemNameID
{
    PersonalHealingHPLow,
    PersonalHealingHPMiddle,
    PersonalHealingHPMax,
    PersonalUltimateGaugeIncarease,
    PersonalUltimateGaugeMax,
    PowerUp,
    AttackSpeedUp,
    SuperMode,
}

