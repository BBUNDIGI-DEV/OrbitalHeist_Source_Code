using System;

    public class BasicTimer : TimerBase
    {
        public BasicTimer(string nameID, eTimerUpdateMode updateMode,  int instanceID,
            float duration = 0.0f, Action onTimerStart = null, Action onTimerEnd = null, TimerUpdateCallback onTimerUpdate = null) 
            : base(nameID, updateMode, instanceID, duration, onTimerStart, onTimerEnd, onTimerUpdate)
        {

        }
    }
