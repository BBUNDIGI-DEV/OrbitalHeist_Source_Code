using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class TimeScaleUtil : SingletonClass<TimeScaleUtil>
{
    [ShowInInspector, ReadOnly, HideInEditorMode]
    public eTimeScaleTrigger CurrentTrigger
    {
        get; private set;
    }
    private Dictionary<string, PriorityAndTimeScalePair> mTimeScaleDic;
    private Dictionary<string, PriorityAndTimeScalePair> mFixedTimeScaleDic;
    private float mDefaultFixedTimeScale;

    protected override void Awake()
    {
        base.Awake();
        mDefaultFixedTimeScale = Time.fixedDeltaTime;
        mTimeScaleDic = new Dictionary<string, PriorityAndTimeScalePair>();
        mFixedTimeScaleDic = new Dictionary<string, PriorityAndTimeScalePair>();
        CurrentTrigger = eTimeScaleTrigger.None;
    }

    public void AddTimeScale(string key, PriorityAndTimeScalePair value)
    {
        if(mTimeScaleDic.ContainsKey(key))
        {
            mTimeScaleDic[key] = value;
        }
        else
        {
            mTimeScaleDic.Add(key, value);
        }

        updateTimeScale();
    }

    public void AddFixedTimeScale(string key, PriorityAndTimeScalePair value)
    {
        if (mFixedTimeScaleDic.ContainsKey(key))
        {
            mFixedTimeScaleDic[key] = value;
        }
        else
        {
            mFixedTimeScaleDic.Add(key, value);
        }

        updateTimeScale();
    }

    public void RemoveTimeScale(string key)
    {
        if(!mTimeScaleDic.ContainsKey(key))
        {
            return;
        }

        mTimeScaleDic.Remove(key);
        updateTimeScale();
    }

    public void RemoveFixedTimeScale(string key)
    {
        if (!mFixedTimeScaleDic.ContainsKey(key))
        {
            return;
        }

        mFixedTimeScaleDic.Remove(key);
        updateTimeScale();
    }

    private void updateTimeScale()
    {
        if (mTimeScaleDic.Count == 0)
        {
            Time.timeScale = 1.0f;
            CurrentTrigger = eTimeScaleTrigger.None;
        }
        else
        {
            PriorityAndTimeScalePair topPriority = pickSpeed(mTimeScaleDic);
            CurrentTrigger = topPriority.Priority;
            Time.timeScale = topPriority.TimeScale;
        }

        if (mFixedTimeScaleDic.Count == 0)
        {
            Time.fixedDeltaTime = mDefaultFixedTimeScale;
        }
        else
        {
            PriorityAndTimeScalePair topPriority = pickSpeed(mFixedTimeScaleDic);
            Time.fixedDeltaTime = topPriority.TimeScale;
        }
    }

    private PriorityAndTimeScalePair pickSpeed(Dictionary<string, PriorityAndTimeScalePair> dic)
    {
        PriorityAndTimeScalePair topPriority = new PriorityAndTimeScalePair(eTimeScaleTrigger.None, 1.0f);
        foreach (var item in dic)
        {
            if((int)item.Value.Priority > (int)topPriority.Priority)
            {
                topPriority = item.Value;
            }
        }

        return topPriority;
    }
}

public struct PriorityAndTimeScalePair
{
    public eTimeScaleTrigger Priority;
    public float TimeScale;

    public PriorityAndTimeScalePair(eTimeScaleTrigger priority, float timeScale)
    {
        Priority = priority;
        TimeScale = timeScale;
    }
}

public enum eTimeScaleTrigger
{
    None = -1,
    BulletTime,
    UltimateCinemachinePlaying,
    Tutoiral,
    StageCleared,
    PauseGame
}
