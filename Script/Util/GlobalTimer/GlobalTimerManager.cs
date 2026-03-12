using System.Collections.Generic;
using UnityEngine;
using System;
using DG.Tweening;

    public class GlobalTimer : SingletonClass<GlobalTimer>
    {
        private Dictionary<string, TimerBase> _TimerDic;
        private List<TimerBase> _bufferedAddedTimer;
        private List<string> _bufferedRemovedTimer;

        protected override void Awake()
        {
            base.Awake();
            _TimerDic = new Dictionary<string, TimerBase>();
            _bufferedAddedTimer = new List<TimerBase>(8);
            _bufferedRemovedTimer = new List<string>(8);
        }

        public void Init()
        {
            _TimerDic.Clear();
            _bufferedAddedTimer.Clear();
            _bufferedRemovedTimer.Clear();

            _TimerDic = new Dictionary<string, TimerBase>();
            _bufferedAddedTimer = new List<TimerBase>(8);
            _bufferedRemovedTimer = new List<string>(8);
        }

        private void Update()
        {
            DOTween.ManualUpdate(Time.unscaledDeltaTime, Time.unscaledDeltaTime);

            addAndRemoveBufferedTimers();
            processTimepass(eTimerUpdateMode.Update, Time.deltaTime);
            processTimepass(eTimerUpdateMode.UnscaledUpdate, Time.unscaledDeltaTime);

        }

        private void FixedUpdate()
        {
            addAndRemoveBufferedTimers();
            processTimepass(eTimerUpdateMode.FixedUpdate, Time.fixedDeltaTime);
        }

        private void LateUpdate()
        {
            addAndRemoveBufferedTimers();
            processTimepass(eTimerUpdateMode.LateUpdate, Time.deltaTime);
            processTimepass(eTimerUpdateMode.UnscaledLateUpdate, Time.unscaledDeltaTime);
        }

        public void AddTimer(TimerBase data)
        {
            Debug.Assert(!_TimerDic.ContainsKey(data.InstanceNameID), $"Duplicate timer insert detected [{data.InstanceNameID} aready in]");
            _bufferedAddedTimer.Add(data);
        }

        public BasicTimer AddBasicTimer(GameObject ownerOrNull, string key, eTimerUpdateMode updateMode
            ,float duration = 0.0f, Action onTimerStart = null, Action onTimerEnd = null, TimerBase.TimerUpdateCallback onTimerUpdate = null)
        {
            int instanceID = ownerOrNull == null ? 0 : ownerOrNull.GetInstanceID();
            BasicTimer timer = new BasicTimer(key, updateMode, instanceID, duration, onTimerStart, onTimerEnd, onTimerUpdate);
            AddTimer(timer);
            return timer;
        }

        public DelayTimer AddDelayTimer(GameObject ownerOrNull, string key, eTimerUpdateMode updateMode
            , float duration = 0.0f, float delayDuration = 0.0f, Action onTimerStart = null, Action onTimerEnd = null, TimerBase.TimerUpdateCallback onTimerUpdate = null)
        {
            int instanceID = ownerOrNull == null ? 0 : ownerOrNull.GetInstanceID();
            DelayTimer timer = new DelayTimer(key, updateMode, instanceID, duration, delayDuration, onTimerStart, onTimerEnd, onTimerUpdate);
            AddTimer(timer);
            return timer;
        }

        public ChainedTimer AddChainedTimer(GameObject ownerOrNull, string key, eTimerUpdateMode updateMode
            , Action onTimerStart = null, Action onTimerEnd = null, TimerBase.TimerUpdateCallback onTimerUpdate = null)
        {
            int instanceID = ownerOrNull == null ? 0 : ownerOrNull.GetInstanceID();
            ChainedTimer timer = new ChainedTimer(key, updateMode, instanceID, onTimerStart, onTimerEnd, onTimerUpdate);
            AddTimer(timer);
            return timer;
        }

        public void RemoveAllTimerByInstance(int instanceID)
        {
            foreach (var item in _TimerDic)
            {
                if (item.Value.InstanceID == instanceID)
                {
                    RemoveTimer(item.Key);
                }
            }
        }

        public void RemoveTimer(string key)
        {
            Debug.Assert(_TimerDic.ContainsKey(key) || checkContainInBufferedTimer(key) != -1, $"You cannot remove key not contain in mTimerDic [{key} not in]");
            _bufferedRemovedTimer.Add(key);
        }

        public void RemoveTimer(TimerBase timerBase)
        {
            RemoveTimer(timerBase.InstanceNameID);
        }

        public TimerBase GetTimerData(string key)
        {
            TimerBase timer = null;
            if (_TimerDic.ContainsKey(key))
            {
                timer = _TimerDic[key];
            }

            for (int i = 0; i < _bufferedAddedTimer.Count; i++)
            {
                if (_bufferedAddedTimer[i].InstanceNameID == key)
                {
                    timer = _bufferedAddedTimer[i];
                }
            }
            return timer;
        }

        public T GetTimerData<T>(string key) where T : TimerBase
        {
            return GetTimerData(key) as T;
        }

        public void ClearAllTimer()
        {
            _TimerDic.Clear();
            _bufferedAddedTimer.Clear();
            _bufferedRemovedTimer.Clear();
        }

        private void processTimepass(eTimerUpdateMode updateMode, float timePass)
        {
            foreach (var item in _TimerDic)
            {
                if (item.Value.UpdateMode == updateMode)
                {
                    item.Value.CalculateTimePass(timePass);
                }
            }
        }

        private void addAndRemoveBufferedTimers()
        {
            for (int i = 0; i < _bufferedAddedTimer.Count; i++)
            {
                TimerBase item = _bufferedAddedTimer[i];
                Debug.Assert(!_TimerDic.ContainsKey(item.InstanceNameID), $"Timer Dic Already Contain that key [{item.InstanceNameID}]");
                _TimerDic.Add(item.InstanceNameID, item);
            }

            for (int i = 0; i < _bufferedRemovedTimer.Count; i++)
            {
                string key = _bufferedRemovedTimer[i];
                Debug.Assert(_TimerDic.ContainsKey(key) || checkContainInBufferedTimer(key) != -1);
                if (_TimerDic.ContainsKey(key))
                {
                    _TimerDic.Remove(key);
                }
                else
                {
                    _bufferedAddedTimer.RemoveAt(checkContainInBufferedTimer(key));
                }
            }
            _bufferedAddedTimer.Clear();
            _bufferedRemovedTimer.Clear();
        }

        private int checkContainInBufferedTimer(string key)
        {
            int result = -1;
            for (int i = 0; i < _bufferedAddedTimer.Count; i++)
            {
                if (_bufferedAddedTimer[i].InstanceNameID == key)
                {
                    result = i;
                    break;
                }
            }
            return result;
        }
    }
