using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using System;
using System.ComponentModel;
using System.IO;
using System.Reflection;
using UnityEditor;
using UnityEngine;

public static class ExcelParser
{
    public static void ParsingCSVToSO(string excelFilePath, string sheetName, string exportPath, Type dataType)
    {
        using (var fs = new FileStream(excelFilePath, FileMode.Open, FileAccess.Read))
        {
            string workbookName = Path.GetFileNameWithoutExtension(excelFilePath);
            IWorkbook workbook = new XSSFWorkbook(fs);
            IFormulaEvaluator evaluator = workbook.GetCreationHelper().CreateFormulaEvaluator();
            if (workbook == null)
            {
                Debug.LogError($"You Cannot read [{excelFilePath}, path not founded]");
                return;
            }
            ISheet targetSheet = workbook.GetSheet(sheetName);

            if (targetSheet == null)
            {
                Debug.LogError($"You Cannot read sheet [{sheetName}, sheent name is not founded]");
                return;
            }

            int rowLength = GetValidRowRange(targetSheet, evaluator);
            IRow headerRow = targetSheet.GetRow(targetSheet.FirstRowNum);
            int columnLength = GetValidColumnRange(headerRow, evaluator);
            FieldInfo[] fieldInfos = dataType.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            int[] nameToFieldIndex = new int[columnLength];


            for (int i = 0; i < columnLength; i++)
            {
                string rawData = headerRow.GetCell(i + headerRow.FirstCellNum).ToString();

                if(rawData == "")
                {
                    continue;
                }

                bool isSearched = false;
                for (int j = 0; j < fieldInfos.Length; j++)
                {
                    string name = fieldInfos[j].Name;

                    if(fieldInfos[j].IsPrivate)
                    {
                        name = name.Substring(1);
                    }

                    if (rawData == name)
                    {
                        nameToFieldIndex[i] = j;
                        isSearched = true;
                        break;
                    }
                }

                Debug.Assert(isSearched,
                    "Name Is Not Founded!! " +
                    $"Index : {i}, Name : {rawData}");
            }

            for (int i = targetSheet.FirstRowNum + 1; i < targetSheet.FirstRowNum + rowLength; i++)
            {
                ExcelBasedSO newOBJ = ScriptableObject.CreateInstance(dataType) as ExcelBasedSO;
                IRow dataRow = targetSheet.GetRow(i);
                for (int j = dataRow.FirstCellNum; j < dataRow.FirstCellNum + columnLength; j++)
                {
                    int fieldIndex = nameToFieldIndex[j - dataRow.FirstCellNum];
                    string rawData = GetString(dataRow.GetCell(j), evaluator);
                    Type currentFieldType = fieldInfos[fieldIndex].FieldType;

                    if (string.IsNullOrEmpty(rawData))
                    {
                        continue;
                    }

                    if (currentFieldType.IsArray)
                    {
                        Type elementType = currentFieldType.GetElementType();
                        if (elementType.IsArray)
                        {
                            Type elementType2 = elementType.GetElementType();
                            string[] arrayElementDatas = rawData.Split('_');
                            Array data2DArray = Array.CreateInstance(elementType, arrayElementDatas.Length);
                            for (int k = 0; k < arrayElementDatas.Length; k++)
                            {
                                string[] arrayDatas = arrayElementDatas[k].Split(' ');

                                //fieldInfos[fieldIndex].SetValue(resultArr[i], )
                                Array dataArray = Array.CreateInstance(elementType2, arrayDatas.Length);
                                for (int l = 0; l < dataArray.Length; l++)
                                {
                                    var elementConverter = TypeDescriptor.GetConverter(elementType2).ConvertFromString(arrayDatas[l]);
                                    dataArray.SetValue(Convert.ChangeType(elementConverter, elementType2), l);
                                }
                                data2DArray.SetValue(dataArray, k);
                            }
                            fieldInfos[fieldIndex].SetValue(newOBJ, data2DArray);
                        }
                        else
                        {
                            string[] arrayDatas = rawData.Split('|');

                            Array dataArray = Array.CreateInstance(elementType, arrayDatas.Length);
                            for (int k = 0; k < dataArray.Length; k++)
                            {
                                var elementConverter = TypeDescriptor.GetConverter(elementType).ConvertFromString(arrayDatas[k]);
                                dataArray.SetValue(Convert.ChangeType(elementConverter, elementType), k);
                            }
                            fieldInfos[fieldIndex].SetValue(newOBJ, dataArray);
                        }
                    }
                    else// Value Types
                    {
                        var converter = TypeDescriptor.GetConverter(currentFieldType).ConvertFromString(rawData);
                        fieldInfos[fieldIndex].SetValue(newOBJ, Convert.ChangeType(converter, currentFieldType));
                    }
                }

                string objName = $"{ dataType.Name }_{ newOBJ.ID}";
                newOBJ.name = objName;

                string assetPath = @$"{exportPath}\{objName}.asset";
                UnityEngine.Object prevAsset = AssetDatabase.LoadAssetAtPath(assetPath, dataType);

                newOBJ.ParsedFromInfo = $"{workbookName}/{targetSheet.SheetName}";
                newOBJ.AutoUpdate();
                if (prevAsset == null)
                {
                    AssetDatabase.CreateAsset(newOBJ, assetPath);
                }
                else
                {
                    EditorUtility.CopySerialized(newOBJ, prevAsset);
                    AssetDatabase.SaveAssets();
                    UnityEngine.Object.DestroyImmediate(newOBJ);
                }
            }
        }
    }

