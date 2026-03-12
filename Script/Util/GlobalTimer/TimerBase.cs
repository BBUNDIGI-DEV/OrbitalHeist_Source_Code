using UnityEngine;
using System;

    public abstract class TimerBase
    {
        public delegate void TimerUpdateCallback(float normalizedTime, float currentTime);

        public ObservedData<float> CurrentTime;
        public ObservedData<float> NormalizedTime;

        public bool IsTimeOut
        {
            get
            {

                return mCurrentDuration <= 0.0f;
            }
        }

        public bool IsActivate
        {
            get; protected set;
        }

        public string InstanceNameID
        {
            get; private set;
        }

        public string NameID
        {
            get; private set;
        }

        public int InstanceID
        {
            get; private set;
        }

        public eTimerUpdateMode UpdateMode
        {
            get; private set;
        }

        protected float mDuration
        {
            get
            {
                return _mDuration;
            }
            set
            {
                _mDuration = value;
            }
        }
        protected float mCurrentDuration
        {
            set
            {
                _mCurrentDuration = value;
                CurrentTime.Value = _mCurrentDuration;
                NormalizedTime.Value = mCurrentDuration / mDuration;
            }
            get
            {
                return _mCurrentDuration;
            }
        }

        protected Action mOnTimerStartOrNull;
        protected TimerUpdateCallback mOnTimerUpdateOrNull;
        protected Action mOnTimerEndOrNull;
        protected bool mIsPaused;
        private float _mDuration;
        private float _mCurrentDuration;

        public TimerBase(string nameID, eTimerUpdateMode updateMode, int instanceID,
            float duration = 0.0f, Action onTimerStart = null, Action onTimerEnd = null, TimerUpdateCallback onTimerUpdate = null)
        {
            mDuration = duration;

            UpdateMode = updateMode;
            NameID = nameID;
            InstanceID = instanceID;
            InstanceNameID = nameID + instanceID;

            mOnTimerStartOrNull = onTimerStart;
            mOnTimerUpdateOrNull = onTimerUpdate;
            mOnTimerEndOrNull = onTimerEnd;
        }

        public virtual void CalculateTimePass(float deltaTime)
        {
            if (!IsActivate)
            {
                return;
            }

            if(mIsPaused)
            {
                return;
            }
            mCurrentDuration += deltaTime;
            mOnTimerUpdateOrNull?.Invoke(NormalizedTime, mCurrentDuration);

            if (mCurrentDuration > mDuration)
            {
                StopTimer(true);
                return;
            }
        }

        public virtual void StartTimer()
        {
            mCurrentDuration = 0.0f;
            IsActivate = true;
            mOnTimerStartOrNull?.Invoke();
        }

        public virtual void PauseTimer()
        {

        }

        public virtual void StopTimer(bool invokeCallback)
        {
            mCurrentDuration = 0;
            IsActivate = false;
            if (invokeCallback)
            {
                mOnTimerEndOrNull?.Invoke();
            }
        }

        public virtual void Restart(bool needInvokeCallback)
        {
            StopTimer(needInvokeCallback);
            StartTimer();
        }

        public virtual TimerBase ChangeUpdateMode(eTimerUpdateMode updateMode)
        {
            this.UpdateMode = updateMode;
            return this;
        }

        public virtual TimerBase ChangeDuration(float newDuration)
        {
            mDuration = newDuration;
            return this;
        }

        public virtual TimerBase ChangeTimerStartCallback(Action onTimerStart)
        {
            mOnTimerStartOrNull = onTimerStart;
            return this;
        }

        public virtual TimerBase ChangeTimerUpdateCallback(TimerUpdateCallback onTimerUpdateOrNull, bool replaceCallback = true)
        {
            if(replaceCallback)
            {
                mOnTimerUpdateOrNull = onTimerUpdateOrNull;
            }
            else
            {
                mOnTimerUpdateOrNull += onTimerUpdateOrNull;
            }
            return this;
        }

        public virtual TimerBase ChangeTimerEndCallback(Action onTimerEnd)
        {
            mOnTimerEndOrNull = onTimerEnd;
            return this;
        }
    }


    public enum eTimerUpdateMode
    {
        Update,
        FixedUpdate,
        LateUpdate,
        UnscaledUpdate,
        UnscaledLateUpdate
    }
