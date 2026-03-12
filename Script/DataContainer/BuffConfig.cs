using UnityEngine;

public class BuffData : ExcelBasedSO
{
    public eBuffNameID BuffNameIDEnum;
    public eBuffType BuffTypeEnum;

    public ParticleBinder AuraFX;
    public ParticleBinder InstanceFX;
    public bool IsForPlayer;
    public float TickGap;
    public float AdditionalPowerPerStack;
    public float AdditionalDurationPerStack;
    public int MaxStack;

    [SerializeField] private string sfBuffNameID;
    [SerializeField] private string sfBuffType;
    [SerializeField] private string sfAuraFXPath;
    [SerializeField] private string sfInstanceFXPath;


    public override void AutoUpdate()
    {
        tryParsingStringAndAutoSetToEnum(sfBuffNameID, ref BuffNameIDEnum);
        tryParsingStringAndAutoSetToEnum(sfBuffType, ref BuffTypeEnum);
        if(sfAuraFXPath != null && sfAuraFXPath != "")
        {
            AuraFX = Resources.Load<ParticleBinder>(sfAuraFXPath);
        }

        if (sfInstanceFXPath != null && sfInstanceFXPath != "")
        {
            InstanceFX = Resources.Load<ParticleBinder>(sfInstanceFXPath);
        }
    }
}

