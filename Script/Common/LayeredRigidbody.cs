using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LayeredRigidbody
{
    [ShowInInspector] private readonly ActorPriortyValueContainer<Vector3> VELOCITY_PRIORITY;
    [ShowInInspector] private readonly ActorPriortyValueContainer<Vector3> ROT_VALUES;

    private Dictionary<eSpeedMultiplierSource, float> mSpeedMultiplierDic;
    private float mZAxisPusher;
    private const float DEFAULT_ROT_SPEED = 10.0f;
    private float mRotSpeed;
    public LayeredRigidbody()
    {
        mZAxisPusher = RuntimeDataLoader.GlobalSetting.ZAxisPusher;
        VELOCITY_PRIORITY = new ActorPriortyValueContainer<Vector3>(new eActorType[]
        {
            eActorType.Dead,
            eActorType.Damaged,
            eActorType.Dash,
            eActorType.Appearance,
            eActorType.NormalChargeAttack,
            eActorType.UltimateAttack,
            eActorType.NormalAttack,
            eActorType.SpecialAttack,
            eActorType.SwitchAttack,
            eActorType.CounterAttack,
            eActorType.TryCounterAttack,
            eActorType.DashAttack,
            eActorType.InputMovement,
            eActorType.AIMovement,
        }
        );

        ROT_VALUES = new ActorPriortyValueContainer<Vector3>(new eActorType[]
        {
            eActorType.Dead,
            eActorType.DashAttack,
            eActorType.Damaged,
            eActorType.Appearance,
            eActorType.NormalChargeAttack,
            eActorType.Dash,
            eActorType.UltimateAttack,
            eActorType.NormalAttack,
            eActorType.SpecialAttack,
            eActorType.SwitchAttack,
            eActorType.CounterAttack,
            eActorType.TryCounterAttack,
            eActorType.AIMovement,
            eActorType.InputMovement,
        }, new eActorType[]
        {
            eActorType.AIMovement,
            eActorType.InputMovement
        });

        mSpeedMultiplierDic = new Dictionary<eSpeedMultiplierSource, float>();

        for (int i = 0; i < (int)eSpeedMultiplierSource.Count; i++)
        {
            eSpeedMultiplierSource speedMulti = (eSpeedMultiplierSource)i;
            mSpeedMultiplierDic.Add(speedMulti, 1.0f);
        }
    }

    public bool EnrollVelocity(Vector3 velocity, eActorType actorType)
    {
        VELOCITY_PRIORITY.EnrollValue(velocity, actorType, out bool isTopPriorityActor);

        return isTopPriorityActor;
    }

    public void EnrollLookRotation(Vector3 lookRotation, eActorType actorType)
    {
        ROT_VALUES.EnrollValue(lookRotation, actorType, out bool isTopPriorityActor);
    }

    public void DisEnrollVelocity(eActorType actorType)
    {
        VELOCITY_PRIORITY.DisEnrollValue(actorType);
    }

    public void DisEnrollLookRotation(eActorType actorType)
    {
        ROT_VALUES.DisEnrollValue(actorType);
    }


    public void DisEnrollVelocityAll()
    {
        VELOCITY_PRIORITY.DisEnrollValueAll();
    }

    public void DisEnrollRotationAll()
    {
        ROT_VALUES.DisEnrollValueAll();
    }

    public bool TryGetVelocity(out Vector3 velocity, out eActorType actor)
    {
        bool result = VELOCITY_PRIORITY.TryGetValue(out velocity, out actor);
        if (!result)
        {
            return false;
        }
        velocity *= getSpeedMultiplier();
        velocity.Set(velocity.x, velocity.y, velocity.z * mZAxisPusher);
        return true;
    }

    public bool TryGetLookRotation(out Vector3 lookRotation, out eActorType actor)
    {
        bool result = ROT_VALUES.TryGetValue(out lookRotation, out actor);
        return result;
    }

    public bool TryGetLookRotationToward(Vector3 currentLookRotation, out Vector3 outRotation, out eActorType actor)
    {
        bool result = ROT_VALUES.TryGetValue(out outRotation, out actor);
        if (!result)
        {
            return false;
        }

        if(mRotSpeed == 0.0f)
        {
            mRotSpeed = DEFAULT_ROT_SPEED;
        }

        outRotation = Vector3.RotateTowards(currentLookRotation, outRotation, mRotSpeed * Time.fixedDeltaTime, 0.0f);
        return true;
    }

    public Vector3 GetVelocity(eActorType actorType)
    {
        return VELOCITY_PRIORITY.GetValueByActor(actorType);
    }

    public void SetVelocityMultiplier(float multiplier, eSpeedMultiplierSource source)
    {
        mSpeedMultiplierDic[source] = multiplier;
    }

    public void SetRotationSpeed(float multiplier)
    {
        mRotSpeed = multiplier;
    }

    private float getSpeedMultiplier()
    {
        float totalMultiplier = 1.0f;
        foreach (var item in mSpeedMultiplierDic)
        {
            totalMultiplier *= item.Value;
        }
        return totalMultiplier;
    }
}

