using UnityEngine;

    public static class TimeUtils
    {
        private static BulletTimeHelper mHelper
        {
            get
            {
                if (_Helper == null)
                {
                    _Helper = new BulletTimeHelper();
                }
                return _Helper;
            }
        }

        private static BulletTimeHelper _Helper;

        public static void PlayBulletTime(BulletTimeData inputData)
        {
            mHelper.PlayBulletTime(inputData);
        }
    }

    public class BulletTimeHelper
    {
        private readonly float mDefaultTimeScale;
        private readonly float mDefaultFixedTimeScale;
        private BulletTimeData mCurrentUsedData;

        private BasicTimer mBulletTimeTimer;

        public BulletTimeHelper()
        {
            mDefaultTimeScale = 1.0f;
            mDefaultFixedTimeScale = Time.fixedDeltaTime;
            mBulletTimeTimer = GlobalTimer.Instance.AddBasicTimer(null,"BulletTime", eTimerUpdateMode.UnscaledLateUpdate);

            mBulletTimeTimer
                .ChangeTimerEndCallback(endBulletTime)
                .ChangeTimerUpdateCallback(updateBulletTime);
        }

        public void PlayBulletTime(BulletTimeData data)
        {
            mCurrentUsedData = data;
            mBulletTimeTimer.ChangeDuration(mCurrentUsedData.BulletDuration).StartTimer();
        }

        private void updateBulletTime(float normalizedTime, float timePass)
        {
            float bulletTimeFactor = mCurrentUsedData.BulletTimeCurve.Evaluate(normalizedTime);
            bulletTimeFactor = Mathf.Clamp(bulletTimeFactor, 0.0f, 1.0f);
            TimeScaleUtil.Instance.AddTimeScale("BulletTime", new PriorityAndTimeScalePair(eTimeScaleTrigger.BulletTime, mDefaultTimeScale * bulletTimeFactor));
            TimeScaleUtil.Instance.AddFixedTimeScale("BulletTime", new PriorityAndTimeScalePair(eTimeScaleTrigger.BulletTime, mDefaultFixedTimeScale * bulletTimeFactor));
        }

        private void endBulletTime()
        {
            TimeScaleUtil.Instance.RemoveTimeScale("BulletTime");
            TimeScaleUtil.Instance.RemoveFixedTimeScale("BulletTime");
        }
    }

    [System.Serializable]
    public struct BulletTimeData
    {
        public AnimationCurve BulletTimeCurve;
        public float BulletDuration;

    }

