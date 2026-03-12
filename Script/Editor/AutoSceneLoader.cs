#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;

[InitializeOnLoad]
public static class AutoSceneLoader
{
    static AutoSceneLoader()
    {
        EditorSceneManager.activeSceneChangedInEditMode += autoSceneLoading;
    }

    private static void autoSceneLoading(Scene prevScene, Scene newScene)
    {
        if (BuildPipeline.isBuildingPlayer)
        {
            return;
        }
        autoSceneLoading(newScene.name);
    }

    private static void autoSceneLoading(string sceneName)
    {
        string globalScenePath = AssetDatabaseUtils.GetSceneAssetPathBySceneNameEnum(eAutoLoadedSceneName.Scene_GlobalManagers.ToString());
        Scene globalScene = EditorSceneManager.OpenScene(globalScenePath, OpenSceneMode.Additive);

        if (sceneName.Contains("Stage"))
        {
            EditorSceneManager.OpenScene(globalScenePath, OpenSceneMode.Additive);

            string globalUIScenePath = AssetDatabaseUtils.GetSceneAssetPathBySceneNameEnum(eAutoLoadedSceneName.Scene_GlobalUI.ToString());
            EditorSceneManager.OpenScene(globalUIScenePath, OpenSceneMode.Additive);

            string playerScenePath = AssetDatabaseUtils.GetSceneAssetPathBySceneNameEnum(eAutoLoadedSceneName.Scene_Player.ToString());
            EditorSceneManager.OpenScene(playerScenePath, OpenSceneMode.Additive);
        }

        SceneManager.SetActiveScene(globalScene);
    }
}
#endif
