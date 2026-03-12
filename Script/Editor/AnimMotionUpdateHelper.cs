using Sirenix.OdinInspector.Editor;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;
using System.Collections.Generic;

public class MotionUpdateWindow : OdinEditorWindow
{
    private const string ART_RESOURCES_PATH = "Assets/ArtResource";
    [SerializeField, 
        LabelText("New FBX Motion"),
        ValidateInput("FBXFileChecking"),
        Sirenix.OdinInspector.FilePath(Extensions = "fbx", AbsolutePath = false)]
    private string sfFBXPath;

    [MenuItem("Tools/Animation/MotionUpdator")]
    private static void OpenWindow()
    {
        GetWindow<MotionUpdateWindow>().Show();
    }

    [Button]
    private void tryUpdateMotion()
    {
        string progressDebugString = string.Empty;
        bool isSuccess = true;
        if (!checkMotionUpdateFBXFileValid(sfFBXPath, ref progressDebugString))
        {
            isSuccess = false;
            goto LogAndReturn;
        }

        AnimationClip importedClip = null;
        Object[] subassets = AssetDatabase.LoadAllAssetsAtPath(sfFBXPath);
        for (int j = 0; j < subassets.Length; j++)
        {
            if (subassets[j] is AnimationClip clip)
            {
                if(clip.name.Contains("preview"))
                {
                    continue;
                }

                if (clip.name.Contains("ANI_"))
                {
                    importedClip = clip;
                    break;
                }
            }
        }

        string[] searchedClipIDS = AssetDatabase.FindAssets(importedClip.name, new string[] { ART_RESOURCES_PATH });
        List<string> replacableClipIDList = new List<string>();

        for (int i = 0; i < searchedClipIDS.Length; i++)
        {
            AnimationClip searchedClip = AssetDatabase.LoadAssetAtPath<AnimationClip>
                (AssetDatabase.GUIDToAssetPath(searchedClipIDS[i]));

            if (AssetDatabase.IsMainAsset(searchedClip) && searchedClip.name == importedClip.name)
            {
                replacableClipIDList.Add(searchedClipIDS[i]);
            }
        }

        if (replacableClipIDList.Count == 0)
        {
            progressDebugString = $"you try to update motion name [{importedClip.name}] but there is no file to update in artresources";
            isSuccess = false;
            goto LogAndReturn;
        }

        string oldClipFilePath = string.Empty;

        if (replacableClipIDList.Count >= 2)
        {
            oldClipFilePath = EditorUtility.OpenFolderPanel("Duplicated animation clip name more then two detected, Please select clip for update", ART_RESOURCES_PATH, "ANI_clip");
            if (oldClipFilePath == string.Empty)
            {
                progressDebugString = "File path is not selected, you must select specific file path for udpate";
                isSuccess = false;
                goto LogAndReturn;
            }
        }
        else
        {
            oldClipFilePath = AssetDatabase.GUIDToAssetPath(replacableClipIDList[0]);
        }

        progressDebugString += $"Old animation clip file path successfully catched [{oldClipFilePath}] \n";

        //Catch Events and loop type in old data
        AnimationClip prevClip = AssetDatabase.LoadAssetAtPath<AnimationClip>(oldClipFilePath);
        AnimationEvent[] prevEvents = prevClip.events;
        bool prevLoopType = AnimationUtility.GetAnimationClipSettings(prevClip).loopTime;

        progressDebugString += $"Events is catched Count: [{ prevEvents.Length}]\n";
        progressDebugString += $"loop time check box catached [{prevLoopType}]\n";

        //Create clone of subasset animation clip in fbx file
        var clone = Object.Instantiate(importedClip);
        clone.name = prevClip.name;
        //Update old events and loop type into new updated animation clip
        AnimationClipSettings clipSetting = AnimationUtility.GetAnimationClipSettings(clone);
        clipSetting.loopTime = prevLoopType;
        AnimationUtility.SetAnimationClipSettings(clone, clipSetting);
        AnimationUtility.SetAnimationEvents(clone, prevEvents);
        //Save updated animation clip into old clip file path

        EditorUtility.CopySerialized(clone, prevClip);
        EditorApplication.delayCall += () =>
        {
            AssetDatabase.SaveAssets();
        };
    //Log
    LogAndReturn:
        if (isSuccess)
        {
            progressDebugString += "Now Animation Update is successfully done";
            Debug.Log(progressDebugString);
        }
        else
        {
            Debug.LogError(progressDebugString);
        }
    }

    private bool FBXFileChecking(string fbxPath, ref string message)
    {
        return checkMotionUpdateFBXFileValid(fbxPath, ref message);
    }

    private bool checkMotionUpdateFBXFileValid(string fbxPath, ref string infoMessage)
    {
        infoMessage = "";
        Object fbxFile = AssetDatabase.LoadAssetAtPath<Object>(fbxPath);
        if (fbxFile == null)
        {
            infoMessage = "not valid filepath";
            return false;
        }

        Object[] subassets = AssetDatabase.LoadAllAssetsAtPath(fbxPath);

        for (int i = 0; i < subassets.Length; i++)
        {
            if (subassets[i].name.Contains("preview"))
            {
                continue;
            }

            if (subassets[i].name.Contains("ANI_"))
            {
                infoMessage = $"valid file path: {subassets[i].name} founded";
                return true;
            }
        }

        infoMessage = "file path is valid but there is no animation for update, 애니메이션 이름의 시작이 [ANI_] 인지 확인해주세요";
        return false;
    }


}