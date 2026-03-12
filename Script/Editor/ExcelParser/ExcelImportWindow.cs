using Sirenix.OdinInspector.Editor;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;
using System;
using System.Linq;

public class ExcelImportWindow : OdinEditorWindow
{
    [SerializeField, Sirenix.OdinInspector.FilePath(RequireExistingPath = true)] private string sfExcelFilePath;

    [SerializeField, FolderPath(RequireExistingPath = true)] private string sfExportPath;
    [SerializeField, ShowIf("isValidExcelFilePath")] private string sfSheetName;
    [SerializeField, TypeSelectorSettings(FilterTypesFunction= "typeFilter", ShowCategories = false)] private Type sfTypeName;


    [MenuItem("Tools/DataParsing/ExcelParser")]
    private static void OpenWindow()
    {
        GetWindow<ExcelImportWindow>().Show();
    }

    [ShowIf("isValidExcelFilePath"), Button]
    private void clearAllData()
    {
        ExcelParser.ClearAllDataByType(sfExportPath, sfTypeName);
    }

    // Start is called before the first frame update
    [ShowIf("isValidExcelFilePath"), Button]
    public void Parsing()
    {
        ExcelParser.ClearAllDataByType(sfExportPath, sfTypeName);
        ExcelParser.ParsingCSVToSO(sfExcelFilePath, sfSheetName, sfExportPath, sfTypeName);
        EditorUtility.UnloadUnusedAssetsImmediate();
    }

    [ShowIf("isValidExcelFilePath"), Button]
    public void UpdateGradeEnum()
    {
        const string ENUM_CS_FILE_PATH = @"Assets/Script/Enum/eAbilNameID.cs";
        GrowthAbilityData[] allDatas = Resources.LoadAll<GrowthAbilityData>("GrowthData");
        string[] abilNameIDArr = new string[allDatas.Length];
        for (int i = 0; i < abilNameIDArr.Length; i++)
        {
            abilNameIDArr[i] = allDatas[i].AbilNameIDString;
        }

        abilNameIDArr = abilNameIDArr.Distinct().ToArray();


        AutoEnumUpdator.GenerateEnumFromStringArray(ENUM_CS_FILE_PATH, abilNameIDArr);
    }

    [Button]
    public void ParsingSkillConfig()
    {
        ExcelParser.SkillConfigParsing();
    }

    private bool typeFilter(Type type)
    {
        if(type.IsAbstract)
        {
            return false;
        }

        return typeof(ExcelBasedSO).IsAssignableFrom(type);
    }

    private bool isValidExcelFilePath()
    {
        return ExcelParser.ExcelFileExit(sfExcelFilePath);
    }
}
