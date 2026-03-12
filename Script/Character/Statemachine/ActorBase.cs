using UnityEngine;
using UnityEngine.AI;

public abstract class ActorBase
{
    public eActorType ActorType
    {
        get
        {
            return BaseConfig.ActorType;
        }
    }

    public ActorConfig BaseConfig
    {
        get; protected set;
    }

    public string NameID
    {
        get; protected set;
    }

    public bool NeedUpdate
    {
        get; private set;
    }

    protected readonly ActorStateMachine OWNER;

    protected Vector3 mPosition
    {
        get
        {
            return OWNER.Translator.Trans.position;
        }
    }

    protected Rigidbody mRB
    {
        get
        {

            return OWNER.Translator.RB;
        }
    }

    protected NavMeshAgent mAgent
    {
        get
        {
            return OWNER.Translator.Agent;
        }
    }

    protected Transform mTransform
    {
        get
        {
            return OWNER.Translator.Trans;
        }
    }

    protected CharacterAnimator Anim
    {
        get
        {
            return OWNER.Anim;
        }
    }

    public ActorBase(ActorStateMachine onwerStateMachine, ActorConfig config, string nameID)
    {
        OWNER = onwerStateMachine;
        BaseConfig = config;
        NameID = nameID;
        SetEnabledUpdating(BaseConfig.IsUpdatedActor);
    }

    public abstract void StopActing();

    public abstract void DestoryActor();

    public virtual void InovkeActing(object parameter1, object parameter2)
    {
        Debug.LogError($"You cannot call invokeActing base method directly, Caller [{ActorType}]");
    }

    public virtual void UpdateActing(float deltaTime)
    {
        Debug.LogError($"You Cannot call placeholder update acting [{NameID}]");
        //place holder
    }

    public void SetEnabledUpdating(bool enabled)
    {
        NeedUpdate = enabled;
    }

    protected void checkParamterValidate(System.Type paramCheckType1, System.Type paramCheckType2, object parameter1, object parameter2)
    {
        bool check1 = false;
        if(paramCheckType1 != null)
        {
            check1 = parameter1 != null && parameter1.GetType() == paramCheckType1;
        }
        else
        {
            check1 = parameter1 == null;
        }

        bool check2 = false;
        if (paramCheckType2 != null)
        {
            check2 = parameter2 != null && parameter2.GetType() == paramCheckType2;
        }
        else
        {
            check2 = parameter2 == null;
        }

        Debug.Assert(check1 && check2,
            $"you pass wrong paramenter in invokeActing, you have to pass [{paramCheckType1}][{paramCheckType2}] but [{parameter1}], [{parameter2}]");
    }
}