public enum eSpeedMultiplierSource
{
    Skill,
    GotHit,
    Count,
}

[System.Serializable]
public class ActorPriortyValueContainer<T> where T : struct
{
    private readonly eActorType[] PROGRESS_PRIORITY;
    private readonly eActorType[] OVERRIDE_ACTOR;
    [ShowInInspector] private T[] mEnrolledValues;
    private bool[] mIsValueEnrolled;
    private int mTopPriorityIndex;

    [ShowInInspector] private T mEnrolledValue
    {
        get
        {
            if(mTopPriorityIndex == int.MaxValue)
            {
                return default(T);
            }

            return mEnrolledValues[mTopPriorityIndex];
        }
    }

    //[ShowInInspector] private eActorType mTopPriorityActor
    //{
    //    get
    //    {
    //        return PROGRESS_PRIORITY[mTopPriorityIndex];
    //    }
    //}


    public ActorPriortyValueContainer(eActorType[] proirty, eActorType[] overidedActors = null)
    {
        PROGRESS_PRIORITY = proirty;
        OVERRIDE_ACTOR = overidedActors;
        mEnrolledValues = new T[(int)eActorType.Count];
        mIsValueEnrolled = new bool[(int)eActorType.Count];
        mTopPriorityIndex = int.MaxValue;

#if UNITY_EDITOR
        for (int i = 0; i < PROGRESS_PRIORITY.Length; i++)
        {
            int count = 0;
            eActorType current = PROGRESS_PRIORITY[i];
            for (int j = 0; j < PROGRESS_PRIORITY.Length; j++)
            {
                if (current == PROGRESS_PRIORITY[j])
                {
                    count++;
                    if (count > 1)
                    {
                        Debug.LogError($"Duplicate progress priority detected [{current}]");
                    }
                }

            }
        }
#endif
    }

    public bool TryGetValue(out T outValue, out eActorType actor)
    {
        if (mTopPriorityIndex == int.MaxValue)
        {
            actor = eActorType.None;
            outValue = default(T);
            return false;
        }

        Debug.Assert(mIsValueEnrolled[mTopPriorityIndex],
            $"You Cannot get value not enrolled [{findActorFromPriority(mTopPriorityIndex)}]");

        actor = findActorFromPriority(mTopPriorityIndex);
        outValue = mEnrolledValues[mTopPriorityIndex];

        return true;
    }

    public T GetValueByActor(eActorType actor)
    {
        int priorityIndex = findPriority(actor);

        return mEnrolledValues[priorityIndex];
    }


    public void EnrollValue(T value, eActorType actorType, out bool topPriorityEnrolled)
    {
        topPriorityEnrolled = false;
        int priorityIndex = findPriority(actorType);

        mIsValueEnrolled[priorityIndex] = true;
        mEnrolledValues[priorityIndex] = value;
        if (priorityIndex < mTopPriorityIndex)
        {
            mTopPriorityIndex = priorityIndex;
            topPriorityEnrolled = true;
        }

        if (OVERRIDE_ACTOR != null)
        {
            tryUpdateOverridedActor(actorType);
        }
    }

    public void DisEnrollValue(eActorType actorType)
    {
        int priorityIndex = findPriority(actorType);
        mIsValueEnrolled[priorityIndex] = false;
        if (mTopPriorityIndex != priorityIndex)
        {
            return;
        }

        bool isTopPriorityUpdated = false;
        for (int i = 0; i < mEnrolledValues.Length; i++)
        {
            if (mIsValueEnrolled[i])
            {
                mTopPriorityIndex = i;
                isTopPriorityUpdated = true;
                break;
            }
        }

        if (!isTopPriorityUpdated)
        {
            mTopPriorityIndex = int.MaxValue;
        }
    }

    public void DisEnrollValueAll()
    {
        for (int i = 0; i < mIsValueEnrolled.Length; i++)
        {
            mIsValueEnrolled[i] = false;
        }
        mTopPriorityIndex = int.MaxValue;
    }

    private int findPriority(eActorType actorType)
    {
        for (int i = 0; i < PROGRESS_PRIORITY.Length; i++)
        {
            if (actorType == PROGRESS_PRIORITY[i])
            {
                return i;
            }
        }

        return PROGRESS_PRIORITY.Length;
    }

    private eActorType findActorFromPriority(int i)
    {
        if (i == PROGRESS_PRIORITY.Length)
        {
            return eActorType.None;
        }
        return PROGRESS_PRIORITY[i];
    }

    private void tryUpdateOverridedActor(eActorType enrolledActor)
    {
        int newEnrolledPriority = findPriority(enrolledActor);
        for (int i = 0; i < OVERRIDE_ACTOR.Length; i++)
        {
            int priority = findPriority(OVERRIDE_ACTOR[i]);

            if (!mIsValueEnrolled[priority] || priority <= newEnrolledPriority)
            {
                continue;
            }
            mEnrolledValues[priority] = mEnrolledValues[newEnrolledPriority];
        }
    }
}
