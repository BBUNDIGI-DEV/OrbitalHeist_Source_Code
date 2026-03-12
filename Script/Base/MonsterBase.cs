using Sirenix.OdinInspector;
using System;
using UnityEditor;
using UnityEngine;

public abstract class MonsterBase : CharacterBase
{
    public static GlobalMonsterBalanceData sGlobalData
    {
        get
        {
            if(smGlobalData == null)
            {
                smGlobalData = Resources.Load<GlobalMonsterBalanceData>("BalanceData/Setting_000DefaultBalance");
            }
            return smGlobalData;
        }
        set
        {
            smGlobalData = value;
        }
    }
    private static GlobalMonsterBalanceData smGlobalData;
    public MonsterStatus MonsterStatus
    {
        get; protected set;
    }

    public MonsterConfig MonsterConfig
    {
        get
        {
            return sfConfig;
        }
    }

    public Action<MonsterBase> OnMonsterDead;

    [SerializeField, Required, AssetsOnly] protected MonsterConfig sfConfig;
    [SerializeField, LabelText("Set Wandering Option Manually?")] protected bool sfUseManualWanderingOption;
    [SerializeField, LabelText("Override Monster Wandering Option"), ShowIf("USE_MANUAL_WANDERING_OPTION")] protected MonsterWanderingOption sfOverridWanderingOption;

    protected ActorStateMachine mStateMachine;
    protected BilliardHelper mBilliardHelperOrNull;
     
    private bool mIsInitialized;

    private void OnEnable()
    {
        AimHelper.AddEnemeyTrans(Translator.Trans);
        Hitbox.Collider.enabled = true;
    }

    protected override void Awake()
    {
        MonsterStatus = new MonsterStatus();
        CharacterStatus = MonsterStatus;
        MonsterStatus.MaxHP.Value = sfConfig.InitialMaxHP * sGlobalData.GlobalMaxHPMultiplier;
        MonsterStatus.CurrentHP.Value = sfConfig.InitialHP * sGlobalData.GlobalMaxHPMultiplier;
        MonsterStatus.Defense = sfConfig.InitialDefense * sGlobalData.GlobalDefenceMultiplier;
        MonsterStatus.IsSuperArmor.Value = sfConfig.InitialSuperArmor;
        MonsterStatus.ShieldAmount.Value = new Gauge(sfConfig.InitialShieldAmount * sGlobalData.GlobalMaxHPMultiplier, sfConfig.InitialMaxShieldAmount * sGlobalData.GlobalMaxHPMultiplier);

        transform.FindRecursiveGameobjectWithTag("CharacterTrans").gameObject.SetActive(false);
        base.Awake();
    }

    protected override void Start()
    {
        base.Start();
        initializeStateMachine();
        PlayerManager.Instance.SetMonsterColliderIgnorePlayer(SM.Translator.Trans.GetComponent<Collider>());
        Translator.Agent.speed = sfConfig.InitialSpeed;
        Translator.Agent.avoidancePriority = UnityEngine.Random.Range(10, 100);
        if(mIsInitialized)
        {
            if(SM.HasActor(eActorType.Appearance))
            {
                SM.TrySwitchActor(eActorType.Appearance);
            }
        }
    }

    private void Update()
    {
        if(MonsterStatus.IsDead)
        {
            return;
        }

        if(!mIsInitialized)
        {
            return;
        }

        if (SM.CurrentActorType == eActorType.None)
        {
            SM.TrySwitchActor(eActorType.AIMovement);
        }

        SM.Translator.UpdateTransform();
        BuffHandler.UpdateBuff(Time.deltaTime);
    }

    private void FixedUpdate()
    {
        if (MonsterStatus.IsDead)
        {
            return;
        }

        if (!mIsInitialized)
        {
            return;
        }

        SM.UpdateActor(Time.fixedDeltaTime);
    }


    private void OnDisable()
    {
        AimHelper.TryRemoveEnemeyTrans(Translator.Trans);
        if (!GlobalTimer.IsExist)
        {
            return;
        }

        GlobalTimer.Instance.RemoveAllTimerByInstance(gameObject.GetInstanceID());
    }

    private void OnDestroy()
    {
        if (SM.CurrentActorType != eActorType.Dead)
        {
            SM.DestoryActors();
        }
    }


    public void SetAsSleepMode()
    {
        transform.FindRecursiveGameobjectWithTag("CharacterTrans").gameObject.SetActive(true);
    }

    public void Initialized(Vector3 initialPos, Quaternion rotation)
    {
        Translator.Trans.localPosition = initialPos;
        Translator.Trans.localRotation = rotation;
        Initialized();
    }

    public void Initialized()
    {
        mIsInitialized = true;
        transform.FindRecursiveGameobjectWithTag("CharacterTrans").gameObject.SetActive(true);

        if(SM != null && SM.HasActor(eActorType.Appearance))
        {
            SM.TrySwitchActor(eActorType.Appearance);
        }
    }

