using System;
using System.Collections.Generic;
using UnityEngine;

    /// <summary>
    /// 연속된 콜백을 한 타이머에서 사용 가능
    /// </summary>
    public sealed class ChainedTimer : TimerBase
    {
        private struct ChainedTimerCallback
        {
            public string Tag;
            public float Duration;
            public Action Callback;

            public ChainedTimerCallback(string tag, float duration, Action callback)
            {
                Tag = tag;
                Duration = duration;
                Callback = callback;
            }
        }

        List<ChainedTimerCallback> mCallbackList;
        int mCurrentIndex;

        public ChainedTimer(string nameID, eTimerUpdateMode updateMode, int instanceID
            , Action onTimerStart = null, Action onTimerEnd = null, TimerUpdateCallback onTimerUpdate = null)
            : base(nameID, updateMode, instanceID, 0.0f, onTimerStart, onTimerEnd, onTimerUpdate)
        {
            mCallbackList = new List<ChainedTimerCallback>();
        }

        public override void CalculateTimePass(float deltaTime)
        {
            base.CalculateTimePass(deltaTime);
            if (!IsActivate)
            {
                return;
            }

            if(mIsPaused)
            {
                return;
            }

            if (mCurrentDuration >= mCallbackList[mCurrentIndex].Duration)
            {
                mCallbackList[mCurrentIndex].Callback?.Invoke();
                mCurrentIndex++;
            }
        }

        public override void StartTimer()
        {
            Debug.Assert(mCallbackList.Count != 0, "Empty Callback list checked in chainedTimer");
            base.StartTimer();
            mCurrentIndex = 0;
        }

        public override void StopTimer(bool invokeCallback)
        {
            base.StopTimer(invokeCallback);
            if (invokeCallback)
            {
                for (int i = mCurrentIndex; i < mCallbackList.Count; i++)
                {
                    mCallbackList[i].Callback?.Invoke();
                }
            }
        }

        public bool IsTaggedTimerActivated(string callbackTag)
        {
            return IsActivate && mCallbackList[mCurrentIndex].Tag == callbackTag;
        }

        public void SkipTaggedTimer(string tag, bool withCallback = true)
        {
            Debug.Assert(IsTaggedTimerActivated(tag), $"you cannot skip that tagged  timer[{tag}]");
            if(withCallback)
            {
                mCallbackList[mCurrentIndex].Callback?.Invoke();
                mCurrentDuration = mCallbackList[mCurrentIndex].Duration;
                mCurrentIndex++;
            }
        }

        public ChainedTimer AddCallback(string callbackTag, float spanDuration, Action callback)
        {
            Debug.Assert(!containTag(callbackTag), $"Duplicate tagged callback data detected [{callbackTag}]");
            if(mCallbackList.Count != 0)
            {
                spanDuration += mCallbackList[mCallbackList.Count - 1].Duration;
            }
            mCallbackList.Add(new ChainedTimerCallback(callbackTag, spanDuration, callback));
            mDuration = spanDuration;
            return this;
        }

        public ChainedTimer RemoveCallback(string callbackTag)
        {
            int index = findIndex(callbackTag);
            Debug.Assert(index != -1, $"Cannot found tagged callback data [{callbackTag}]");

            if(index == mCallbackList.Count)
            {
                if(mCallbackList.Count == 1)
                {
                    mDuration = 0.0f;
                }
                else
                {
                    mDuration = mCallbackList[mCallbackList.Count - 1].Duration;
                }
            }

            mCallbackList.RemoveAt(index);
            return this;
        }

        public ChainedTimer ChangeCallback(string callbackTag, Action newCallback)
        {
            int index = findIndex(callbackTag);
            Debug.Assert(index != -1, $"Cannot found tagged callback data [{callbackTag}]");

            ChainedTimerCallback callbackData = mCallbackList[index];
            callbackData.Callback = newCallback;
            mCallbackList[index] = callbackData;
            return this;
        }

        public ChainedTimer ChangeTiming(string callbackTag, float newDuration)
        {
            int index = findIndex(callbackTag);
            Debug.Assert(index != -1, $"Cannot found tagged callback data [{callbackTag}]");

            float durationOffset = newDuration - calculateSperateDuration(index);

            for (int i = index; i < mCallbackList.Count; i++)
            {
                ChainedTimerCallback callbackData = mCallbackList[i];
                callbackData.Duration += durationOffset;
                mCallbackList[i] = callbackData;
            }
            mDuration = mCallbackList[mCallbackList.Count - 1].Duration;
            return this;
        }

        public ChainedTimer ClearAllTiming()
        {
            mCallbackList.Clear();
            return this;
        }

        public bool HasKey(string tag)
        {
            return findIndex(tag) != -1;
        }

        private float calculateSperateDuration(int index)
        {
            Debug.Assert(index >= 0 && index < mCallbackList.Count);

            float subsetDuration = mCallbackList[index].Duration;
            if(index != 0)
            {
                subsetDuration -= mCallbackList[index - 1].Duration;
            }
            return subsetDuration;
        }

        private bool containTag(string tag)
        {
            return findIndex(tag) != -1;
        }

        private int findIndex(string tag)
        {
            for (int i = 0; i < mCallbackList.Count; i++)
            {
                if (mCallbackList[i].Tag == tag)
                {
                    return i;
                }
            }
            return -1;
        }
    }
