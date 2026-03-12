using System;
using UnityEditor;
using UnityEngine;
using Sirenix.OdinInspector;

public class GrowthAbilityData : ExcelBasedSO
{
    [ReadOnly] public eAbilGrade AbilGradeEnum;
    [ReadOnly] public eAbilNameID AbilNameIDEnum;
    [ReadOnly] public eCharacterName ApplyingTargetCharacterEnum;
    public int OwnLimit;
    public string DynamicParam1;
    public string DynamicParam2;
    public string DynamicParam3;
    public string DynamicParam4;
    public string NameForUI;
    public string DescriptionForUI;
    public string AbilNameIDString
    {
        get
        {
            return sfAbilNameID;
        }
    }
    public Sprite IconSprite
    {
        get
        {
            if(sfIconSprite == null)
            {
                sfIconSprite = Resources.Load<Sprite>($"GrowthData/Icons/{ID}_{AbilNameIDString}");
            }
            return sfIconSprite;
        }
    }
    [SerializeField, ReadOnly] private string sfGrade;
    [SerializeField, ReadOnly] private string sfAbilNameID;
    [SerializeField, ReadOnly] private string sfAbilityApplyingTarget;
    [SerializeField, ReadOnly] private Sprite sfIconSprite;

    private bool checkDynamicParmType(Type param1 = null, Type param2 = null, Type param3 = null, Type param4 = null)
    {
        return
            isParsableFromType(param1, DynamicParam1) &&
            isParsableFromType(param2, DynamicParam2) &&
            isParsableFromType(param3, DynamicParam3) &&
            isParsableFromType(param4, DynamicParam4);
    }

    private bool isParsableFromType(Type param1, string from)
    {
        if (param1 == null)
        {
            return string.IsNullOrEmpty(from);
        }
        else
        {
            try
            {
                System.ComponentModel.TypeDescriptor.GetConverter(param1).ConvertFromString(DynamicParam1);
            }
            catch (Exception)
            {
                return false;
            }

            return true;
        }
    }

    public override void AutoUpdate()
    {
        if (sfAbilityApplyingTarget != "All")
        {
            tryParsingStringAndAutoSetToEnum(sfAbilityApplyingTarget, ref ApplyingTargetCharacterEnum);
        }
        else
        {
            ApplyingTargetCharacterEnum = eCharacterName.None;
        }

        tryParsingStringAndAutoSetToEnum(sfAbilNameID, ref AbilNameIDEnum);
        tryParsingStringAndAutoSetToEnum(sfGrade, ref AbilGradeEnum);

        sfIconSprite = Resources.Load<Sprite>($"GrowthData/Icons/{ID}_{AbilNameIDString}");
    }


#if UNITY_EDITOR
    private void OnValidate()
    {
        if (Application.isPlaying)
        {
            return;
        }

        if (ParsedFromInfo == "")
        {
            return;
        }
        bool isDirty = false;

        bool abilGradeParsingResult = Enum.TryParse(sfGrade, out eAbilGrade parsedAbilGrade);
        Debug.Assert(abilGradeParsingResult,
                $"mGrade문자열은 eAbilGrade중 하나여야 합니다. [{sfGrade}]", this);

        if (AbilGradeEnum != parsedAbilGrade)
        {
            AbilGradeEnum = parsedAbilGrade;
            isDirty = true;
        }

        Debug.Assert(Enum.IsDefined(typeof(eAbilNameID), sfAbilNameID), $"AbilityNameID문자열은 반드시 eAbilNameID에 정의되어 있어야 합니다, [{sfAbilNameID}]");
        Enum.TryParse(sfAbilNameID, out eAbilNameID parsedAbilNameID);

        if (AbilNameIDEnum != parsedAbilNameID)
        {
            AbilNameIDEnum = parsedAbilNameID;
            isDirty = true;
        }
        if (isDirty)
        {
            EditorUtility.SetDirty(this);
        }
    }


#endif
}


public enum eAbilGrade
{
    Common,
    Rare,
    Unique,
    Epic,
    Count,
}