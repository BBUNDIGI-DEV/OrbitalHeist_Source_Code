#if UNITY_EDITOR

using System;
using UnityEditor;
using UnityEngine;
[InitializeOnLoad]
public static class EnumValidation
{
    static EnumValidation()
    {
        validateEnumRepresentSceneName<eSceneName>();
        validateEnumRepresentSceneName<eAutoLoadedSceneName>();
        validateEnumAllContainsLayerMask<eLayerName>();
    }

    private static void validateEnumRepresentSceneName<T>() where T : Enum
    {
        string[] sceneAssets = AssetDatabaseUtils.GetScenesAssetPath();

        string[] enumNameArray = Enum.GetNames(typeof(T));

        for (int i = 0; i < enumNameArray.Length; i++)
        {
            if(enumNameArray[i] == "Count" || enumNameArray[i] == "None")
            {
                continue;
            }

            bool isFounded = false;
            for (int j = 0; j < sceneAssets.Length; j++)
            {
                 if(sceneAssets[j].Contains(enumNameArray[i]))
                {
                    isFounded = true;
                    break;
                }
            }

            if(!isFounded)
            {
                Debug.LogError($"Validation Fail" +
                    $"Enum Name [{typeof(T).Name}] / Failed Enum : [{enumNameArray[i]}]");
            }
        }
    }

    private static void validateEnumAllContainsLayerMask<T>()
    {
        string[] enumNameArray = Enum.GetNames(typeof(T));


        for (int i = 0; i < enumNameArray.Length; i++)
        {
            if(LayerMask.NameToLayer(enumNameArray[i]) == -1)
            {
                Debug.LogError($"LayerMask Naming Enum Validation Fail" +
               $"Enum Name [{typeof(T).Name}] / Failed Enum : [{enumNameArray[i]}]");
            }    
        }
    }
}
#endif