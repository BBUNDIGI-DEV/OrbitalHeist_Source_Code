using System.Collections.Generic;
using UnityEngine;
using System.Collections;
using Sirenix.OdinInspector;
using FMODUnity;

public class AttackBoxElement : PoolableMono
{
    public delegate void OnAttackBoxHit(float damage);

    public Collider OwnerHitbox
    {
        get; private set;
    }

    public eAttackBoxLifetimeType LifetimeType
    {
        get
        {
            return mOptions.LifetimeType;
        }
    }

    [SerializeField] private bool sfCannotClearProjectile;
    [SerializeField] protected bool sfDeactiveOnHit;
    [SerializeField] protected bool sfIsTriggerByManual;
    [SerializeField] ParticleBinder sfBindedParticle;
    [SerializeField, ShowIf("sfBindedParticle")] eAttackboxFXSpawnTimingType sfSpawnTimingType;
    [SerializeField] ParticleBinder sfBindedIndicator;
    [SerializeField] EventReference sfBindedFXOnEnabled;
    [SerializeField] EventReference sfBindedFXOnColliderEnabled;

    protected Collider mAttackBox;
    protected AttackBoxOption mOptions;
    protected CharacterBase mAttackBoxOwner;
    protected OnAttackBoxHit mOnAttackBoxHit;
    protected eTargetTag mTargetTag;
    protected DamageInfo mDamageInfo;
    protected Dictionary<int, int> mHitStack;
    protected List<KeyValuePair<int, float>> mIntervalStack;
    protected int mHitCount;
    private float mRemainTime;
    private float mCurrentColliderLifetime;
    private Vector3 mOriginalScale;
    private bool mIsFXActivatedAtOnce;

    private void Awake()
    {
        mHitStack = new Dictionary<int, int>(16);
        mIntervalStack = new List<KeyValuePair<int, float>>(16);
        mAttackBox = GetComponentInChildren<Collider>(true);
    }

    private void OnEnable()
    {
        if (sfBindedParticle != null && sfSpawnTimingType == eAttackboxFXSpawnTimingType.OnEnable)
        {
            sfBindedParticle.gameObject.SetActive(true);
            sfBindedFXOnColliderEnabled.TryPlay();
        }
        mIsFXActivatedAtOnce = false;
        sfBindedFXOnEnabled.TryPlay();
    }

    private void Update()
    {
        if(mAttackBox != null && !mAttackBox.gameObject.activeInHierarchy)
        {
            return;
        }

        for (int i = 0; i < mIntervalStack.Count; i++)
        {
            if (mIntervalStack[i].Value < 0.0f)
            {
                continue;
            }

            mIntervalStack[i] = new KeyValuePair<int, float>(mIntervalStack[i].Key, mIntervalStack[i].Value - Time.deltaTime);
        }

        if (mOptions.LifetimeType == eAttackBoxLifetimeType.ClearWithSkillActor)
        {
            if (mCurrentColliderLifetime < 0.0f)
            {
                gameObject.SetActive(false);
            }
        }
        else if (mOptions.LifetimeType == eAttackBoxLifetimeType.HandleWithManual)
        {
            if (mRemainTime < 0.0f)
            {
                gameObject.SetActive(false);
            }

            if (mCurrentColliderLifetime < 0.0f)
            {
                mAttackBox.enabled = false;
            }
            mRemainTime -= Time.deltaTime;
        }
        else if(mOptions.LifetimeType == eAttackBoxLifetimeType.DeactiveWithParticleBinder)
        {
            if(mIsFXActivatedAtOnce == false && sfBindedParticle.gameObject.activeInHierarchy == true)
            {
                mIsFXActivatedAtOnce = true;
            }

            if(mIsFXActivatedAtOnce == true && sfBindedParticle.gameObject.activeInHierarchy == false)
            {
                gameObject.SetActive(false);
            }

            if (mCurrentColliderLifetime < 0.0f)
            {
                mAttackBox.enabled = false;
            }
        }
        else if (mOptions.LifetimeType == eAttackBoxLifetimeType.HandleByAnotherComponent)
        {

        }

        mCurrentColliderLifetime -= Time.deltaTime;
    }


    protected virtual void OnDisable()
    {
        mHitCount = 0;
        mHitStack.Clear();
        mIntervalStack.Clear();
        transform.localScale = mOriginalScale;
    }

