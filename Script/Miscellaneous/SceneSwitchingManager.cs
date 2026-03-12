using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class SceneSwitchingManager : SingletonClass<SceneSwitchingManager>
{
    public ObservedData<eSceneName> OnSceneSwitchingStart;
    public ObservedData<eSceneName> CurrentScene;
    [SerializeField] private OnScreenFadingHelper sfScreenFadingHelper;
    private bool mIsOnLoading;

    protected override void Awake()
    {
        base.Awake();
        CurrentScene.Value = eSceneName.None;
    }

    protected void Start()
    {
        tryUpdateCurrentScene();
        if (CurrentScene != eSceneName.None)
        {
            return;
        }


        if(tryCheckStageSceneExist())
        {
            return;
        }


        LoadOtherScene(eSceneName.Scene_Title);

    }

    private void Update()
    {
        if(CurrentScene == eSceneName.None)
        {
            tryUpdateCurrentScene();
        }
        else
        {
            enabled = false;
        }
    }


    public void LoadOtherScene(eSceneName newScene, bool withFadingInOut = false, bool reloadAll = false)
    {
        if (newScene == eSceneName.Scene_Title)
        {
            if(PlayerManager.sPreOwnedAbilNameIDs != null)
            {
                PlayerManager.sPreOwnedAbilNameIDs.Clear();
            }
        }
        if(mIsOnLoading)
        {
            return;
        }

        StartCoroutine(LoadSceneRoutine(newScene, withFadingInOut, reloadAll));
    }

    public void ReloadScene()
    {

    }

    private void tryUpdateCurrentScene()
    {
        for (int i = 0; i < (int)eSceneName.Count; i++)
        {
            eSceneName checkSceneName = (eSceneName)i;

            if (SceneManager.GetSceneByName(checkSceneName.ToString()).isLoaded)
            {
                OnSceneSwitchingStart.Value = checkSceneName;
                CurrentScene.Value = checkSceneName;
                return;
            }
        }
    }

    private bool tryCheckStageSceneExist()
    {

        for (int i = 0; i < SceneManager.sceneCount; i++)
        {
            Scene loadedScene = SceneManager.GetSceneAt(i);
            if (loadedScene.name.Contains("Stage"))
            {
                return true;
            }

        }
        return false;
    }

    private IEnumerator LoadSceneRoutine(eSceneName newScene, bool withFadingInOut = false, bool reloadAll = false)
    {
        Debug.Assert(newScene != eSceneName.None,
            "You Cannot load None Scene this is for checking state");
        Debug.Assert(newScene != eSceneName.Count,
            "You Cannot load count scene this is for count list of enum");
        mIsOnLoading = true;
        OnSceneSwitchingStart.Value = newScene;
        if (withFadingInOut)
        {
            sfScreenFadingHelper.PlayOnScreenFadeIn();
            yield return new WaitUntil(() => !sfScreenFadingHelper.IsFadeInPlaying);
        }

        if (reloadAll)
        {
            foreach (eAutoLoadedSceneName sceneName in GetAutoLoadingScene(newScene))
            {
                if (SceneManager.GetSceneByName(sceneName.ToString()).isLoaded)
                {
                    AsyncOperation unloadAdditiveSceneAsync = SceneManager.UnloadSceneAsync(sceneName.ToString());
                }
            }
            GlobalTimer.Instance.ClearAllTimer();
        }

        if (CurrentScene != eSceneName.None)
        {
            AsyncOperation unloadAsync = SceneManager.UnloadSceneAsync(CurrentScene.ToString());
            yield return new WaitUntil(() => unloadAsync.isDone);
        }

        yield return sceneLoadingRoutine(newScene);
        CurrentScene.Value = newScene;

        if (withFadingInOut)
        {
            sfScreenFadingHelper.PlayOnScreenFadeOut();
            yield return new WaitUntil(() => !sfScreenFadingHelper.IsFadeInPlaying);
        }
        mIsOnLoading = false;
    }

    private IEnumerator sceneLoadingRoutine(eSceneName newScene)
    {
        foreach (eAutoLoadedSceneName sceneName in GetAutoUnLoadingScene(newScene))
        {
            if (SceneManager.GetSceneByName(sceneName.ToString()).isLoaded)
            {
                Debug.Log(sceneName);
                AsyncOperation unloadAsync = SceneManager.UnloadSceneAsync(sceneName.ToString(), UnloadSceneOptions.UnloadAllEmbeddedSceneObjects);
                yield return new WaitUntil(() => unloadAsync.isDone);

            }
        }

        SceneManager.LoadScene(newScene.ToString(), LoadSceneMode.Additive);

        foreach (eAutoLoadedSceneName sceneName in GetAutoLoadingScene(newScene))
        {
            if (!SceneManager.GetSceneByName(sceneName.ToString()).isLoaded)
            {
                SceneManager.LoadScene(sceneName.ToString(), LoadSceneMode.Additive);
            }
        }

    }

    private IEnumerable<eAutoLoadedSceneName> GetAutoLoadingScene(eSceneName sceneName)
    {
        if(sceneName.CheckIsLevelScene())
        {
            yield return eAutoLoadedSceneName.Scene_GlobalUI;
            yield return eAutoLoadedSceneName.Scene_Player;
        }
    }

    private IEnumerable<eAutoLoadedSceneName> GetAutoUnLoadingScene(eSceneName sceneName)
    {
        if (!sceneName.CheckIsLevelScene())
        {
            yield return eAutoLoadedSceneName.Scene_GlobalUI;
            yield return eAutoLoadedSceneName.Scene_Player;
        }
    }
}

public enum eSceneName
{
    None = -1,
    Scene_Title,
    Scene_Stage1,
    Scene_Stage2,
    Scene_Stage3,
    Scene_StoryCutscene,
    Scene_EndingStoryCutscene,
    Scene_Credit,
    Count,
}

public enum eAutoLoadedSceneName
{
    Scene_GlobalManagers,
    Scene_GlobalUI,
    Scene_Player,
}