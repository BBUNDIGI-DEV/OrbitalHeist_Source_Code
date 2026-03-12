using FMODUnity;
using FMOD.Studio;
using UnityEngine;

public class BGMManager : SingletonClass<BGMManager>
{
    private EventInstance mCurrentBGMInstance;

    private EventInstance mTitleSceneInstance;
    private EventInstance mBattleSceneInstance;
    private EventInstance mAMBSoundInstance;
    private eSceneName mPrevScene = eSceneName.None;

    protected override void Awake()
    {
        base.Awake();
        mTitleSceneInstance = RuntimeManager.CreateInstance("event:/BGM/BGM_Title");
        mBattleSceneInstance = RuntimeManager.CreateInstance("event:/BGM/BGM_Main");
        mAMBSoundInstance = RuntimeManager.CreateInstance("event:/AMB/AMB_DessertWind");
    }

    private void Start()
    {
        SceneSwitchingManager.Instance.OnSceneSwitchingStart.AddListener(tryReleaseBGM, true);
        SceneSwitchingManager.Instance.CurrentScene.AddListener(onSceneChange, true);
    }

    public void SetBGMLabel(string label)
    {
        mBattleSceneInstance.setParameterByNameWithLabel("Battle", label);
    }

    private void tryReleaseBGM(eSceneName newScene)
    {
        if (newScene == mPrevScene)
        {
            return;
        }
        EventInstance prevBGMInstance = mCurrentBGMInstance;

        switch (mPrevScene)
        {
            case eSceneName.Scene_StoryCutscene:
            case eSceneName.Scene_EndingStoryCutscene:
            case eSceneName.Scene_Credit:
            case eSceneName.Scene_Title:
            case eSceneName.Scene_Stage3:
                mAMBSoundInstance.getPlaybackState(out PLAYBACK_STATE ps);
                if (ps == PLAYBACK_STATE.PLAYING)
                {
                    mAMBSoundInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
                }
                break;
        }

        switch (newScene)
        {
            case eSceneName.Scene_Title:
            case eSceneName.Scene_StoryCutscene:
            case eSceneName.Scene_EndingStoryCutscene:
            case eSceneName.Scene_Credit:
                mCurrentBGMInstance = mTitleSceneInstance;
                break;
            case eSceneName.Scene_Stage1:
            case eSceneName.Scene_Stage2:
            case eSceneName.Scene_Stage3:
                mCurrentBGMInstance = mBattleSceneInstance;
                break;
            default:
                Debug.Assert(false);
                break;
        }

        if (prevBGMInstance.isValid() && !prevBGMInstance.Equals(mCurrentBGMInstance))
        {
            prevBGMInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
            return;
        }
    }

    private void onSceneChange(eSceneName newScene)
    {
        if (newScene == mPrevScene)
        {
            return;
        }

        switch (newScene)
        {
            case eSceneName.Scene_Stage1:
            case eSceneName.Scene_Stage2:
                mAMBSoundInstance.getPlaybackState(out PLAYBACK_STATE playbackState);
                if (playbackState == PLAYBACK_STATE.STOPPED)
                {
                    mAMBSoundInstance.start();
                }
                break;
        }

        mCurrentBGMInstance.getPlaybackState(out PLAYBACK_STATE ps);
        if (mCurrentBGMInstance.isValid() && ps == PLAYBACK_STATE.STOPPED)
        {
            mCurrentBGMInstance.start();
        }

        mPrevScene = newScene;
    }

    public void SetMainBGMAsNormal()
    {
        mCurrentBGMInstance.setParameterByName("IsBattle", 0.0f);
        mCurrentBGMInstance.setParameterByName("IsBossBattle", 0.0f);
    }

    public void SetMainBGMAsBattle()
    {
        mCurrentBGMInstance.setParameterByName("IsBattle", 1.0f);
    }

    public void SetMainBGMAsBoss()
    {
        mCurrentBGMInstance.setParameterByName("IsBossBattle", 1.0f);
    }

}
