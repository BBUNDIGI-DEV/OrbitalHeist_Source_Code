using UnityEngine;

public class AIMovementActor : ActorBase
{
    public eAIMovementState CurState
    {
        get; private set;
    }

    private readonly AIMovementConfig Config;
    private readonly System.Action mOnPlayerFounded;
    private readonly WanderingMovementHelper mWanderingHelper;
    private readonly CombatMovementHelper mCombatMovementHelper;

    private Vector3 mPlayerPos
    {
        get
        {
            if(PlayerManager.Instance.AcitvatedPlayerTrans == null)
            {
                return Vector3.zero;
            }
            return PlayerManager.Instance.AcitvatedPlayerTrans.position;

        }
    }

    public AIMovementActor(ActorStateMachine ownerStateMachine, AIMovementConfig config, System.Action onPlayerFound)
        : this(ownerStateMachine, config, config.WanderingOption, config.CombatMovementOption, onPlayerFound)
    {

    }

    public AIMovementActor(ActorStateMachine ownerStateMachine, AIMovementConfig config, MonsterWanderingOption wanderingOption, MonsterCombatMovementOption CombatOption, System.Action onPlayerFound)
    : base(ownerStateMachine, config.BaseConfig, config.name)
    {
        Config = config;
        mWanderingHelper = new WanderingMovementHelper(wanderingOption, OWNER.Translator);
        mCombatMovementHelper = new CombatMovementHelper(CombatOption, OWNER.Translator);
        if (Config.IsSkipWandering)
        {
            CurState = eAIMovementState.Combat;
            OWNER.SetEnabledUpdate(true, eActorType.AIAttack);
            switch (config.CombatMovementOption.MovementType)
            {
                case eCombatMovementType.RushToPlayer:
                    mAgent.stoppingDistance = config.CombatMovementOption.StopDistance;
                    break;
                case eCombatMovementType.EvadeFromPlayer:
                case eCombatMovementType.LookPlayer:
                case eCombatMovementType.DoNothing:
                    mAgent.stoppingDistance = 0.5f;
                    break;
                default:
                    break;
            }

        }
        else
        {
            CurState = eAIMovementState.Wandering;
            mAgent.stoppingDistance = 0.5f;
        }
        mOnPlayerFounded = onPlayerFound;
        SetEnabledUpdating(false);
    }

    public override void UpdateActing(float deltaTime)
    {
        doMovementAction();
        Anim.UpdateMovementAnim(mAgent.velocity);
    }

    public override void InovkeActing(object parameter1, object parameter2)
    {
        Anim.UpdateMovementAnim(Vector3.zero);
        SetEnabledUpdating(true);
    }

    public override void StopActing()
    {
        SetEnabledUpdating(false);
        mRB.DisEnrollSetVelocity(eActorType.AIMovement);
        mRB.DisEnrollLookRotatoin(eActorType.AIMovement);
    }

    public override void DestoryActor()
    {

    }

    public void OnPlayerFounded()
    {
        MonsterStatus status = (OWNER.OwnerCharacterBase as MonsterBase).MonsterStatus;
        if (status.IsEnemeyFoundPlayer)
        {
            return;
        }
        status.IsEnemeyFoundPlayer.Value = true;
        CurState = eAIMovementState.Combat;
        switch (Config.CombatMovementOption.MovementType)
        {
            case eCombatMovementType.RushToPlayer:
                mAgent.stoppingDistance = Config.CombatMovementOption.StopDistance;
                break;
            case eCombatMovementType.EvadeFromPlayer:
            case eCombatMovementType.DoNothing:
                mAgent.stoppingDistance = 0.5f;
                break;
            default:
                break;
        }
        mOnPlayerFounded?.Invoke();
        OWNER.SetEnabledUpdate(true, eActorType.AIAttack);
    }

    private void doMovementAction()
    {
        switch (CurState)
        {
            case eAIMovementState.Wandering:
                doWanderingAction();
                if(searchPlayer())
                {
                    OnPlayerFounded();
                }
                break;
            case eAIMovementState.Combat:
                doCombatMovementAction();
                break;
            default:
                break;
        }
    }

    private void doWanderingAction()
    {
        mWanderingHelper.UpdateWandering();
    }

    private void doCombatMovementAction()
    {
        mCombatMovementHelper.UpdateCombat();
    }

    private bool searchPlayer()
    {
        float sqrtDetection= Config.DetectionRange * Config.DetectionRange;
        Vector3 playerToEnemey = mPlayerPos - mPosition;
        return playerToEnemey.sqrMagnitude < sqrtDetection;
    }
}

public enum eAIMovementState
{
    Pause,
    Wandering,
    Combat,
}