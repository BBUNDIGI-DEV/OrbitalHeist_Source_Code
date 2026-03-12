using UnityEngine;
using Cinemachine;

public sealed class CameraShaker : CameraActorBase
{
    private CinemachineBasicMultiChannelPerlin mVcamNoiseActor;
    private AnimationCurve mShakeCurve;
    private float mShakePower;
    private float mFrequencyPower;
    public CameraShaker(CinemachineBrain owner) : base(owner)
    {
        tryUpdateVCAM();
        mVcamNoiseActor = mActiveVCam.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
    }

    public void PlayShake(float shakeDuration, float shakePower, float shakeFrequency, AnimationCurve shakeCurve)
    {
        mShakePower = shakePower;
        mFrequencyPower = shakeFrequency;
        mShakeCurve = shakeCurve;
        playActing(shakeDuration);
        mVcamNoiseActor.m_FrequencyGain = mFrequencyPower;
    }

    protected override void onStartActing()
    {
        mVcamNoiseActor.m_AmplitudeGain = 0.0f;
    }
    
    protected override void updateActing(float normalizedTime, float timePass)
    {
        float curveValue = mShakeCurve.Evaluate(normalizedTime);
        curveValue = Mathf.Abs(curveValue);
        mVcamNoiseActor.m_AmplitudeGain = curveValue * mShakePower;
    }

    protected override void onEndActing()
    {
        mVcamNoiseActor.m_FrequencyGain = 0.0f;
        mVcamNoiseActor.m_AmplitudeGain = 0.0f;
    }
}