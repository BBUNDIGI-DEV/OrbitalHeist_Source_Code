using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class FXPostProcessingHelper : MonoBehaviour
{
    [SerializeField] private Volume M_CHROMATIC_ABERRATION;
    [SerializeField] private Volume M_BRIGHT;
    [SerializeField] private Volume M_RADIAL;
    [SerializeField] private Material M_CUSTOM_PP_MAT;

    private Dictionary<ePostProcessingType, PostProcessingFXTimer> mPPOnOffTimerDic;
    public void Awake()
    {
        mPPOnOffTimerDic = new Dictionary<ePostProcessingType, PostProcessingFXTimer>(16);
    }

    public void OnDestroy()
    {
        if(GlobalTimer.IsExist)
        {
            GlobalTimer.Instance.RemoveAllTimerByInstance(gameObject.GetInstanceID());
        }
    }

    public void InvokePP(ePostProcessingType ppType, PostProcessingInvokingConfig config)
    {
        if(!mPPOnOffTimerDic.ContainsKey(ppType))
        {
            BasicTimer ppTimer = GlobalTimer.Instance.AddBasicTimer(gameObject, ppType.ToString(), eTimerUpdateMode.Update);
            PostProcessingFXTimer fxTimer = null;

            switch (ppType)
            {
                case ePostProcessingType.ChromaticAberration:
                    fxTimer = new PostProcessingFXTimer(ppTimer, M_CHROMATIC_ABERRATION);
                    break;
                case ePostProcessingType.EdgeGrayscale:
                    fxTimer = new PostProcessingFXTimer(ppTimer, M_CUSTOM_PP_MAT, "_GrayScale");
                    break;
                case ePostProcessingType.Bright:
                    fxTimer = new PostProcessingFXTimer(ppTimer, M_BRIGHT);
                    break;
                case ePostProcessingType.RadialBlur:
                    fxTimer = new PostProcessingFXTimer(ppTimer, M_RADIAL);
                    break;
                default:
                    Debug.LogError($"Defualt Switch detected [{ppType}]", this);
                    break;
            }

            mPPOnOffTimerDic.Add(ppType, fxTimer);
        }

        mPPOnOffTimerDic[ppType].PlayPostProcessingFX(config);
    }
}

public class PostProcessingFXTimer
{
    private Volume mPPVolume;
    private Material mPPMat;
    private string mMatParamName;

    private BasicTimer mTimer;
    private PostProcessingInvokingConfig mConfig;

    public PostProcessingFXTimer(BasicTimer basicTimer, Volume ppVolume)
    {
        mTimer = basicTimer;
        mPPVolume = ppVolume;
    }

    public PostProcessingFXTimer(BasicTimer ppTimer, Material ppMat, string parameter)
    {
        mTimer = ppTimer;
        mPPMat = ppMat;
        mMatParamName = parameter;
        ppTimer.ChangeTimerUpdateCallback(updatePP);
    }

    public void PlayPostProcessingFX(PostProcessingInvokingConfig config)
    {
        if(mTimer.IsActivate)
        {
            mTimer.StopTimer(true);
        }

        mTimer.ChangeDuration(config.Duration)
            .ChangeTimerUpdateCallback(updatePP)
            .ChangeTimerEndCallback(onPPEnd)
            .StartTimer();
        mConfig = config;
    }

    private void updatePP(float normalizeTime, float deltaTime)
    {
        float factor = 0.0f;

        if(mConfig.SettingType == eVolumnWeightSettingType.Constant)
        {
            factor = mConfig.Power;
        }
        else
        {
            factor = mConfig.PowerCurve.Evaluate(normalizeTime) * mConfig.Power;
        }

        factor = Mathf.Clamp01(factor);

        if (mPPVolume != null)
        {
            mPPVolume.weight = factor;
        }
        

        if(mPPMat != null)
        {
            mPPMat.SetFloat(mMatParamName, factor);
        }
    }

    private void onPPEnd()
    {
        if (mPPVolume != null)
        {
            mPPVolume.weight = 0.0f;
        }


        if (mPPMat != null)
        {
            mPPMat.SetFloat(mMatParamName, 0.0f);
        }
    }
}


