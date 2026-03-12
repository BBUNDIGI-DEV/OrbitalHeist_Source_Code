using UnityEngine;
using Cinemachine;


public class CameraZoomAndOutHelper : CameraActorBase
{
    private readonly float DEFAULT_SIZE;

    private AnimationCurve mZoomCurve;
    public CameraZoomAndOutHelper(CinemachineBrain owner) : base(owner)
    {
        tryUpdateVCAM();
        DEFAULT_SIZE = mActiveVCam.m_Lens.FieldOfView;
    }

    public void PlayZoomInZoomOut(AnimationCurve zoomCurve, float duration)
    {
        mZoomCurve = zoomCurve;
        playActing(duration);
    }

    protected override void updateActing(float normalizedTime, float timePass)
    {
        float curveValue = mZoomCurve.Evaluate(normalizedTime);
        if (curveValue < 0.005f)
        {
            return;
        }

        float sizeMultiplier = Mathf.Abs(curveValue);
        float newSize = DEFAULT_SIZE * sizeMultiplier;
        mActiveVCam.m_Lens.FieldOfView = newSize;
    }

    protected override void onEndActing()
    {
        mActiveVCam.m_Lens.OrthographicSize = DEFAULT_SIZE;
    }
}