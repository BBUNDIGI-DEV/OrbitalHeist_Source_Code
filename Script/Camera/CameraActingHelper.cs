using Cinemachine;
using System.Collections;
using UnityEngine;

[RequireComponent(typeof(CinemachineBrain))]
public class CameraActingHelper : MonoBehaviour
{
    [SerializeField] private GameObject[] sfOnHealingFX;
    private Animator mOnScreenAnimator;
    private CameraShaker mCameraShakeHelper;
    private CameraZoomAndOutHelper mZoomAndOutHelper;
    private FXPostProcessingHelper mHelper;

    private IEnumerator Start()
    {
        CinemachineBrain cineBrain = GetComponent<CinemachineBrain>();
        mOnScreenAnimator = GetComponentInChildren<Animator>();
        yield return new WaitUntil(() => cineBrain.ActiveVirtualCamera != null);
        mCameraShakeHelper = new CameraShaker(cineBrain);
        mZoomAndOutHelper = new CameraZoomAndOutHelper(cineBrain);

        mHelper = GetComponent<FXPostProcessingHelper>();
    }

    public void ProcessCameraActing(CameraActingEventData data)
    {
        if (data.ActingType.HasFlag(eCameraActingType.Shake))
        {
            mCameraShakeHelper.PlayShake(data.ShakeDuration, data.ShakePower, data.ShakeFrequency, data.ShakeCurve);
        }
        if (data.ActingType.HasFlag(eCameraActingType.ZoomInAndOutCurve))
        {
            mZoomAndOutHelper.PlayZoomInZoomOut(data.ZoomInOutCurve, data.ZoomInAndOutDuration);
        }

        if (data.PostProcessingType.HasFlag(ePostProcessingType.ChromaticAberration))
        {
            mHelper.InvokePP(ePostProcessingType.ChromaticAberration, data.ChromaticAberrationInvokeSetting);
        }
        if (data.PostProcessingType.HasFlag(ePostProcessingType.Bright))
        {
            mHelper.InvokePP(ePostProcessingType.Bright, data.BrightingInvokeSetting);
        }
        if (data.PostProcessingType.HasFlag(ePostProcessingType.EdgeGrayscale))
        {
            mHelper.InvokePP(ePostProcessingType.EdgeGrayscale, data.EdgeGrayscaleInvokeSetting);
        }
        if (data.PostProcessingType.HasFlag(ePostProcessingType.RadialBlur))
        {
            mHelper.InvokePP(ePostProcessingType.RadialBlur, data.RadialBlurInvokeSetting);
        }

    }

    public void PlayOnScreenVFX(eOnScreenVFXType vfx)
    {
        switch (vfx)
        {
            case eOnScreenVFXType.OnHurt:
                mOnScreenAnimator.SetTrigger(vfx.ToString());
                break;
            case eOnScreenVFXType.OnHealing:
                for (int i = 0; i < sfOnHealingFX.Length; i++)
                {
                    if(!sfOnHealingFX[i].activeInHierarchy)
                    {
                        sfOnHealingFX[i].SetActive(true);
                        break;
                    }
                }
                break;
            default:
                break;
        }
    }
}
public enum eOnScreenVFXType
{
    OnHurt,
    OnHealing,
}