    public override void OnHit(DamageInfo damageInfo)
    {
        if(CharacterStatus.IsPowerOverwalming.GetValue())
        {
            return;
        }

        bool isShieldDecreased;
        float damageReduction = Mathf.Clamp(1 - CharacterStatus.Defense, 0.2f, 1f);
        float damage = damageInfo.Damage * damageReduction;
        damageInfo.Damage = damage;
        MonsterStatus.LastDamageInfo.Value = damageInfo;

        MonsterStatus.DecreaseHP(damage, out isShieldDecreased);

        SFXManager.PlayMonsterHitSound();

        if (damageInfo.HitData.OnHitFX != null)
        {
            ParticleBinder effect = FXPool.GetGameobject(damageInfo.HitData.OnHitFX);
            effect.SetFXTransformType(eFXTransformType.PositionOnlyWorld,Translator.Trans, mHitFXSpawnPoint.localPosition);
        }

        damageInfo.HitData.OnHitSound.TryPlay();
        if (damageInfo.HitData.BuffDataOnHitOrNull != null)
        {
            BuffHandler.AddBuff(damageInfo.HitData.BuffDataOnHitOrNull, damageInfo.HitData.BuffDuration, damageInfo.HitData.BuffStackPerHit, damageInfo.HitData.BuffPower1, damageInfo.HitData.BuffPower2, damageInfo.HitData.BuffPower3);
        }
        if (MonsterStatus.CurrentHP <= 0)
        {
            AimHelper.RemoveEnemeyTrans(Translator.Trans);
            SM.TrySwitchActor(eActorType.Dead, damageInfo.HitData, new Action(DestoryMonster));
            ItemDataUtil.TrySpawnItem(Translator.Trans.position);
            FXPool.gameObject.SetActive(false);
            BuffHandler.ClearAllBuff();
            OnMonsterDead?.Invoke(this);
            return;
        }
        else
        {
            if (damageInfo.HitData.HitffectType == eHitEffectType.None)
            {
                return;
            }
            SM.Anim.PlayOnHitMaterailAnim();
            if(isShieldDecreased || MonsterStatus.IsSuperArmor)
            {
                return; 
            }
            SM.TrySwitchActor(eActorType.Damaged, damageInfo.HitData, null);
            mBilliardHelperOrNull?.SetBillardInfo(damageInfo);
        }
    }

    public void MakeMonsterDeadWithSkipDeadActor()
    {
        OnMonsterDead?.Invoke(this);
        SM.TrySwitchActor(eActorType.Dead, null , new Action(DestoryMonster));
    }

    public void DestoryMonster()
    {
        if(sfConfig.DontDestoryOnDead)
        {
            return;
        }

        MonsterStatus.IsDestory.Value = true;
        SM.DestoryActors();
        Destroy(gameObject);
    }

    public void SetMonsterSpeed()
    {
        Translator.Agent.speed = sfConfig.InitialSpeed;
    }

    protected abstract void initializeStateMachine();

    protected void onDamagedEnd()
    {
        mBilliardHelperOrNull?.StopBilliard();
        SM.TrySwitchActor(eActorType.AIMovement);
    }

    protected void onStunEnd()
    {
        SM.TrySwitchActor(eActorType.AIMovement);
    }

#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        if (sfConfig == null)
        {
            return;
        }

        if (sfConfig.AIMovementConfig == null)
        {
            return;
        }

        Rigidbody rb = GetComponentInChildren<Rigidbody>(true);
        Handles.color = Color.yellow;
        Handles.DrawWireDisc(rb.position + Vector3.up, Vector3.up, sfConfig.AIMovementConfig.DetectionRange);
    }
#endif

}

public class MonsterStatus : CharacterStatus
{
    public ObservedData<Gauge> ShieldAmount;

    public ObservedData<bool> IsSuperArmor;
    public ObservedData<bool> IsDestory;
    public ObservedData<bool> IsEnemeyFoundPlayer;
    public ObservedData<eFloatingInfoMessageTag> FloatingInfomessageTrigger;

    public override void AddHP(float amount)
    {
        if (amount > 0)
        {
            IncreaseHP(amount);
        }
        else
        {
            DecreaseHP(-amount);
        }
    }

    public void IncreaseHP(float healAmount)
    {
        if (Mathf.Approximately(CurrentHP, MaxHP))
        {
            healAmount = Mathf.Clamp(healAmount, 0.0f, ShieldAmount.Value.Max - ShieldAmount.Value.Current);
            ShieldAmount.Value = new Gauge(ShieldAmount.Value.Current + healAmount, ShieldAmount.Value);
        }
        else
        {
            healAmount = Mathf.Clamp(healAmount, 0.0f, MaxHP - CurrentHP);
            CurrentHP.Value -= healAmount;
        }
    }

    public void DecreaseHP(float dealAmount)
    {
        bool placeholder;
        DecreaseHP(dealAmount, out placeholder);
    }

    public void DecreaseHP(float dealAmount, out bool isShieldDecreased)
    {
        isShieldDecreased = false;
        dealAmount = Mathf.Clamp(dealAmount, 2.0f, dealAmount);
        dealAmount = Mathf.Max(dealAmount, 2.0f);

        if (ShieldAmount.Value.Current > 0.0f)
        {
            dealAmount = Mathf.Clamp(dealAmount, 0.0f, ShieldAmount.Value.Current);
            ShieldAmount.Value = ShieldAmount.Value.AddGuage(-dealAmount);
            isShieldDecreased = true;
        }
        else
        {
            dealAmount = Mathf.Clamp(dealAmount, 0.0f, CurrentHP);
            CurrentHP.Value -= dealAmount;
        }
    }
}