    protected virtual void OnTriggerEnter(Collider other)
    {

        if (mOptions.AttackBoxHitType == eAttackBoxType.OneHitOnly)
        {
            onTriggered(other.gameObject);
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (mOptions.AttackBoxHitType == eAttackBoxType.MultiHit)
        {
            onTriggered(other.gameObject);
        }
    }

    private void OnDestroy()
    {
        if (GlobalTimer.IsExist)
        {
            GlobalTimer.Instance.RemoveAllTimerByInstance(gameObject.GetInstanceID());
        }
    }

    public void SetAttackBoxData(float damage, eTargetTag targetTag, AttackBoxOption option, HitEffectData hitEffectData, Collider ownerHitBox, OnAttackBoxHit onAttackBoxHit, CharacterBase owner)
    {
         mOptions = option;
        mAttackBoxOwner = owner;
        mDamageInfo = new DamageInfo(damage, hitEffectData, mAttackBoxOwner);
        mTargetTag = targetTag;
        mOnAttackBoxHit = onAttackBoxHit;
        SetOnwerHitBox(ownerHitBox);

        if (option.LifetimeType == eAttackBoxLifetimeType.HandleWithManual)
        {
            mRemainTime = option.RemainDuration;
        }

        if (option.IsMeleeboxProjectile)
        {
            GetComponent<MeleeboxProjectile>().IntializeMeleeboxProjectile(option.Speed, option.RemainDuration);
        }

        if (option.IsThrowingMeleebox)
        {
            GetComponent<ThrowingAttackBox>().MoveToDest(option.ThrowingDest, option.ThrowingDuration);
        }

        mOriginalScale = transform.localScale;
        float sizeMultiplier = 1 + option.SizeMultiplier;
        transform.localScale *= sizeMultiplier;
    }

    public void SetColliderLifetime(float duration)
    {
        if (sfBindedParticle != null && sfSpawnTimingType == eAttackboxFXSpawnTimingType.OnColliderEnable)
        {
            sfBindedParticle.gameObject.SetActive(true);
        }

        sfBindedFXOnColliderEnabled.TryPlay();
        mAttackBox.enabled = true;
        mCurrentColliderLifetime = duration;
    }

    public bool IsInHitStack(GameObject otherGameobject)
    {
        bool isInHitStack = false;
        int instanceID = otherGameobject.GetInstanceID();
        switch (mOptions.AttackBoxHitType)
        {
            case eAttackBoxType.OneHitOnly:
                isInHitStack = mHitStack.ContainsKey(instanceID);
                break;
            case eAttackBoxType.MultiHit:
                isInHitStack = (mHitStack.ContainsKey(instanceID) && (mHitStack[instanceID] >= mOptions.MaxHitCounts)) || isInInterval(instanceID);
                break;
            default:
                Debug.Assert(false, $"Default Switch Detected [eAttackBoxType], [{mOptions.AttackBoxHitType}]");
                break;
        }

        return isInHitStack;
    }

    public void ProcessOnHit(GameObject hitObject)
    {
        tryDeactiveOnHit();
        int instanceID = hitObject.GetInstanceID();
        if (mHitStack.ContainsKey(instanceID))
        {
            mHitStack[instanceID]++;
        }
        else
        {
            mHitStack.Add(instanceID, 1);
        }

        if (mAttackBoxOwner == null)
        {
            mDamageInfo.HitData.AttackerPosition = Vector3.zero;
        }
        else
        {
            mDamageInfo.HitData.AttackerPosition = mAttackBoxOwner.Translator.Trans.position;
        }

        mDamageInfo.HitData.ColliderPostiion = transform.position;


        IDamagable damagableTarget = hitObject.GetComponentInParent<IDamagable>();
        Debug.Assert(damagableTarget != null, $"You Cannot hit not damagable object [{hitObject}]");
        DamageInfo damageInfo = mDamageInfo;
        if (mOptions.AttackBoxHitType != eAttackBoxType.MultiHit || mHitStack[instanceID] <= 1)
        {
            if (damageInfo.HitData.UseCameraActing && mHitCount == damageInfo.HitData.CameraActInvokingHitCountThreshold)
            {
                CameraManager.Instance.Actor.ProcessCameraActing(damageInfo.HitData.CameraActingOnHit);
            }
            if (damageInfo.HitData.UseBulletTime && mHitCount == damageInfo.HitData.BulletTimeInvokingHitCountThreshold)
            {
                TimeUtils.PlayBulletTime(damageInfo.HitData.BulletTime);
            }
            if (damageInfo.HitData.UseHitStop)
            {
                if (mAttackBoxOwner.SM.CurrentActorType.IsAttackType())
                {
                    SkillActor skillActor = mAttackBoxOwner.SM.CurrentActorOrNull as SkillActor;
                    skillActor.InvokeHitStop(mDamageInfo.HitData.HitStopOption);
                }
            }
        }
        else if(mOptions.AttackBoxHitType == eAttackBoxType.MultiHit && mHitStack[instanceID] > 1 && !mOptions.InvokeDamagedActorPerHit)
        {
            damageInfo.HitData.HitffectType = eHitEffectType.None;
            damageInfo.HitData.BuffDataOnHitOrNull = null;  
        }

        if (mOptions.AttackBoxHitType == eAttackBoxType.MultiHit && mOptions.DivideFinalDamageByMaxHitCount)
        {
            damageInfo.Damage /= mOptions.MaxHitCounts;
        }

        damagableTarget.OnHit(damageInfo);
        mOnAttackBoxHit?.Invoke(damageInfo.Damage);

        int searchIndex = mIntervalStack.FindIndex((value) => value.Key == instanceID);
        if (searchIndex != -1)
        {
            mIntervalStack[searchIndex] = new KeyValuePair<int, float>(mIntervalStack[searchIndex].Key, mOptions.HitInterval);
        }
        else
        {
            mIntervalStack.Add(new KeyValuePair<int, float>(instanceID, mOptions.HitInterval));
        }
        mHitCount++;
    }

    public void SetOnwerHitBox(Collider ownerHitBox)
    {
        if (OwnerHitbox != null)
        {
            return;
        }
        OwnerHitbox = ownerHitBox;

        if (GetComponent<Collider>() == null)
        {
            return;
        }

        Physics.IgnoreCollision(ownerHitBox, GetComponent<Collider>());
    }

    public void BindingIndicator(float indicatingDuration, float colliderDuration)
    {
        StartCoroutine(indicatingAndSetAttackBoxRoutine(indicatingDuration, colliderDuration));
    }

    private void tryDeactiveOnHit()
    {
        if (!sfDeactiveOnHit)
        {
            return;
        }
        GetComponent<Collider>().enabled = false;
    }

    private void onTriggered(GameObject triggeredObject)
    {
        if (sfIsTriggerByManual)
        {
            return;
        }

        int attackBoxLayer = LayerMask.NameToLayer("AttackBox");
        if (triggeredObject.gameObject.layer == attackBoxLayer)
        {
            if (!sfCannotClearProjectile && gameObject.tag != "Projectile" && triggeredObject.tag == "Projectile")
            {
                ProjectileHandler projHandler = triggeredObject.GetComponent<ProjectileHandler>();
                Debug.Assert(projHandler != null,
                    $"Projectile tagged object must have ProjectileHandler Element [{triggeredObject.name}]");
                projHandler.ClearProjectile();
            }
            return;
        }


        if (IsInHitStack(triggeredObject.gameObject))
        {
            return;
        }

        if (!mTargetTag.CheckTarget(triggeredObject.tag))
        {
            return;
        }

        ProcessOnHit(triggeredObject.gameObject);
    }

    private bool isInInterval(int instanceID)
    {
        int searchIndex = mIntervalStack.FindIndex((value) => value.Key == instanceID);
        if (searchIndex == -1)
        {
            return false;
        }

        return mIntervalStack[searchIndex].Value > 0.0f;
    }

    private IEnumerator indicatingAndSetAttackBoxRoutine(float indicatingDuration, float colliderDuration)
    {
        yield return new WaitUntil(() => mAttackBox.gameObject.activeInHierarchy);
        sfBindedIndicator.gameObject.SetActive(true);
        sfBindedIndicator.SetSimulationSpeed(1 / indicatingDuration);
        yield return new WaitForSeconds(indicatingDuration);
        SetColliderLifetime(colliderDuration);
    }
}

public enum eAttackBoxLifetimeType
{
    ClearWithSkillActor,
    HandleWithManual,
    HandleByAnotherComponent,
    DeactiveWithParticleBinder,
}

public enum eAttackboxFXSpawnTimingType
{
    OnEnable,
    OnColliderEnable,
}
