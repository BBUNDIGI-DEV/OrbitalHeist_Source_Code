using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.AI;

public abstract class CharacterBase : MonoBehaviour, IDamagable
{
    public BuffManager BuffHandler
    {
        get; protected set;
    }

    [ShowInInspector]
    public CharacterStatus CharacterStatus
    {
        get; protected set;
    }

    public ActorStateMachine SM
    {
        get; protected set;
    }

    public HitBoxElement Hitbox
    {
        get; private set;
    }

    public TranslatorBinder Translator
    {
        get; private set;
    }

    public CharacterAnimator Anim
    {
        get; private set;
    }
    public GameObjectPool FXPool
    {
        get; private set;
    }
    protected Transform mHitFXSpawnPoint;

    [ShowInInspector]
    private LayeredRigidbody _LRB
    {
        get
        {
            if(Translator == null)
            {
                return null;
            }
            return Translator.RB.GetLayeredRigidbody();
        }
    }


    protected virtual void Awake()
    {
        NavMeshAgent agent = GetComponentInChildren<NavMeshAgent>(true);
        Rigidbody rb = GetComponentInChildren<Rigidbody>(true);
        Translator = new TranslatorBinder(rb, agent);
        if(agent != null)
        {
            agent.enabled = false;
        }
        Anim = GetComponentInChildren<CharacterAnimator>(true);
        Hitbox = GetComponentInChildren<HitBoxElement>(true);
        BuffHandler = new BuffManager(this);
        mHitFXSpawnPoint = transform.FindRecursive("HitFXSpawnPoint");
    }

    protected virtual void Start()
    {
        FXPool = GameObjectPool.TryGetGameobjectPool(transform, "EffectPool");
    }

    public abstract void OnHit(DamageInfo damageInfo);

    public void SetHitBoxEnable(bool toggle)
    {
        Hitbox.Collider.enabled = toggle;
    }

#if UNITY_EDITOR

    [SerializeField, HideLabel] float[] sfPowers;
    [Button]
    private void debugAddBuff(BuffData buffData, float duration, int count, bool skipFX, float power1, float power2, float power3)
    {
        for (int i = 0; i < count; i++)
        {
            BuffHandler.AddBuff(buffData, skipFX, duration, power1, power2, power3);
        }
    }
#endif
}
