using Cinemachine;
using FMOD.Studio;
using UnityEngine;

public abstract class CameraActorBase
{
    public bool mIsActivate
    {
        get
        {
            return mActingTimer.IsActivate;
        }
    }

    protected readonly CinemachineBrain mCinemachineBrain;

    protected CameraActingEventData mActingData;
    protected CinemachineVirtualCamera mActiveVCam;
    protected float mDelay;

    private static int msInstanceCount;
    private readonly BasicTimer mActingTimer;

    public CameraActorBase(CinemachineBrain cineBrain)
    {
        msInstanceCount++;
        mCinemachineBrain = cineBrain;
        mActingTimer = GlobalTimer.Instance.AddBasicTimer(cineBrain.gameObject, $"CameraActing {msInstanceCount}", eTimerUpdateMode.Update, 0.0f, onStartActing, onEndActing, updateActing);
    }

    protected void playActing(float duration)
    {
        tryUpdateVCAM();
        if (mActingTimer.IsActivate)
        {
            mActingTimer.StopTimer(false);
        }

        mActingTimer.ChangeDuration(duration);
        mActingTimer.StartTimer();
    }

    protected virtual void onStartActing()
    {
        //PlaceHolder
    }

    protected abstract void updateActing(float normalizedTime, float timePass);

    protected abstract void onEndActing();

    protected void tryUpdateVCAM()
    {
        if (mActiveVCam != null && mCinemachineBrain.IsLive(mActiveVCam))
        {
            return;
        }

        Debug.Assert(mCinemachineBrain.ActiveVirtualCamera != null);
        mActiveVCam = mCinemachineBrain.ActiveVirtualCamera as CinemachineVirtualCamera;
    }
}

