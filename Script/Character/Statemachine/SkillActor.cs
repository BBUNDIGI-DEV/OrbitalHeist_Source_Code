using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class SkillActor : ActorBase
{
    public delegate void SkillAnimEventCallback(eSkillEventMarkerType markerType, string clipName);

    public SkillConfig SkillConfig
    {
        get; protected set;
    }

    public eSkillProgressState CurrentProgressState
    {
        get; private set;
    }

    public bool IsOnAttack
    {
        get
        {
            return CurrentProgressState != eSkillProgressState.None;
        }
    }

    public bool CanCancelable
    {
        get
        {
            return CurrentProgressState == eSkillProgressState.Preparing || CurrentProgressState == eSkillProgressState.Cancelable;
        }
    }

    public bool CanAttack
    {
        get
        {
            return !IsOnAttack
                && !mCooltimeTimer.IsActivate
                && OWNER.CurrentActorType != eActorType.Damaged;
        }
    }

    protected static GameObjectPool GLOBAL_ATTACK_BOX_POOL;
    protected readonly GameObjectPool LOCAL_ATTACK_BOX_POOL;
    protected readonly GameObjectPool PROJECTILE_POOL;
    protected readonly GameObjectPool EFFECT_POOL;
    protected readonly System.Action ON_ATTACK_END;

    protected readonly BasicTimer mTransitionTimer;
    protected readonly BasicTimer mTrackPlayerUpdateTimer;
    protected readonly BasicTimer mCooltimeTimer;
    protected readonly ChainedTimer mProjectileMultiShotTimer;

    protected readonly List<AttackBoxElement> mSpawnedAttackBoxes;
    protected readonly List<ParticleBinder> mSpawnedFX;
    protected readonly List<ParticleBinder> mSpawnedIndicator;

    protected readonly Transform mProjectileShootingPoint;
    protected readonly Collider mOwnerHitBox;
    private readonly AttackBoxElement.OnAttackBoxHit mOnAttackBoxHit;

    private SkillConfig mStackedConfig;
    private Vector3 mAttackDir;
    private Vector3 mAssistedPos;
    private float mDamage;

    private int mCurrentManualSkillEventIndexes;
    private int[] mSkillEventIndex;

    private ChainedTimer mHitStopTimer;
    private float mHitStopSpeedMultiplier
    {
        set
        {
            _mHitStopSpeedMultiplier = value;
            updateAttackSpeed();
        }
        get
        {
            return _mHitStopSpeedMultiplier;
        }
    }
    private float _mHitStopSpeedMultiplier;

    public SkillActor(ActorStateMachine onwerStateMachine, SkillConfig skillConfig, AttackBoxElement.OnAttackBoxHit onAttackBoxHit, System.Action onAttackEnd)
    : base(onwerStateMachine, skillConfig.BaseConfig, skillConfig.name)
    {
        Debug.Assert(skillConfig.BaseConfig.ActorType.ToString().Contains("Attack"),
            $"Don't pass eActortype not contain \"Attack\" in SkillActor [{skillConfig.BaseConfig.ActorType}]," +
            $" Only attack actorType can be type of SkillActor");

        GameObject ownerGameobject = onwerStateMachine.OwnerCharacterBase.gameObject;
        ON_ATTACK_END = onAttackEnd;
        if(GLOBAL_ATTACK_BOX_POOL == null)
        {
            GLOBAL_ATTACK_BOX_POOL = GameObjectPool.TryGetGameobjectPoolByTag("GlobalAttackbox");
        }
        LOCAL_ATTACK_BOX_POOL = GameObjectPool.TryGetGameobjectPool(ownerGameobject.transform, "AttackBoxPool");
        EFFECT_POOL = GameObjectPool.TryGetGameobjectPool(ownerGameobject.transform, "EffectPool");
        PROJECTILE_POOL = GameObjectPool.TryGetGameobjectPool(ownerGameobject.transform, "ProjectilePool");
        mOnAttackBoxHit = onAttackBoxHit;
        if (!OWNER.IsPlayerActor)
        {
            mTrackPlayerUpdateTimer = GlobalTimer.Instance.AddBasicTimer(ownerGameobject, getTimerKeyString(skillConfig, "TrackPlayer"), eTimerUpdateMode.FixedUpdate);
        }

        mTransitionTimer = GlobalTimer.Instance.AddBasicTimer(ownerGameobject, getTimerKeyString(skillConfig, "Transition"), eTimerUpdateMode.FixedUpdate);
        mCooltimeTimer = GlobalTimer.Instance.AddBasicTimer(ownerGameobject, getTimerKeyString(skillConfig, "CoolTime"), eTimerUpdateMode.FixedUpdate);
        mProjectileMultiShotTimer = GlobalTimer.Instance.AddChainedTimer(ownerGameobject, getTimerKeyString(skillConfig, "ProjectileTimer"), eTimerUpdateMode.FixedUpdate);
        mProjectileShootingPoint = ownerGameobject.transform.FindRecursive(skillConfig.ShootingPointTransName);
        changeConfig(skillConfig.GetRuntimeSkillConfig());

        ownerGameobject.GetComponentInChildren<SkillAnimEventReciver>(true).EnrollEventInvoking(skillConfig.BaseConfig.ActorType, invokeSkillEvent);
        mSpawnedAttackBoxes = new List<AttackBoxElement>();
        mSpawnedFX = new List<ParticleBinder>();
        mSpawnedIndicator = new List<ParticleBinder>();
        CurrentProgressState = eSkillProgressState.None;

        mSkillEventIndex = new int[(int)eSkillEventMarkerType.Count];
        mOwnerHitBox = onwerStateMachine.OwnerCharacterBase.Hitbox.Collider;
        SetEnabledUpdating(false);

        mCooltimeTimer.ChangeDuration(SkillConfig.InitialCoolTime).StartTimer();
    }

    public void TryChangeConfigOrStackedIn(SkillConfig newSkillConfig)
    {
        if (IsOnAttack)
        {
            mStackedConfig = newSkillConfig;
        }
        else
        {
            changeConfig(newSkillConfig);
        }
    }

    public override void InovkeActing(object parameter1, object parameter2)
    {
        checkParamterValidate(typeof(float), null, parameter1, parameter2);
        if(OWNER.IsPlayerActor)
        {
            SkillConfig.RuntimeAttackSpeedMultiplier = 1.0f + OWNER.OwnerCharacterBase.CharacterStatus.AttackSpeedMultiplier + PlayerManager.Instance.GlobalPlayerStatus.GlobalAttackSpeedMultiplier;
        }
        else
        {
            SkillConfig.RuntimeAttackSpeedMultiplier = (1.0f + OWNER.OwnerCharacterBase.CharacterStatus.AttackSpeedMultiplier) * MonsterBase.sGlobalData.GlobalAttackSpeedMultiplier;
        }
        OWNER.Translator.SwitchComponent(eTranslatorType.Rigidbody);
        mDamage = (float)parameter1;
        mAssistedPos = Vector3.zero;
        mAttackDir = getAttackDir(out mAssistedPos);

        switch (SkillConfig.RotType)
        {
            case eSkillRotateType.Default:
                mRB.EnrollLookRotation(mAttackDir, ActorType);
                break;
            case eSkillRotateType.Immediate:
                mTransform.rotation = Quaternion.LookRotation(mAttackDir);
                mRB.EnrollLookRotationAndForceRotating(mAttackDir, ActorType);
                break;
            case eSkillRotateType.Fix:
                break;
            default:
                break;
        }

        handleAttackTransition(SkillConfig.TransitionData);

        setSkillProgressState(eSkillProgressState.Preparing);
        Anim.TriggerAttackAnim(SkillConfig);

        mCurrentManualSkillEventIndexes = 0;

        if (SkillConfig.BuffDataOnUsingSkill != null)
        {
            for (int i = 0; i < PlayerManager.Instance.ActivatedCharacters.Count; i++)
            {
                if(PlayerManager.Instance.ActivatedCharacters[i].CharacterStatus.IsDead)
                {
                    continue;
                }
                PlayerManager.Instance.ActivatedCharacters[i].BuffHandler.AddBuff(SkillConfig.BuffDataOnUsingSkill, true, SkillConfig.BuffDuration, SkillConfig.BuffPower);
            }
        }

        if (SkillConfig.CounterTrySkill)
        {
            //ForceCounterInvokingBox counterBox = MELEE_ATTACKBOX_POOL.GetGameobject(SkillConfig.CounterBox);
            //counterBox.SetCounterBoxData(SkillConfig.TargetTag, OWNER.OwnerCharacterBase);
        }

        if (SkillConfig.UsingBulletTimeOnSkillInvoked)
        {
            TimeUtils.PlayBulletTime(SkillConfig.BulletTimeOnUsingSkill);
        }
         
        for (int i = 0; i < mSkillEventIndex.Length; i++)
        {
            mSkillEventIndex[i] = 0;
        }

        if (SkillConfig.ManualSkillEvents != null && SkillConfig.ManualSkillEvents.Length != 0)
        {
            SetEnabledUpdating(true);
        }
        else
        {
            SetEnabledUpdating(false);
        }

        if(BaseConfig.ActorType == eActorType.UltimateAttack && OWNER.IsPlayerActor)
        {
            PlayerCharacterController pc = OWNER.OwnerCharacterBase as PlayerCharacterController;
            pc.PlayUltimateSkillTimeline();
        }

        AnimationEvent[] events = SkillConfig.AttackMotion.events;
        int attackBoxCount = 0;
        int projectileCount = 0;
        for (int i = 0; i < events.Length; i++)
        {
            if (events[i].IsSkillEvent())
            {
                events[i].GetSkillEventData(out eActorType skillActor, out eSkillEventMarkerType markerType);

                if (markerType == eSkillEventMarkerType.InvokeAttackBox)
                {
                    if (!SkillConfig.MeleeAttackData[attackBoxCount].UseIndicator)
                    {
                        continue;
                    }

                    ParticleBinder indicator = spawnIndicator(SkillConfig.MeleeAttackData[attackBoxCount].IndicatorParticle, events[i].time);
                    indicator.SetFXTransformType(SkillConfig.MeleeAttackData[attackBoxCount].IndicatorFXTransformType, mTransform);
                    attackBoxCount++;
                    mSpawnedIndicator.Add(indicator);
                }
                else if (markerType == eSkillEventMarkerType.InvokeProjectile)
                {
                    ProjectileAttackData attackData = SkillConfig.ProjectileData[projectileCount];
                    if (!attackData.UseIndicator)
                    {
                        continue;
                    }

                    if (attackData.ShootingType == ProjectileAttackData.eShootingType.ShotGun)
                    {
                        float intialRot = attackData.ShotgunAmount == 1 ? 0.0f : -attackData.ShotgunAngle / 2;
                        float rotAmountPerShoot = attackData.ShotgunAmount == 1 ? 0.0f : attackData.ShotgunAngle / (attackData.ShotgunAmount - 1);

                        for (int j = 0; j < attackData.ShotgunAmount; j++)
                        {
                            Quaternion newRot = Quaternion.Euler(0.0f, intialRot + rotAmountPerShoot * j, 0.0f);
                            Vector3 newAttackDir = newRot * mAttackDir;
                            ParticleBinder indicator = spawnIndicator(SkillConfig.ProjectileData[projectileCount].IndicatorParticle, events[i].time);
                            indicator.SetFXTransformType(mTransform.position, Quaternion.LookRotation(newAttackDir));
                            mSpawnedIndicator.Add(indicator);
                        }
                    }
                    else
                    {
                        ParticleBinder indicator = spawnIndicator(SkillConfig.MeleeAttackData[attackBoxCount].IndicatorParticle, events[i].time);
                        indicator.SetFXTransformType(SkillConfig.MeleeAttackData[attackBoxCount].IndicatorFXTransformType, mTransform);
                        mSpawnedIndicator.Add(indicator);
                    }
                    projectileCount++;
                }
            }
        }

        updateManualSkillEventTiming();

        // Hard coding
        if(BaseConfig.ActorType == eActorType.UltimateAttack && OWNER.IsPlayerActor && (OWNER.OwnerCharacterBase as PlayerCharacterController).CharName == eCharacterName.Hypo)
        {
            PlayerManager.Instance.GlobalPlayerStatus.ForceShieldAmount.Value = 3;
        }
    }

    public override void UpdateActing(float deltaTime)
    {
        updateManualSkillEventTiming();
    }

    public override void StopActing()
    {
        if (CurrentProgressState == eSkillProgressState.None)
        {
            return;
        }
        setSkillProgressState(eSkillProgressState.None);
    }

    public override void DestoryActor()
    {

    }

    public void InvokeForceCountering(CharacterBase target)
    {
        if (mSkillEventIndex[(int)eSkillEventMarkerType.InvokeAttackBox] != 0)
        {
            return;
        }

        Debug.DrawLine(target.Translator.Trans.position, OWNER.Translator.Trans.position, Color.red, 10.0f);
        mRB.EnrollLookRotationAndForceRotating(target.Translator.Trans.position - OWNER.Translator.Trans.position, ActorType);
        Anim.WarpToSpecifiedSkillEvent(eSkillEventMarkerType.InvokeAttackBox);
    }

    public void InvokeHitStop(HitStopData hitStopData)
    {
        const string SPEED_UP_KEY = "HitStopSpeedUp";
        const string BACK_TO_NORMAL = "HitStopBackToNormal";

        if (mHitStopTimer == null)
        {
            mHitStopTimer = GlobalTimer.Instance.AddChainedTimer(OWNER.OwnerCharacterBase.gameObject, SkillConfig.name + "HitStopTimer", eTimerUpdateMode.Update);
            mHitStopTimer.AddCallback(SPEED_UP_KEY, 0.0f, null);
            mHitStopTimer.AddCallback(BACK_TO_NORMAL, 0.0f, null);
        }

        mHitStopSpeedMultiplier = hitStopData.SlowDownAmount;
        mHitStopTimer
        .ChangeTiming(SPEED_UP_KEY, hitStopData.SlowTime)
        .ChangeCallback(SPEED_UP_KEY, () => mHitStopSpeedMultiplier = 1)
        .ChangeTiming(BACK_TO_NORMAL, hitStopData.SlowTime)
        .ChangeCallback(BACK_TO_NORMAL, () => mHitStopSpeedMultiplier = 1)
        .StartTimer();
        updateAttackSpeed();
    }



    protected virtual void onAttackEnd()
    {
        if (SkillConfig.TransitionData.ActionType == eAttackTransitionType.MoveToSpecificDest && mTransitionTimer.IsActivate)
        {
            mTransitionTimer.StopTimer(true);
        }

        if (mHitStopTimer != null && mHitStopTimer.IsActivate)
        {
            mHitStopTimer.StopTimer(false);
            mHitStopSpeedMultiplier = 1.0f;
        }

        if (mTrackPlayerUpdateTimer != null && mTrackPlayerUpdateTimer.IsActivate)
        {
            mTrackPlayerUpdateTimer.StopTimer(true);
        }

        for (int i = 0; i < mSpawnedAttackBoxes.Count; i++)
        {
            if (mSpawnedAttackBoxes[i].LifetimeType == eAttackBoxLifetimeType.ClearWithSkillActor)
            {
                mSpawnedAttackBoxes[i].gameObject.SetActive(false);
            }
        }
        mSpawnedAttackBoxes.Clear();

        for (int i = 0; i < mSpawnedFX.Count; i++)
        {
            mSpawnedFX[i].SetSimulationSpeed(1);
        }
        mSpawnedFX.Clear();

        for (int i = 0; i < mSpawnedIndicator.Count; i++)
        {
            mSpawnedIndicator[i].gameObject.SetActive(false);
        }
        mSpawnedIndicator.Clear();

        float coolTime = SkillConfig.CoolTime;

        if (!OWNER.IsPlayerActor)
        {
            coolTime += Random.Range(-SkillConfig.CoolTimeRandomRange, SkillConfig.CoolTimeRandomRange);
        }

        mCooltimeTimer.ChangeDuration(coolTime).StartTimer();
        mRB.DisEnrollSetVelocity(ActorType);
        mRB.GetLayeredRigidbody().SetVelocityMultiplier(1, eSpeedMultiplierSource.Skill);
        mRB.DisEnrollLookRotatoin(ActorType);
        mRB.GetLayeredRigidbody().SetRotationSpeed(0.0f);
        OWNER.CheckAndClearActor(NameID);

        if (mProjectileMultiShotTimer.IsActivate)
        {
            mProjectileMultiShotTimer.StopTimer(false);
        }

        if (ON_ATTACK_END != null)
        {
            ON_ATTACK_END.Invoke();
        }

        Anim.EndAttackAnim();

        if (mStackedConfig != null)
        {
            changeConfig(mStackedConfig);
            mStackedConfig = null;
        }
        OWNER.OwnerCharacterBase.CharacterStatus.IsPowerOverwalming.DisEnrollValue(CharacterStatus.ePowerOverwalmingSetter.SetBySkill);
    }

    protected virtual string getTimerKeyString(SkillConfig config, string baseName)
    {
        return baseName + config.name;
    }

    protected void setNextSkillProgressState()
    {
        eSkillProgressState newState;
        if (CurrentProgressState == eSkillProgressState.Cancelable)
        {
            newState = eSkillProgressState.None;
        }
        else
        {
            newState = CurrentProgressState + 1;
        }
        setSkillProgressState(newState);
    }

    protected virtual void setSkillProgressState(eSkillProgressState state)
    {
        CurrentProgressState = state;
        updateAttackSpeed();
        switch (CurrentProgressState)
        {
            case eSkillProgressState.None:
                onAttackEnd();
                break;
            case eSkillProgressState.Preparing:
                break;
            case eSkillProgressState.OnAttack:
                break;
            case eSkillProgressState.Cancelable:
                if (SkillConfig.IsExplositionAttack)
                {
                    MonsterBase monsterBase = OWNER.OwnerCharacterBase as MonsterBase;
                    monsterBase.MakeMonsterDeadWithSkipDeadActor();
                    return;
                }
                OWNER.TryInvokeBufferedActor();
                break;
            default:
                Debug.LogError($"Default switch detected [{CurrentProgressState}]");
                break;
        }
    }

    protected void changeConfig(SkillConfig config)
    {
#if UNITY_EDITOR
        Debug.Assert(!AssetDatabase.Contains(config));
#endif

        SkillConfig = config;
        BaseConfig = SkillConfig.BaseConfig;
    }

    protected virtual void invokeSkillEvent(eSkillEventMarkerType markerType, string clipName)
    {
        //애니메이터가 애니메이션 끼리 보간하는 Transition 중간에도 애니메이션 이벤트가 발생하는데
        //이를 Animator.IsInTransiton으로 예외처리할 생각이었으니 정상적으로 동작을 하지 않아
        //Config.AttackMotion을 통해 현재 활성화 된 ClipName을 파악하고 이를 애니메이션 이벤트에서 확보할수 있는 정보인 해당 이벤트를 발생시킨 clipname과 비교하여 예외처리를 함
        if (!clipName.Contains(SkillConfig.AttackMotion.name))
        {
            return;
        }
        invokeSkillEvent(markerType);
    }

    protected virtual void invokeSkillEvent(eSkillEventMarkerType markerType)
    {
        if (CurrentProgressState == eSkillProgressState.None)
        {
            return;
        }

        if (!checkSkillEventExcutable(markerType))
        {
            return;
        }

        int eventIndex = mSkillEventIndex[(int)markerType];
        switch (markerType)
        {
            case eSkillEventMarkerType.InvokeSkillFX:
                FXSpawnData fxInfo = SkillConfig.FXSpawnData[eventIndex];
                mSkillEventIndex[(int)markerType]++;
                spawnFX(fxInfo);
                break;
            case eSkillEventMarkerType.InvokeAttackBox:
                handleAttackBox(SkillConfig.MeleeAttackData[eventIndex]);
                mSkillEventIndex[(int)markerType]++;
                break;
            case eSkillEventMarkerType.InvokePointSpecifyAttack:
                handlePointSpecifyAttack(SkillConfig.PointSpecifyAttackData[eventIndex]);
                mSkillEventIndex[(int)markerType]++;
                break;
            case eSkillEventMarkerType.InvokeProjectile:
                handleProjectileAttackData(SkillConfig.ProjectileData[eventIndex]);
                mSkillEventIndex[(int)markerType]++;
                break;
            case eSkillEventMarkerType.ChangeProgressState:
                setNextSkillProgressState();
                break;
            case eSkillEventMarkerType.InvokeSound:
                FMODUnity.RuntimeManager.PlayOneShot(SkillConfig.AttackSound[eventIndex]);
                mSkillEventIndex[(int)markerType]++;
                break;
            case eSkillEventMarkerType.InvokeTransition:
                MoveToSpecificDestData transitionData = SkillConfig.TransitionData.MoveToSpecificDest[eventIndex];
                mSkillEventIndex[(int)markerType]++;
                if (OWNER.IsPlayerActor)
                {
                    mAttackDir = getAttackDir(out mAssistedPos);

                    if (mAssistedPos != Vector3.zero && transitionData.NeedAssistedDestSubtraction)
                    {
                        transitionData.DestDistance = (mRB.position - mAssistedPos).magnitude;
                        transitionData.DestDistance -= transitionData.AssistedDestSubtraction;
                        transitionData.DestDistance = Mathf.Clamp(transitionData.DestDistance, 0.0f, transitionData.DestDistance);
                    }
                }

                else if(!OWNER.IsPlayerActor && transitionData.TargetingPlayerIfInRange)
                {
                    bool result = tryGetAttackDirTowardPlayer(transitionData.DestDistance, out Vector3 attackDir, out Vector3 assistedPos);
                    if(result)
                    {
                        mAttackDir = attackDir;
                        mAssistedPos = assistedPos;
                        transitionData.DestDistance = (mRB.position - mAssistedPos).magnitude;
                        transitionData.DestDistance -= transitionData.PlayerTagettingSubstraction;
                        transitionData.DestDistance = Mathf.Clamp(transitionData.DestDistance, 0.0f, transitionData.DestDistance);
                    }
                }


                mTransitionTimer
                    .ChangeDuration(transitionData.Duration * (1 / SkillConfig.RuntimeAttackSpeedMultiplier))
                    .ChangeTimerStartCallback(() => setSpeedByDestAndDuration(mAttackDir, transitionData.DestDistance, transitionData.Duration * (1 / SkillConfig.RuntimeAttackSpeedMultiplier), transitionData.Dir))
                    .ChangeTimerEndCallback(() => mRB.EnrollSetVelocity(Vector3.zero, ActorType))
                    .StartTimer();
                break;
            case eSkillEventMarkerType.SetComboInputWait:
                Debug.Assert(SkillConfig.IsComboAttack, $"You Cannot SetComboInputWait Event not comboskillConfig [{SkillConfig.name}]");
                break;
            case eSkillEventMarkerType.InvokeCameraActing:
                CameraManager.Instance.Actor.ProcessCameraActing(SkillConfig.CameraActingOnUsingSkill[eventIndex]);
                mSkillEventIndex[(int)markerType]++;
                break;
            case eSkillEventMarkerType.PowerOverwalrming:
                if (eventIndex % 2 == 0)
                {
                    OWNER.OwnerCharacterBase.CharacterStatus.IsPowerOverwalming.EnrollValue(CharacterStatus.ePowerOverwalmingSetter.SetBySkill, true);
                }
                else
                {
                    OWNER.OwnerCharacterBase.CharacterStatus.IsPowerOverwalming.DisEnrollValue(CharacterStatus.ePowerOverwalmingSetter.SetBySkill);
                }

                mSkillEventIndex[(int)markerType]++;
                break;
            default:
                Debug.Assert(false, $"Default Switch Detected [{typeof(eSkillEventMarkerType)}], [{markerType}]");
                break;
        }
    }

    protected void handleAttackBox(MeleeAttackData attackData)
     {
        AttackBoxElement clonedAttackBox = null;
        Vector3 playerTrans = OWNER.OwnerCharacterBase.Translator.Trans.position;
        Quaternion playerRot = OWNER.OwnerCharacterBase.Translator.Trans.rotation;

        switch (attackData.Option.SpawnType)
        {
            case eAttackboxSpawnType.Default:
                clonedAttackBox = LOCAL_ATTACK_BOX_POOL.GetGameobject(attackData.AttackCollider);
                clonedAttackBox.transform.localPosition = attackData.AttackCollider.transform.localPosition;
                break;
            case eAttackboxSpawnType.SpawnInProjectile:
                clonedAttackBox = PROJECTILE_POOL.GetGameobject(attackData.AttackCollider);
                clonedAttackBox.transform.position = playerTrans;
                clonedAttackBox.transform.rotation = Quaternion.LookRotation(mAttackDir);
                break;
            case eAttackboxSpawnType.SpawnInGlobalAttackBox:
                clonedAttackBox = GLOBAL_ATTACK_BOX_POOL.GetGameobject(attackData.AttackCollider);
                clonedAttackBox.transform.position = playerTrans;
                clonedAttackBox.transform.rotation = playerRot;
                break;
            default:
                Debug.Assert(false, "Default switch detected", OWNER.OwnerCharacterBase.gameObject);
                break;
        }

        if(attackData.SpawnInShootingPoint)
        {
            clonedAttackBox.transform.position = mProjectileShootingPoint.position;
        }

        setKnockbackDir(ref attackData.OnHitEffect.KnockbackData, mAttackDir);
        clonedAttackBox.SetAttackBoxData(mDamage * SkillConfig.DamagePercentage, SkillConfig.TargetTag ,attackData.Option, attackData.OnHitEffect,mOwnerHitBox, mOnAttackBoxHit, OWNER.OwnerCharacterBase);
        attackData.OnHitEffect.CountableType = SkillConfig.CountableType;

        if (attackData.ColliderRemainTime != 0.0f)
        {
            clonedAttackBox.SetColliderLifetime(attackData.ColliderRemainTime);
        }
        mSpawnedAttackBoxes.Add(clonedAttackBox);
        clonedAttackBox.gameObject.SetActive(true);
    }

    protected ParticleBinder spawnFX(FXSpawnData fxInfo)
    {
        return spawnFX(fxInfo, mAttackDir);
    }

    protected ParticleBinder spawnFX(FXSpawnData fxInfo, Vector3 fxDir)
    {
        ParticleBinder particleBinder = fxInfo.EffectPrefab.GetComponent<ParticleBinder>();
        ParticleBinder cloneObject = EFFECT_POOL.GetGameobject(particleBinder);
        if (fxInfo.SpawnInShootingPoint)
        {
            cloneObject.SetFXTransformType(fxInfo.FXTransformType, mRB.position, Quaternion.LookRotation(fxDir), mTransform, mProjectileShootingPoint.localPosition);
        }
        else
        {
            cloneObject.SetFXTransformType(fxInfo.FXTransformType, mRB.position, Quaternion.LookRotation(fxDir), mTransform);
        }
        cloneObject.ScaleFX(fxInfo.SizeMultiplier);
        mSpawnedFX.Add(cloneObject);
        updateAttackSpeed();
        return cloneObject;
    }

    private void handlePointSpecifyAttack(PointSpecifyAttackData attackData)
    {
        if(attackData.IsMultiSpawn)
        {
            for (int i = 0; i < attackData.MultiSpawnCount; i++)
            {
                instancePointSpecifyAttackbox(attackData);
            }
        }
        else
        {
            instancePointSpecifyAttackbox(attackData);
        }
    }

    private void handleAttackTransition(AttackTransitionData data)
    {
        if (data.ActionType == eAttackTransitionType.None)
        {
            return;
        }

        switch (data.ActionType)
        {
            case eAttackTransitionType.SetDecellationFromCurrentSpeed:
                mRB.GetLayeredRigidbody().SetVelocityMultiplier(data.DecellationAmount, eSpeedMultiplierSource.Skill);
                break;
            case eAttackTransitionType.MoveToSpecificDest:
            case eAttackTransitionType.Pause:
                mRB.EnrollSetVelocity(Vector3.zero, ActorType);
                break;
            case eAttackTransitionType.RushToPlayer:
                mRB.EnrollSetVelocity(Vector3.zero, ActorType);

                mTrackPlayerUpdateTimer
                    .ChangeTimerUpdateCallback(rushToPlayer)
                    .ChangeDuration(1000.0f)
                    .StartTimer();
                break;
            case eAttackTransitionType.LookPlayer:
                mRB.EnrollSetVelocity(Vector3.zero, ActorType);

                mTrackPlayerUpdateTimer
                    .ChangeTimerUpdateCallback(lookPlayer)
                    .ChangeDuration(1000.0f)
                    .StartTimer();
                break;
            case eAttackTransitionType.ConstantRotate:
                mRB.EnrollSetVelocity(Vector3.zero, ActorType);

                mTrackPlayerUpdateTimer
                    .ChangeTimerUpdateCallback(constantRotate)
                    .ChangeDuration(1000.0f)
                    .StartTimer();
                break;
            default:
                break;
        }
        return;
    }

    private void setSpeedByDestAndDuration(Vector3 attackDir, float dest, float duration, MoveToSpecificDestData.eDir dir)
    {
        Vector3 moveDir = attackDir;
        switch (dir)
        {
            case MoveToSpecificDestData.eDir.Front:
                break;
            case MoveToSpecificDestData.eDir.Left:
                moveDir = Quaternion.Euler(new Vector3(0.0f, 90.0f, 0.0f)) * attackDir;
                break;
            case MoveToSpecificDestData.eDir.Right:
                moveDir = Quaternion.Euler(new Vector3(0.0f, -90.0f, 0.0f)) * attackDir;
                break;
            default:
                break;
        }
        float speed = dest / duration;
        Vector3 velocity = moveDir * speed;
        mRB.EnrollSetVelocity(velocity, ActorType);
    }

    protected void handleProjectileAttackData(ProjectileAttackData attackData)
    {
        if (OWNER.IsPlayerActor)
        {
            mAttackDir = getAttackDir(out mAssistedPos); // in case player actor using skill actor updaet attack dir but monster is optonable
        }
        setKnockbackDir(ref attackData.OnHitEffect.KnockbackData, mAttackDir);

        if (attackData.IsMultiShot)
        {
            mProjectileMultiShotTimer.ClearAllTiming();
            for (int i = 0; i < attackData.ShootingAmount; i++)
            {
                Vector3 shootDir = mAttackDir;
                switch (attackData.MultiShotAiming)
                {
                    case ProjectileAttackData.eMultiShotAimingType.NoneChangeOnFirstShoot:
                        break;
                    case ProjectileAttackData.eMultiShotAimingType.TrackingPerShoot:
                        break;
                    case ProjectileAttackData.eMultiShotAimingType.ConstantRotate:
                        if(i == 0)
                        {
                            mAttackDir = Quaternion.Euler(0.0f, -attackData.InitialDegreeOffset, 0.0f) * mAttackDir;
                            shootDir = mAttackDir;
                        }
                        else
                        {
                            Quaternion newAim = Quaternion.Euler(0.0f, attackData.RotateDegreePerShoot * i, 0.0f);
                            shootDir = newAim * mAttackDir;
                        }
                        break;
                    default:
                        Debug.LogError(attackData.MultiShotAiming);
                        break;
                }
                float timing = i == 0 ? 0.0f : attackData.DelayBetweenShooting * SkillConfig.RuntimeAttackSpeedMultiplier;
                mProjectileMultiShotTimer.AddCallback(getMultishotChainedTimerTag(i), timing,
                    () => handleProjectilePerShoot(attackData, shootDir));
            }
            mProjectileMultiShotTimer.StartTimer();
        }
        else
        {
            handleProjectilePerShoot(attackData, mAttackDir);
        }
    }

    private void handleProjectilePerShoot(ProjectileAttackData attackData, Vector3 attackDir)
    {
        if(!attackData.MuzzleFX.IsNull())
        {
            spawnFX(attackData.MuzzleFX, attackDir);
        }
        switch (attackData.ShootingType)
        {
            case ProjectileAttackData.eShootingType.OneShot:
                instanceProjectileAndShoot(attackData, attackDir, SkillConfig.TargetTag);
                break;
            case ProjectileAttackData.eShootingType.ShotGun:
                if (attackData.ShotgunAmount == 0)
                {
                    Debug.LogWarning("ShotgunAmount is set as 0");
                    return;
                }
                float intialRot = attackData.ShotgunAmount == 1 ? 0.0f : -attackData.ShotgunAngle / 2;
                float rotAmountPerShoot = attackData.ShotgunAmount == 1 ? 0.0f : attackData.ShotgunAngle / (attackData.ShotgunAmount - 1);

                for (int i = 0; i < attackData.ShotgunAmount; i++)
                {
                    Quaternion newRot = Quaternion.Euler(0.0f, intialRot + rotAmountPerShoot * i, 0.0f);
                    Vector3 newAttackDir = newRot * attackDir;
                    instanceProjectileAndShoot(attackData, newAttackDir, SkillConfig.TargetTag);
                }
                break;
            case ProjectileAttackData.eShootingType.Circular:
                float rotAmount = attackData.CircularAmount == 1 ? 0.0f : 360.0f / attackData.CircularAmount;

                for (int i = 0; i < attackData.CircularAmount; i++)
                {
                    Quaternion newRot = Quaternion.Euler(0.0f, rotAmount * i, 0.0f);
                    Vector3 newAttackDir = newRot * attackDir;
                    instanceProjectileAndShoot(attackData, newAttackDir, SkillConfig.TargetTag);
                }
                break;
            default:
                break;
        }
    }

    private void instanceProjectileAndShoot(ProjectileAttackData attackData, Vector3 attackDir, eTargetTag target)
    {
        ProjectileHandler projectileHandler = PROJECTILE_POOL.GetGameobject(attackData.Projectile);
        attackData.OnHitEffect.CountableType = SkillConfig.CountableType;

        projectileHandler.GetComponent<AttackBoxElement>().SetAttackBoxData(mDamage * SkillConfig.DamagePercentage, SkillConfig.TargetTag, attackData.AttackBoxOption, attackData.OnHitEffect, mOwnerHitBox, mOnAttackBoxHit, OWNER.OwnerCharacterBase);
        Debug.Assert(projectileHandler != null, $"Projectile Gameobject without projectile element detected [{projectileHandler.name}]");
        Vector3 shootingPoint = mProjectileShootingPoint.localPosition;
        shootingPoint = Quaternion.LookRotation(attackDir) * shootingPoint;
        shootingPoint += OWNER.Translator.Trans.position;
        projectileHandler.InitializeProjectile(attackDir, target, attackData.ProjectileInfo, shootingPoint);
    }

    private void instancePointSpecifyAttackbox(PointSpecifyAttackData attackData)
    {
        AttackBoxElement clonedAttackBox = null;
        Vector3 playerTrans = OWNER.OwnerCharacterBase.Translator.Trans.position;
        Quaternion playerRot = OWNER.OwnerCharacterBase.Translator.Trans.rotation;
        Vector3 destPos = Vector3.zero;
        Quaternion destRotation = Quaternion.identity;

        switch (attackData.SpawnPosition)
        {
            case ePointSpecifyAttackSpawnType.SpreadRandomCircle:
                clonedAttackBox = GLOBAL_ATTACK_BOX_POOL.GetGameobject(attackData.AttackCollider);
                Vector3 randomPoint = Random.insideUnitCircle;
                randomPoint = new Vector3(randomPoint.x, 0.0f, randomPoint.y);
                randomPoint = randomPoint * Random.Range(1.0f, attackData.SpreadRadius);
                destPos = OWNER.OwnerCharacterBase.Translator.Trans.position + randomPoint;
                break;
            case ePointSpecifyAttackSpawnType.TargetPlayer:
                clonedAttackBox = GLOBAL_ATTACK_BOX_POOL.GetGameobject(attackData.AttackCollider);
                destPos = PlayerManager.Instance.AcitvatedPlayerTrans.position;
                destRotation = Quaternion.LookRotation(mAttackDir);
                break;
            case ePointSpecifyAttackSpawnType.TargetAimAssistance:
                clonedAttackBox = GLOBAL_ATTACK_BOX_POOL.GetGameobject(attackData.AttackCollider);
                if (mAssistedPos != Vector3.zero)
                {
                    destPos = mAssistedPos;
                }
                else
                {
                    destPos = OWNER.OwnerCharacterBase.Translator.Trans.position + attackData.MaxRange * OWNER.OwnerCharacterBase.Translator.Trans.forward;
                }
                break;
            default:
                Debug.Assert(false, "Default switch detected");
                break;
        }

        if (attackData.Option.IsThrowingMeleebox)
        {
            attackData.Option.ThrowingDest = destPos;
            clonedAttackBox.transform.position = OWNER.Translator.Trans.position;
            clonedAttackBox.transform.rotation = destRotation;
        }
        else
        {
            clonedAttackBox.transform.position = destPos;
            clonedAttackBox.transform.rotation = destRotation;
        }
            


        attackData.OnHitEffect.CountableType = SkillConfig.CountableType;
        setKnockbackDir(ref attackData.OnHitEffect.KnockbackData, mAttackDir);

        clonedAttackBox.SetAttackBoxData(mDamage * SkillConfig.DamagePercentage, SkillConfig.TargetTag, attackData.Option, attackData.OnHitEffect, mOwnerHitBox, mOnAttackBoxHit, OWNER.OwnerCharacterBase);
        if (attackData.UseIndicator)
        {
            clonedAttackBox.BindingIndicator(attackData.IndicatingDuration, attackData.ColliderRemainTime);
        }
        else
        {
            clonedAttackBox.SetColliderLifetime(attackData.ColliderRemainTime);
        }
        clonedAttackBox.gameObject.SetActive(true);
    }

    private string getMultishotChainedTimerTag(int shootCount)
    {
        return "Shoot" + shootCount;
    }

    private void rushToPlayer(float normalizedTime, float timePass)
    {
        Debug.Assert(!OWNER.IsPlayerActor, "this function is for monster not player");
        AttackTransitionData data = SkillConfig.TransitionData;

        if (timePass < data.RushDelay * SkillConfig.PreparingDurationMultiplier)
        {
            Vector3 enemeyToPlayer = PlayerManager.Instance.AcitvatedPlayerTrans.position - mRB.position;
            mRB.EnrollLookRotation(enemeyToPlayer, ActorType);
        }
        else
        {
            Vector3 towardFromVector = mRB.velocity;
            if (mRB.velocity == Vector3.zero)
            {
                towardFromVector = mRB.GetForward();
            }
            Vector3 towardVector = PlayerManager.Instance.GetRotateTowardVectorToPlayer(towardFromVector.normalized, mRB.position, data.TrackRotatingSpeed);
            mRB.EnrollLookRotation(towardVector, ActorType);
            mRB.EnrollSetVelocity(towardVector * data.RushSpeed, ActorType);
        }
    }

    private void lookPlayer(float normalizedTime, float timePass)
    {
        if(CurrentProgressState == eSkillProgressState.Cancelable)
        {
            return;
        }

        Debug.Assert(!OWNER.IsPlayerActor, "this function is for monster not player");
        AttackTransitionData data = SkillConfig.TransitionData;
        Vector3 enemyToPlayer = PlayerManager.Instance.AcitvatedPlayerTrans.position - mTransform.position;
        enemyToPlayer.y = 0.0f;
        enemyToPlayer = Quaternion.Euler(new Vector3(0.0f, data.OffsetAngle, 0.0f)) * enemyToPlayer;
        mRB.EnrollLookRotation(enemyToPlayer, ActorType);
        mRB.GetLayeredRigidbody().SetRotationSpeed(data.TrackRotatingSpeed);
    }

    private void constantRotate(float normalizedTime, float timePass)
    {
        if (CurrentProgressState == eSkillProgressState.Preparing || CurrentProgressState == eSkillProgressState.Cancelable)
        {
            return;
        }

        AttackTransitionData data = SkillConfig.TransitionData;
        Vector3 forward = OWNER.Translator.Trans.forward;
        Vector3 toward = Quaternion.Euler(new Vector3(0.0f, data.RotatePerSecond * Time.fixedDeltaTime, 0.0f)) * forward;
        mRB.EnrollLookRotationAndForceRotating(toward, ActorType);
    }

    private Vector3 getAttackDir(out Vector3 assistedPos)
    {
        Vector3 attackDir = Vector3.zero;
        assistedPos = Vector3.zero;
        if(SkillConfig.RotType == eSkillRotateType.Fix)
        {
            return OWNER.Translator.Trans.forward;
        }
        if (OWNER.IsPlayerActor)
        {
            attackDir = InputManager.Instance.GetAttackAim(mTransform);
            if(SkillConfig.UsingAimAssistance)
            {
                eInputDeviceType inputDevice = InputManager.Instance.LoadedInputHandler.CurrentInputDevice.Value;
                AimAssistanceConfig aimAssistanceConfig = SkillConfig.AimAisstanceConfig;
                switch (inputDevice)
                {
                    case eInputDeviceType.KeyboardAndMouse:
                        break;
                    case eInputDeviceType.GamePad:
                        if(SkillConfig.ConosleAimAssistance != null)
                        {
                            aimAssistanceConfig = SkillConfig.ConosleAimAssistance;
                        }
                        break;
                    case eInputDeviceType.Mobile:
                        break;
                    default:
                        break;
                }

                AimHelper.TryToGetAimAssist(ref attackDir, out assistedPos, mTransform.position, aimAssistanceConfig);
            }
        }
        else
        {
            attackDir = PlayerManager.Instance.AcitvatedPlayerTrans.position - mTransform.position;
            attackDir.Set(attackDir.x, 0.0f, attackDir.z);
            attackDir.Normalize();
        }
        return attackDir;
    }

    private bool tryGetAttackDirTowardPlayer(float transitionDistance, out Vector3 attackDir, out Vector3 assistedPos)
    {
        assistedPos = PlayerManager.Instance.AcitvatedPlayerTrans.position;
        attackDir = Vector3.zero;

        Vector3 playerToEnemy = PlayerManager.Instance.AcitvatedPlayerTrans.position - mTransform.position;
        float sqrDistance = playerToEnemy.sqrMagnitude;
        if(sqrDistance > transitionDistance * transitionDistance)
        {
            return false;
        }

        attackDir = playerToEnemy.normalized;
        return true;
    }

    private void setKnockbackDir(ref KnockbackData data, Vector3 attackDir)
    {
        switch (data.KnockbackType)
        {
            case eKnockbackType.PushToAttackDir:
                data.SetKnockbackDir(attackDir);
                break;
            case eKnockbackType.CircularToHitPoint:
                break;
            default:
                Debug.LogError($"Default switch detected [{data.KnockbackType}]");
                break;
        }
    }

    private void updateAttackSpeed()
    {
        float attackSpeed = 1.0f;
        switch (CurrentProgressState)
        {
            case eSkillProgressState.None:
                break;
            case eSkillProgressState.Preparing:
                attackSpeed = (1 / SkillConfig.PreparingDurationMultiplier);
                break;
            case eSkillProgressState.OnAttack:
                attackSpeed = (1 / SkillConfig.AttackDurationMultiplier);
                break;
            case eSkillProgressState.Cancelable:
                attackSpeed = (1 / SkillConfig.CancleableDurationMultiplier);
                break;
            default:
                Debug.LogError($"Default switch detected [{CurrentProgressState}]");
                break;
        }

        if (mHitStopTimer != null && mHitStopTimer.IsActivate)
        {
            attackSpeed *= mHitStopSpeedMultiplier;
        }

        Anim.SetAttackSpeed(attackSpeed);
        for (int i = 0; i < mSpawnedFX.Count; i++)
        {
            mSpawnedFX[i].SetSimulationSpeed(attackSpeed);
        }
    }

    private ParticleBinder spawnIndicator(ParticleBinder indicatorParticle, float duration)
    {
        float preparingDuration = duration < SkillConfig.PreparingDuration ? duration : SkillConfig.PreparingDuration;
        duration -= SkillConfig.PreparingDuration;
        duration = Mathf.Max(duration, 0);

        float attackDuration = duration < SkillConfig.AttackDuration ? duration : SkillConfig.AttackDuration;
        duration -= SkillConfig.AttackDuration;

        float cancleableDuration = duration <= 0 ? 0 : duration;

        float invokeDuration = preparingDuration * SkillConfig.PreparingDurationMultiplier 
            + attackDuration * SkillConfig.AttackDurationMultiplier 
            + cancleableDuration * SkillConfig.CancleableDurationMultiplier;

        ParticleBinder clonedParticle = EFFECT_POOL.GetGameobject(indicatorParticle);
        clonedParticle.SetSimulationSpeed(1 / invokeDuration);
        return clonedParticle;
    }

    private void updateManualSkillEventTiming()
    {
        if (SkillConfig.ManualSkillEvents.Length == mCurrentManualSkillEventIndexes)
        {
            return;
        }

        ManualSkillEvent waitingEvent = SkillConfig.ManualSkillEvents[mCurrentManualSkillEventIndexes];
        while (waitingEvent.InvokeTiming <= Anim.NormalizeDurationCurrentAnim)
        {
            invokeSkillEvent(waitingEvent.EventType);
            mCurrentManualSkillEventIndexes++;
            if (SkillConfig.ManualSkillEvents.Length == mCurrentManualSkillEventIndexes)
            {
                break;
            }

            waitingEvent = SkillConfig.ManualSkillEvents[mCurrentManualSkillEventIndexes];
        }
    }

    private bool checkSkillEventExcutable(eSkillEventMarkerType target)
    {
        switch (target)
        {
            case eSkillEventMarkerType.InvokeSkillFX:
                return mSkillEventIndex[(int)target] < SkillConfig.FXSpawnData.Length;
            case eSkillEventMarkerType.InvokeAttackBox:
                return mSkillEventIndex[(int)target] < SkillConfig.MeleeAttackData.Length;
            case eSkillEventMarkerType.InvokeProjectile:
                return mSkillEventIndex[(int)target] < SkillConfig.ProjectileData.Length;
            case eSkillEventMarkerType.InvokeSound:
                return mSkillEventIndex[(int)target] < SkillConfig.AttackSound.Length;
            case eSkillEventMarkerType.InvokeTransition:
                return mSkillEventIndex[(int)target] < SkillConfig.TransitionData.MoveToSpecificDest.Length;
            case eSkillEventMarkerType.InvokeCameraActing:
                return mSkillEventIndex[(int)target] < SkillConfig.CameraActingOnUsingSkill.Length;
        }
        return true;
    }
}