    public static void SkillConfigParsing()
    {
        const string SKILL_CONFIG_FILE_PATH = @"Assets/Settings/Resources/_SkillConfigDataTable.xlsx";
        using (var fs = new FileStream(SKILL_CONFIG_FILE_PATH, FileMode.Open, FileAccess.Read))
        {
            string workbookName = Path.GetFileNameWithoutExtension(SKILL_CONFIG_FILE_PATH);
            IWorkbook workbook = new XSSFWorkbook(fs);
            IFormulaEvaluator evaluator = workbook.GetCreationHelper().CreateFormulaEvaluator();
            if (workbook == null)
            {
                Debug.LogError($"You Cannot read [{SKILL_CONFIG_FILE_PATH}, path not founded]");
                return;
            }
            ISheet targetSheet = workbook.GetSheet("SkillConfigParser");

            if (targetSheet == null)
            {
                Debug.LogError($"You Cannot read sheet [SkillConfigParser, sheent name is not founded]");
                return;
            }

            int rowLength = GetValidRowRange(targetSheet, evaluator);
            IRow headerRow = targetSheet.GetRow(targetSheet.FirstRowNum);
            int columnLength = GetValidColumnRange(headerRow, evaluator);

            string[] allSkillConfigGUID = AssetDatabase.FindAssets("t:SkillConfig", new string[] { "Assets/Settings/Resources" });
            SkillConfig[] allSkillConfigs = new SkillConfig[allSkillConfigGUID.Length];
            for (int i = 0; i < allSkillConfigs.Length; i++)
            {
                allSkillConfigs[i] = AssetDatabase.LoadAssetAtPath<SkillConfig>(AssetDatabase.GUIDToAssetPath(allSkillConfigGUID[i]));
            }

            for (int i = targetSheet.FirstRowNum + 1; i < targetSheet.FirstRowNum + rowLength; i++)
            {
                IRow dataRow = targetSheet.GetRow(i);
                string configName = GetString(dataRow.GetCell(dataRow.FirstCellNum), evaluator);
                string dataSet = GetString(dataRow.GetCell(dataRow.FirstCellNum + 1), evaluator);

                SkillConfig targetConfig = null;
                for (int j = 0; j < allSkillConfigs.Length; j++)
                {
                    if(allSkillConfigs[j].name == configName)
                    {
                        targetConfig = allSkillConfigs[j];
                        break;
                    }
                }
                Debug.Assert(targetConfig != null, $"{configName} not founded");
                if(targetConfig == null)
                {
                    continue;
                }
                targetConfig.UpdateSkillConfigFromString(dataSet);
            }
            AssetDatabase.SaveAssets();
        }
    }

    public static int GetValidRowRange(ISheet checkSheet, IFormulaEvaluator evaluator)
    {
        int validLastRowNum = checkSheet.LastRowNum;
        short firstCellNum = 0;
        for (int i = checkSheet.FirstRowNum; i <= checkSheet.LastRowNum; i++)
        {
            IRow checkRow = checkSheet.GetRow(i);
            if(i == checkSheet.FirstRowNum)
            {
                firstCellNum = checkRow.FirstCellNum;
                continue;
            }
            if(checkRow == null)
            {
                validLastRowNum = i;
                break;
            }
            if(checkRow.FirstCellNum == -1)
            {
                validLastRowNum = i;
                break;
            }
            if(checkRow.GetCell(firstCellNum) == null)
            {
                validLastRowNum = i;
                break;
            }
            else if (string.IsNullOrEmpty(GetString(checkRow.GetCell(firstCellNum), evaluator)))
            {
                validLastRowNum = i;
                break;
            }
        }
        return validLastRowNum - checkSheet.FirstRowNum;
    }

    public static int GetValidColumnRange(IRow checkRow, IFormulaEvaluator evaluator)
    {
        int validLastColumnNum = checkRow.LastCellNum;
        for (int i = checkRow.FirstCellNum; i <= checkRow.LastCellNum; i++)
        {
            if (checkRow == null || checkRow.GetCell(i) == null || string.IsNullOrEmpty(GetString(checkRow.GetCell(i), evaluator)))
            {
                validLastColumnNum = i;
                break;
            }
        }

        return validLastColumnNum - checkRow.FirstCellNum;
    }

    public static bool ExcelFileExit(string excelFilePath)
    {
        string fullPath = Path.GetFullPath(excelFilePath);
        if (!File.Exists(fullPath))
        {
            return false;
        }

       string extenstion = Path.GetExtension(fullPath);

        if(extenstion != ".xlsx")
        {
            return false;   
        }
        return true;
    }

    public static void ClearAllDataByType(string path, Type dataType)
    {
        string[] assets = AssetDatabase.FindAssets($"t:{dataType}", new string[] { path });

        foreach (string guid1 in assets)
        {
            string assetPath = AssetDatabase.GUIDToAssetPath(guid1);
            AssetDatabase.DeleteAsset(assetPath);
        }
    }

    private static string GetString(ICell cell, IFormulaEvaluator evaluator)
    {
        if(cell == null)
        {
            return null;
        }

        string resultStr = evaluator.EvaluateInCell(cell).ToString();
        return resultStr;
    }
}
