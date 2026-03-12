using System;
    public sealed class DelayTimer : TimerBase
    {
        float _delayDuration;
        float _currentDeleyTime;

        public DelayTimer(string nameID, eTimerUpdateMode updateMode, int instanceID, float duration = 0.0f, float delayDuration = 0.0f, 
            Action onTimerStart = null, Action onTimerEnd = null, TimerUpdateCallback onTimerUpdate = null) 
            : base(nameID, updateMode, instanceID, duration, onTimerStart, onTimerEnd, onTimerUpdate)
        {
            _delayDuration = delayDuration;
        }

        public override void CalculateTimePass(float deltaTime)
        {
            if (!IsActivate)
            {
                return;
            }

            if (mIsPaused)
            {
                return;
            }

            if (_currentDeleyTime < mCurrentDuration)
            {
                _currentDeleyTime += deltaTime;
                if (_currentDeleyTime >= mCurrentDuration)
                {
                    _currentDeleyTime = float.MaxValue;
                    mOnTimerStartOrNull?.Invoke();
                }
                return;
            }

            base.CalculateTimePass(deltaTime);
        }

        public override void StartTimer()
        {
            mCurrentDuration = 0.0f;
            _currentDeleyTime = 0.0f;
            IsActivate = true;
        }

        public DelayTimer ChangeDelayTime(float newDelayTime)
        {
            _delayDuration = newDelayTime;
            return this;
        }
    }
