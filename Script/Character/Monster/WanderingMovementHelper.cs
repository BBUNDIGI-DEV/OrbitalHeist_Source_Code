using UnityEngine;
using UnityEngine.AI;

public class WanderingMovementHelper
{
    private NavMeshAgent mAgent
    {
        get
        {
            return mTranslator.Agent;
        }
    }
    private readonly MonsterWanderingOption mWanderingOption;
    private readonly TranslatorBinder mTranslator;
    private readonly BasicTimer mRestTimer;
    private readonly Vector3 mOriginPos;
    private eAIMovementProgress mWanderingState;

    public WanderingMovementHelper(MonsterWanderingOption wanderingOption, TranslatorBinder translator)
    {
        mRestTimer = GlobalTimer.Instance.AddBasicTimer(translator.Trans.gameObject, "WanderingRest", eTimerUpdateMode.FixedUpdate);
        mWanderingOption = wanderingOption;
        mTranslator = translator;
        mOriginPos = translator.Trans.position;
        mWanderingState = eAIMovementProgress.SetDestination;
        
    }

    public void UpdateWandering()
    {
        switch (mWanderingOption.Type)
        {
            case eWanderingActionType.Stop:
                break;
            case eWanderingActionType.RandomWalkAroundRange:
                mTranslator.SwitchComponent(eTranslatorType.NavAgent);
                doRandomWalkAround();
                break;
            default:
                break;
        }
    }

    private void doRandomWalkAround()
    {
        switch (mWanderingState)
        {
            case eAIMovementProgress.GoToDest:
                if(!mAgent.hasPath || mAgent.remainingDistance <= mAgent.stoppingDistance)
                {
                    mWanderingState = eAIMovementProgress.SetRestTime;
                }
                break;
            case eAIMovementProgress.SetDestination:
                Vector3 outPoint;
                bool result = NavmeshExtension.GetCircularRandomPoint(mOriginPos,mWanderingOption.WanderingRange, mWanderingOption.WanderingRange, out outPoint, mAgent.areaMask);
                
                if(result)
                {
                    mTranslator.SetDestination(outPoint);
                    mWanderingState = eAIMovementProgress.GoToDest;
                }
                else
                {
                    mWanderingState = eAIMovementProgress.SetRestTime;
                }
                break;
            case eAIMovementProgress.Rest:
                if(!mRestTimer.IsActivate)
                {
                    mWanderingState = eAIMovementProgress.SetDestination;
                }
                break;
            case eAIMovementProgress.SetRestTime:
                mRestTimer
                    .ChangeDuration(Random.Range(mWanderingOption.MinWaitTime, mWanderingOption.MaxWaitTime))
                    .StartTimer();
                mWanderingState = eAIMovementProgress.Rest;
                break;
            default:
                Debug.LogError("DefaultDrop");
                break;
        }
    }
}

public enum eAIMovementProgress
{
    GoToDest,
    SetDestination,
    Rest,
    SetRestTime,
}