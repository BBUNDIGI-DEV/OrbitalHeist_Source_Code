using System.Collections;
using UnityEngine;
using System;

    public class AutoDeactiveTimer : MonoBehaviour
    {
        private WaitForSeconds mWaitingTime;
        private Action mOnTimerEnd;

        public void SetWaitingTime(float time)
        {
            mWaitingTime = new WaitForSeconds(time);
        }

        public void SetOnTimerEndCallback(Action onTimerEnd)
        {
            if (mOnTimerEnd == null)
            {
                mOnTimerEnd = onTimerEnd;
            }
            else
            {
                mOnTimerEnd += onTimerEnd;
            }
        }

        public void StartTimer()
        {
            gameObject.SetActive(true);
            StartCoroutine(timerRoutine());
        }

        private IEnumerator timerRoutine()
        {
            yield return mWaitingTime;
            gameObject.SetActive(false);
            if (mOnTimerEnd != null)
            {
                mOnTimerEnd.Invoke();
            }
        }
    }
