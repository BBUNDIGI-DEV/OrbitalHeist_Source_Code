using UnityEngine;

namespace UnityEditor
{
    public static class AssetDatabaseUtils
    {
        public static string[] GetScenesAssetPath()
        {
            string[] sceneAssets = AssetDatabase.FindAssets("scene_ a:assets t:scene", new[] { "Assets/Scenes/PlayScene", "Assets/Scenes/SharingScene" });
            for (int i = 0; i < sceneAssets.Length; i++)
            {
                string assetPath = AssetDatabase.GUIDToAssetPath(sceneAssets[i]);
                SceneAsset sceneAsset = AssetDatabase.LoadAssetAtPath<SceneAsset>(assetPath);
                sceneAssets[i] = assetPath;
            }

            return sceneAssets;
        }

        public static string GetSceneAssetPathBySceneNameEnum(string sceneName)
        {
            string[] scenePath = GetScenesAssetPath();

            for (int i = 0; i < scenePath.Length; i++)
            {
                if (scenePath[i].Contains(sceneName.ToString()))
                {
                    return scenePath[i];
                }
            }

            Debug.LogAssertion("Scene path not founded");
            return null;
        }
    }
}
