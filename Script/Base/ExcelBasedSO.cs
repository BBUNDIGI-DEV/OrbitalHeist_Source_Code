using UnityEngine;
using Sirenix.OdinInspector;
using UnityEditor;
using System;

public abstract class ExcelBasedSO : ScriptableObject
{
    [PropertyOrder(-500)] public int ID;
    [ReadOnly, PropertyOrder(-500)] public string ParsedFromInfo;

    [OnInspectorInit]
    public abstract void AutoUpdate();

    protected bool tryParsingStringAndAutoSetToEnum<T>(string param, ref T originalValue) where T: struct
    {
        T parsedResult;
        bool buffNameIDParsingResult = Enum.TryParse(param, out parsedResult);
        Debug.Assert(buffNameIDParsingResult, $"{param}문자열은 반드시 {typeof(T).Name}에 정의되어 있어야 합니다.", this);


        if (!Equals(originalValue, parsedResult))
        {
            originalValue = parsedResult;
            return true;
        }

        return false;
    }
}
