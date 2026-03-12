using UnityEngine.AI;
using UnityEngine;

public class CombatMovementHelper
{
    public MonsterCombatMovementOption CombatMovementOption
    {
        get; private set;
    }
    private readonly TranslatorBinder mTranslator;
    private readonly BasicTimer mEvadeRestTimer;

    private Vector3 mPlayerPos
    {
        get
        {
            return PlayerManager.Instance.AcitvatedPlayerTrans.position;
        }
    }
    private eAIMovementProgress mMovementState;

    public CombatMovementHelper(MonsterCombatMovementOption combatMovementOption, TranslatorBinder translator)
    {
        mTranslator = translator;
        mEvadeRestTimer = GlobalTimer.Instance.AddBasicTimer(translator.Trans.gameObject, "EvadeResting", eTimerUpdateMode.FixedUpdate);
        CombatMovementOption = combatMovementOption;
        mMovementState = eAIMovementProgress.SetDestination;
        if (CombatMovementOption.MovementType == eCombatMovementType.RushToPlayer)
        {
            mTranslator.Agent.speed = mTranslator.Agent.speed * CombatMovementOption.RushSpeedIncreasement;
        }
    }

    public void UpdateCombat()
    {
        switch (CombatMovementOption.MovementType)
        {
            case eCombatMovementType.RushToPlayer:
                mTranslator.SwitchComponent(eTranslatorType.NavAgent);
                rushToPlayer();
                break;
            case eCombatMovementType.EvadeFromPlayer:
                mTranslator.SwitchComponent(eTranslatorType.NavAgent);
                evadeFromPlayer();
                break;
            case eCombatMovementType.LookPlayer:
                mTranslator.SwitchComponent(eTranslatorType.Rigidbody);
                lookPlayer();
                break;
            case eCombatMovementType.DoNothing:
                break;
            default:
                Debug.LogError($"switch default state[{CombatMovementOption.MovementType}]");
                break;
        }
    }

    private void rushToPlayer()
    {
        if (Vector3.Distance(mPlayerPos, mTranslator.Trans.position) > mTranslator.Agent.stoppingDistance + 3.0f)
        {
            Vector3 randomCircle = Random.insideUnitCircle;
            randomCircle = new Vector3(randomCircle.x, 0.0f, randomCircle.z);

            Vector3 dest = mPlayerPos + randomCircle * (mTranslator.Agent.stoppingDistance + 1.25f);
            mTranslator.SetDestination(dest);
        }
    }

    private void evadeFromPlayer()
    {
        switch (mMovementState)
        {
            case eAIMovementProgress.GoToDest:
                if (mTranslator.Agent.desiredVelocity == Vector3.zero)
                {
                    mMovementState = eAIMovementProgress.SetRestTime;
                }
                break;
            case eAIMovementProgress.SetDestination:
                Vector3 agentPos = mTranslator.Trans.position;
                Vector3 playerToEnemey = agentPos - mPlayerPos;
                float playerToEnemeyDistance = playerToEnemey.magnitude;
                float evadeDistance = Random.Range(CombatMovementOption.EvadeDistance - 1.0f, CombatMovementOption.EvadeDistance + 1.0f);

                bool result = false;
                Vector3 destPos = Vector3.zero;
                if (playerToEnemeyDistance > CombatMovementOption.EvadeInvokingThreshold)
                {
                    destPos = agentPos;
                    result = mTranslator.Agent.GetCircularRandomPoint(destPos, evadeDistance, out destPos);
                }
                else
                {
                    destPos = agentPos + playerToEnemey.normalized * evadeDistance;
                    result = mTranslator.Agent.GetCircularRandomPoint(destPos, 1.0f, out destPos);
                }

                if (result)
                {
                    mTranslator.SetDestination(destPos);
                    mMovementState = eAIMovementProgress.GoToDest;
                }
                else
                {
                    mMovementState = eAIMovementProgress.SetRestTime;
                }
                break;
            case eAIMovementProgress.Rest:
                if (!mEvadeRestTimer.IsActivate)
                {
                    mMovementState = eAIMovementProgress.SetDestination;
                }
                break;
            case eAIMovementProgress.SetRestTime:
                mEvadeRestTimer
                    .ChangeDuration(Random.Range(CombatMovementOption.MinWaitTime, CombatMovementOption.MaxWaitTime))
                    .StartTimer();
                mMovementState = eAIMovementProgress.Rest;
                break;
            default:
                break;
        }
    }

    private void lookPlayer()
    {
        Debug.Log("HI");
        Vector3 enemyToPlayer = PlayerManager.Instance.AcitvatedPlayerTrans.position - mTranslator.Trans.position;
        mTranslator.RB.EnrollLookRotation(enemyToPlayer, eActorType.AIMovement);
        mTranslator.RB.GetLayeredRigidbody().SetRotationSpeed(CombatMovementOption.LookPlayerSpeed);
    }
}
