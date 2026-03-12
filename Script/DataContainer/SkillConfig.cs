using UnityEngine;
using Sirenix.OdinInspector;
using System;
using FMODUnity;
using UnityEditor;
using System.Collections.Generic;
using TMPro;
using System.Reflection;
using System.ComponentModel;
using ReadOnlyAttribute = Sirenix.OdinInspector.ReadOnlyAttribute;

[CreateAssetMenu(menuName = "DataContainer/SkillConfig")]
public class SkillConfig : ActorConfigDataContainerBase
{
    public string GetCooltimeKey
    {
        get
        {
            return name + "CoolTime";
        }
    }

    public float PreparingDurationMultiplier
    {
        get
        {
            return ConvertToSpeedMultiplier(sfPreparingTimeAccel);
        }
    }

    public float AttackDurationMultiplier
    {
        get
        {
            return ConvertToSpeedMultiplier(sfAttackDurationAccel);
        }
    }

    public float CancleableDurationMultiplier
    {
        get
        {
            return ConvertToSpeedMultiplier(sfCancleableDurationAccel);
        }
    }

    [TabGroup("Options", "Common")] public float DamagePercentage;
    [TabGroup("Options", "Common")] public float CoolTime;
    [TabGroup("Options", "Common")] public eSkillRotateType RotType;
    [TabGroup("Options", "Common")] [HideIf("mIsPlayerConfig")] public float CoolTimeRandomRange;
    [TabGroup("Options", "Common")] [HideIf("mIsPlayerConfig")] public float InitialCoolTime;
    [TabGroup("Options", "Common")][HideIf("mIsPlayerConfig")] public bool IsExplositionAttack;
    [TabGroup("Options", "Common")] public eTargetTag TargetTag;
    [TabGroup("Options", "Common")] public bool UsingCameraActing;
    [TabGroup("Options", "Common")] [ShowIf("UsingCameraActing")] public CameraActingEventData[] CameraActingOnUsingSkill;

    [TabGroup("Options", "MotionAndTiming")] [AssetsOnly] public AnimationClip AttackMotion;
    [TabGroup("Options", "MotionAndTiming")] [ShowInInspector] [HideIf("@AttackMotion == null")]
    [LabelText("Original Duration")]
    private float mMotionDuration
    {
        get
        {
            return AttackMotion == null ? 0 : AttackMotion.length;
        }
    }

    [TabGroup("Options", "MotionAndTiming"),
        ShowInInspector, 
        ReadOnly, 
        HideIf("@AttackMotion == null"), LabelText("Preparing Duration")]
    public float PreparingDuration
    {
        get; private set;
    }

    [TabGroup("Options", "MotionAndTiming"), ShowInInspector, ReadOnly, HideIf("@AttackMotion == null"), LabelText("Attack Duration")]
    public float AttackDuration
    {
        get; private set;
    }

    [TabGroup("Options", "MotionAndTiming"), ShowInInspector, ReadOnly, HideIf("@AttackMotion == null"), LabelText("Cancleable Duration")]
    public float CancleableDuration
    {
        get; private set;
    }

    [TabGroup("Options", "MotionAndTiming")] [SerializeField, Range(-1, 1)] private float sfPreparingTimeAccel;
    [TabGroup("Options", "MotionAndTiming")] [SerializeField, Range(-1, 1)] private float sfAttackDurationAccel;
    [TabGroup("Options", "MotionAndTiming")] [SerializeField, Range(-1, 1)] private float sfCancleableDurationAccel;
    [TabGroup("Options", "MotionAndTiming")] [PropertyOrder(10)] public ManualSkillEvent[] ManualSkillEvents;

    //ComboInfo

    [TabGroup("PlayerOption", "PlayerOption", TabLayouting= TabLayouting.Shrink, VisibleIf = "mIsPlayerConfig")] public bool UsingAimAssistance;
    [TabGroup("PlayerOption", "PlayerOption")] [ShowIf("UsingAimAssistance")] public AimAssistanceConfig AimAisstanceConfig;
    [TabGroup("PlayerOption", "PlayerOption")] [ShowIf("UsingAimAssistance")] public AimAssistanceConfig ConosleAimAssistance;

    [TabGroup("PlayerOption", "PlayerOption")] public bool UsingBulletTimeOnSkillInvoked;
    [TabGroup("PlayerOption", "PlayerOption")] [ShowIf("UsingBulletTimeOnSkillInvoked"), HideLabel] public BulletTimeData BulletTimeOnUsingSkill;
    [TabGroup("PlayerOption", "ComboAttack")] [ToggleGroup("PlayerOption/ComboAttack/IsComboAttack", "EnableComboAttack")] public bool IsComboAttack;
    [ToggleGroup("PlayerOption/ComboAttack/IsComboAttack", "EnableComboAttack")] [SerializeField] private bool sfIsFirstAttack;
    [ToggleGroup("PlayerOption/ComboAttack/IsComboAttack", "EnableComboAttack")] [ShowIf("mIsFirstAttack")] public int EnabledComboCount;
    [ToggleGroup("PlayerOption/ComboAttack/IsComboAttack", "EnableComboAttack")] public float CombableTime;
    [ToggleGroup("PlayerOption/ComboAttack/IsComboAttack", "EnableComboAttack")] [ReadOnly] public string ComboSkillName;
    [ToggleGroup("PlayerOption/ComboAttack/IsComboAttack", "EnableComboAttack")] [ReadOnly] public int ComboIndex;
    [ToggleGroup("PlayerOption/ComboAttack/IsComboAttack", "EnableComboAttack")] [ReadOnly] public bool SetWaitComboInputManually;

    [TabGroup("PlayerOption", "ChargeAttack")] [ToggleGroup("PlayerOption/ChargeAttack/IsChargingAttack", "EnableChargingAttack")] public bool IsChargingAttack;
    [ToggleGroup("PlayerOption/ChargeAttack/IsChargingAttack", "EnableChargingAttack")] public float MinChargingTime;
    [ToggleGroup("PlayerOption/ChargeAttack/IsChargingAttack", "EnableChargingAttack")] public float[] ChargeStep;
    [ToggleGroup("PlayerOption/ChargeAttack/IsChargingAttack", "EnableChargingAttack")] public SkillConfigVarient[] ChargeConfig;
    [TabGroup("PlayerOption", "CounterOptions")]  public bool CounterTrySkill;
    [TabGroup("PlayerOption", "CounterOptions")] [ShowIf("CounterTrySkill")] public ForceCounterInvokingBox CounterBox;
    [TabGroup("PlayerOption", "CounterOptions")][HideIf("CounterTrySkill")] public eCountableType CountableType;


    [TabGroup("AttackOption", "MeleeAttack")] public MeleeAttackData[] MeleeAttackData;
    [TabGroup("AttackOption", "PointAttack")] public PointSpecifyAttackData[] PointSpecifyAttackData;
    [TabGroup("AttackOption", "Projectile")] public string ShootingPointTransName = "ShootingPoint";
    [TabGroup("AttackOption", "Projectile")] public ProjectileAttackData[] ProjectileData;
    [TabGroup("AttackOption", "BuffOption")] public BuffData BuffDataOnUsingSkill;
    [TabGroup("AttackOption", "BuffOption")][ShowIf("BuffDataOnUsingSkill")] public float BuffDuration;
    [TabGroup("AttackOption", "BuffOption")] [ShowIf("BuffDataOnUsingSkill")] public float BuffPower;
    [TabGroup("AttackOption", "Chanelling")] public bool IsChannellingSkill;
    [TabGroup("AttackOption", "Chanelling")] public float ChannelingDuration;
    [TabGroup("AttackOption", "Chanelling")] public MeleeAttackData ChannelingAttackData;
    [TabGroup("AttackOption", "Chanelling")] public ProjectileAttackData ChannelingProjectileData;
    [TabGroup("AttackOption", "Chanelling")] public FXSpawnData ChannelingFX;
    [TabGroup("AttackOption", "Chanelling")] public EventReference ChannelingLoopSFX;


    [TabGroup("ExtraOption", "FXSpawnOption")] public FXSpawnData[] FXSpawnData;
    [TabGroup("ExtraOption", "TransitionOption")] public AttackTransitionData TransitionData;
    [TabGroup("ExtraOption", "SoundOption")] public EventReference[] AttackSound;

    private const float M_MAX_ACCELARTION_FACTOR = 3.0f;

    [SerializeField, PropertyOrder(-1000)] private eActorType sfSkilActorType;
    [SerializeField, PropertyOrder(-1000), LabelText("Is For Player")] private bool sfIsPlayerCOnfig;

    [NonSerialized] public float RuntimeAttackSpeedMultiplier = 1.0f;
    [NonSerialized] public List<SkillConfig> RuntimeInstantiatedConfigs;

    public float ConvertToSpeedMultiplier(float inspectorValue)
    {
        Debug.Assert(inspectorValue >= -1 && inspectorValue <= 1, $"accel out of range [{inspectorValue} {name}]");
        inspectorValue = Mathf.Clamp((inspectorValue), -1, 1);
        return SkillUtils.ConvertInspectorAccelToMultiplier(inspectorValue, M_MAX_ACCELARTION_FACTOR) * (1 / RuntimeAttackSpeedMultiplier);
    }

    protected override void initializeConfig()
    {
        BaseConfig.ActorType = sfSkilActorType;
        BaseConfig.IsUpdatedActor = false;

        if (IsChargingAttack || (ManualSkillEvents != null && ManualSkillEvents.Length != 0))
        {
            BaseConfig.IsUpdatedActor = true;
        }
        
        if(IsChannellingSkill)
        {
            BaseConfig.IsUpdatedActor = true;
        }

        if(AttackMotion != null)
        {
            AnimationEvent[] events = AttackMotion.events;
            int progressState = 0;
            for (int i = 0; i < events.Length; i++)
            {
                if (!events[i].IsSkillEvent())
                {
                    continue;
                }
                events[i].GetSkillEventData(out eActorType skillActor, out eSkillEventMarkerType markerType);

                if (markerType != eSkillEventMarkerType.ChangeProgressState)
                {
                    continue;
                }

                switch (progressState)
                {
                    case 0:
                        PreparingDuration = events[i].time;
                        break;
                    case 1:
                        AttackDuration = events[i].time - PreparingDuration;
                        break;
                    case 2:
                        CancleableDuration = events[i].time - PreparingDuration - AttackDuration;
                        break;
                    default:
                        break;
                }
                progressState++;
            }
        }
    }

    private float caculateAccelation(float original, float accel)
    {
        float multplier = ConvertToSpeedMultiplier(accel);
        return original * multplier;
    }

    public SkillConfig GetRuntimeSkillConfig()
    {
#if UNITY_EDITOR
        Debug.Assert(AssetDatabase.Contains(this));
#endif
        SkillConfig runtimeConfig = this.InstantiateWithNotNameChanging();
        if (RuntimeInstantiatedConfigs == null)
        {
            RuntimeInstantiatedConfigs = new List<SkillConfig>();
        }

        RuntimeInstantiatedConfigs.Add(runtimeConfig);
        return runtimeConfig;
    }

#if UNITY_EDITOR
    public void UpdateSkillConfigFromString(string stringDataFormat)
    {
        stringDataFormat = stringDataFormat.Trim();
        string[] keyValuePair = stringDataFormat.SplitWithIgnoreBrace('{', '}', ',');
        FieldInfo[] fieldInfos = typeof(SkillConfig).GetFields();

        for (int i = 0; i < keyValuePair.Length; i++)
        {

            int pairingIndex = keyValuePair[i].IndexOf(':');
            if (pairingIndex < 0)
            {
                continue;
            }
            if (pairingIndex == keyValuePair.Length)
            {
                continue;
            }

            string name = keyValuePair[i].Substring(0, pairingIndex).Trim();
            string value = keyValuePair[i].Substring(pairingIndex + 1).Trim();

            for (int j = 0; j < fieldInfos.Length; j++)
            {
                if(fieldInfos[j].Name == name)
                {
                    var converter = TypeDescriptor.GetConverter(fieldInfos[j].FieldType).ConvertFromString(value);
                    fieldInfos[j].SetValue(this, Convert.ChangeType(converter,  fieldInfos[j].FieldType));
                    continue;
                }
            }
        }
        EditorUtility.SetDirty(this);
    }

    private void OnValidate()
    {
        if (BuildPipeline.isBuildingPlayer)
        {
            return;
        }

        if(IsChannellingSkill)
        {
            return;
        }

        string errorMessage = "";
        if (!sfSkilActorType.IsAttackType())
        {
            errorMessage = $"SkillActorType은 Attack Type만 지정 가능합니다. [{name}]";
            goto returnLabel;
        }

        if (IsComboAttack)
        {
            int placeHolder;
            bool result = int.TryParse(name.Substring(name.IndexOf("@") + 1), out placeHolder);
            if (!result)
            {
                errorMessage = "ComboAttack으로 설정된 SkillConfig의 이름은 항상 이름@콤보로 구성되어야 합니다.";
                goto returnLabel;
            }
        }

        if (AttackMotion == null)
        {
            return;
        }

        string validateResult = validateSkillAnimationEvent();
        if(validateResult != "")
        {
            errorMessage = validateResult;
            goto returnLabel;
        }


        int[] markerCount = getExistSkillEventMarkerCount();
        int[] necessaryMarkerCount = getNecessarySkilleventMarkerCount();

        for (int i = 0; i < markerCount.Length; i++)
        {
            if (necessaryMarkerCount[i] == -1)
            {
                continue;
            }

            if (markerCount[i] != necessaryMarkerCount[i])
            {
                errorMessage = $"[{name}], [{(eSkillEventMarkerType)i}]로 설정된 SkillEvent가 [{necessaryMarkerCount[i]}]개 필요합니다. 현재 [{markerCount[i]}] 개 있음";
                goto returnLabel;
            }
        }

    returnLabel:
        {
            if (errorMessage != "")
            {
                Debug.LogWarning(errorMessage, this);
            }
        }
    }

    private int[] getExistSkillEventMarkerCount()
    {
        const string SKILL_EVENT_FUNCTION_NAME = "SkillEventInvoking";
        AnimationEvent[] events = AttackMotion.events;

        int[] markerCount = new int[Enum.GetValues(typeof(eSkillEventMarkerType)).Length];
        for (int i = 0; i < events.Length; i++)
        {
            AnimationEvent curEvent = events[i];
            if (curEvent.functionName != SKILL_EVENT_FUNCTION_NAME)
            {
                continue;
            }

            if (!curEvent.CheckValidForSkillEvent())
            {
                continue;
            }

            curEvent.GetSkillEventData(out eActorType actorType, out eSkillEventMarkerType markerType);
            if (actorType != sfSkilActorType)
            {
                curEvent.stringParameter = sfSkilActorType.ToString();
                events[i] = curEvent;
            }

            markerCount[(int)markerType]++;
        }


        if(ManualSkillEvents != null)
        {
            for (int i = 0; i < ManualSkillEvents.Length; i++)
            {
                markerCount[(int)ManualSkillEvents[i].EventType]++;
            }
        }

        return markerCount;
    }

    private int[] getNecessarySkilleventMarkerCount()
    {
        int[] markerCount = new int[Enum.GetValues(typeof(eSkillEventMarkerType)).Length];
        for (int i = 0; i < markerCount.Length; i++)
        {
            markerCount[i] = -1;
        }

        markerCount[(int)eSkillEventMarkerType.ChangeProgressState] = 3;

        if (TransitionData.ActionType == eAttackTransitionType.MoveToSpecificDest)
        {
            if (TransitionData.MoveToSpecificDest != null)
            {
                markerCount[(int)eSkillEventMarkerType.InvokeTransition] = TransitionData.MoveToSpecificDest.Length;
            }
        }

        if(FXSpawnData != null)
        {
            markerCount[(int)eSkillEventMarkerType.InvokeSkillFX] = FXSpawnData.Length;
        }

        if(MeleeAttackData != null)
        {
            markerCount[(int)eSkillEventMarkerType.InvokeAttackBox] = MeleeAttackData.Length;
        }

        if (ProjectileData != null)
        {
            markerCount[(int)eSkillEventMarkerType.InvokeProjectile] = ProjectileData.Length;
        }

        if (PointSpecifyAttackData != null)
        {
            markerCount[(int)eSkillEventMarkerType.InvokePointSpecifyAttack] = PointSpecifyAttackData.Length;
        }

        markerCount[(int)eSkillEventMarkerType.InvokeSound] = AttackSound.Length;

        return markerCount;
    }

    private bool tryAutoSetSkillAnimationEventParameter()
    {
        const string SKILL_EVENT_FUNCTION_NAME = "SkillEventInvoking";
        AnimationEvent[] events = AttackMotion.events;

        int[] markerCount = new int[Enum.GetValues(typeof(eSkillEventMarkerType)).Length];
        bool isDirty = false;
        for (int i = 0; i < events.Length; i++)
        {
            AnimationEvent curEvent = events[i];
            if (curEvent.functionName != SKILL_EVENT_FUNCTION_NAME)
            {
                continue;
            }

            if (curEvent.stringParameter == null || curEvent.stringParameter == "" || curEvent.stringParameter != sfSkilActorType.ToString())
            {
                curEvent.stringParameter = sfSkilActorType.ToString();
                events[i] = curEvent;
                isDirty = true;
            }

            if (!curEvent.CheckValidForSkillEvent())
            {
                continue;
            }

        }

        if (isDirty)
        {
            AnimationUtility.SetAnimationEvents(AttackMotion, events);
        }
        return isDirty;
    }

    private string validateSkillAnimationEvent()
    {
        const string SKILL_EVENT_FUNCTION_NAME = "SkillEventInvoking";
        AnimationEvent[] events = AttackMotion.events;
        int[] markerCount = new int[Enum.GetValues(typeof(eSkillEventMarkerType)).Length];
        for (int i = 0; i < events.Length; i++)
        {
            AnimationEvent curEvent = events[i];
            if (curEvent.functionName != SKILL_EVENT_FUNCTION_NAME)
            {
                continue;
            }

            if (!curEvent.CheckValidForSkillEvent())
            {
                continue;
            }

            curEvent.GetSkillEventData(out eActorType actorType, out eSkillEventMarkerType markerType);
            if (!curEvent.CheckValidForSkillEvent())
            {
                return $"[{name}] SkillEvent Marker Assert 실패";
            }

            markerCount[(int)markerType]++;
        }
        return "";
    }

    private void validateChargingAttack()
    {
        if(!IsChargingAttack)
        {
            return;
        }

        if(ChargeStep.Length != ChargeConfig.Length)
        {
            Debug.LogWarning($"Charge 공격은 Charge Step으로 저장된 요소의 개수만큼 ChargeConfig가 필요합니다.", this);
        }
    }

    [Button]
    private void autoAddFlags()
    {
        if(AttackMotion == null)
        {
            return;
        }


        int[] markerCount = getExistSkillEventMarkerCount();
        int[] neccesarryMarkerCount = getNecessarySkilleventMarkerCount();

        System.Collections.Generic.List<AnimationEvent> eventList = new System.Collections.Generic.List<AnimationEvent>();



        bool isDirty = false;
        for (int i = 0; i < markerCount.Length; i++)
        {
            if(neccesarryMarkerCount[i] == -1)
            {
                continue;
            }

            int missingCount = neccesarryMarkerCount[i] - markerCount[i];
            for (int j = 0; j < missingCount; j++)
            {
                isDirty = true;
                const string SKILL_EVENT_FUNCTION_NAME = "SkillEventInvoking";
                AnimationEvent newEvent = new AnimationEvent();
                newEvent.functionName = SKILL_EVENT_FUNCTION_NAME;
                newEvent.stringParameter = sfSkilActorType.ToString();
                newEvent.objectReferenceParameter = SkillAnimEventParameter.GetSkillAnimEventParameter((eSkillEventMarkerType)i);
                if (newEvent.CheckValidForSkillEvent())
                {
                    eventList.Add(newEvent);
                }
            }
        }

        if(isDirty)
        {
            AnimationEvent[] prevEvents = AttackMotion.events;
            for (int i = 0; i < prevEvents.Length; i++)
            {
                eventList.Add(prevEvents[i]);
            }
            AnimationUtility.SetAnimationEvents(AttackMotion, eventList.ToArray());
        }
    }

    [OnInspectorInit]
    private void updateSkillConfigAutosetInfo()
    {
        presettingComboInfo();
        tryAutoSetSkillAnimationEventParameter();
    }

    [OnInspectorGUI]
    private void presettingComboInfo()
    {
        if (!IsComboAttack)
        {
            return;
        }
        bool isDirty = false;
        int parsedComboInex;
        int.TryParse(name.Substring(name.IndexOf("@") + 1), out parsedComboInex);

        if (ComboIndex != parsedComboInex - 1)
        {
            ComboIndex = parsedComboInex - 1;
            isDirty = true;
        }

        string comboSkillName = name.Substring(0, name.IndexOf("@"));
        if (ComboSkillName != comboSkillName)
        {
            ComboSkillName = comboSkillName;
            isDirty = true;
        }

        if (AttackMotion != null)
        {
            AnimationEvent[] events = AttackMotion.events;
            bool setWaitComboInputManually = false;
            for (int i = 0; i < events.Length; i++)
            {
                AnimationEvent curEvent = events[i];
                curEvent.GetSkillEventData(out eActorType actorType, out eSkillEventMarkerType markerType);
                if (markerType == eSkillEventMarkerType.SetComboInputWait)
                {
                    setWaitComboInputManually = true;
                    break;
                }
            }

            if (SetWaitComboInputManually != setWaitComboInputManually)
            {
                SetWaitComboInputManually = setWaitComboInputManually;
                isDirty = true;
            }
        }

        if (isDirty)
        {
            EditorUtility.SetDirty(this);
            AssetDatabase.SaveAssetIfDirty(this);
        }
    }

    [HideInEditorMode, Button, PropertyOrder(-100)]
    private void forceUpdateRuntimeConfigs()
    {
        if(RuntimeInstantiatedConfigs == null)
        {
            return;
        }

        for (int i = 0; i < RuntimeInstantiatedConfigs.Count; i++)
        {
            EditorUtility.CopySerialized(this, RuntimeInstantiatedConfigs[i]);
        }
    }
#endif
}

[Serializable]
public struct ProjectileAttackData
{
    [RequiredIn(PrefabKind.PrefabAsset)] public ProjectileHandler Projectile;
    public eShootingType ShootingType;
    public FXSpawnData MuzzleFX;

    [ShowIf("ShootingType", eShootingType.ShotGun)] public int ShotgunAmount;
    [ShowIf("ShootingType", eShootingType.ShotGun)] public float ShotgunAngle;

    [ShowIf("ShootingType", eShootingType.Circular)] public int CircularAmount;

    [ToggleGroup("IsMultiShot", "Multi shot option")] public bool IsMultiShot;
    [ToggleGroup("IsMultiShot", "Multi shot option")] public int ShootingAmount;
    [ToggleGroup("IsMultiShot", "Multi shot option")] public float DelayBetweenShooting;
    [ToggleGroup("IsMultiShot", "Multi shot option")] public eMultiShotAimingType MultiShotAiming;

    [ToggleGroup("IsMultiShot", "Multi shot option"), ShowIf("MultiShotAiming", eMultiShotAimingType.ConstantRotate)] 
    public float RotateDegreePerShoot;
    [ToggleGroup("IsMultiShot", "Multi shot option"), ShowIf("MultiShotAiming", eMultiShotAimingType.ConstantRotate)]
    public float InitialDegreeOffset;



    public HitEffectData OnHitEffect;
    public AttackBoxOption AttackBoxOption;
    public ProjectileInfo ProjectileInfo;
    public bool UseIndicator;
    [ShowIf("UseIndicator")] public ParticleBinder IndicatorParticle;
    [ShowIf("UseIndicator")] public eFXTransformType IndicatorFXTransformType;

    public bool IsNull()
    {
        return Projectile == null;
    }

    public enum eShootingType
    {
        OneShot,
        ShotGun,
        Circular,
    }

    public enum eMultiShotAimingType
    {
        NoneChangeOnFirstShoot,
        TrackingPerShoot,
        ConstantRotate
    }
}

[Serializable]
public struct MeleeAttackData
{
    [RequiredIn(PrefabKind.PrefabAsset)] public AttackBoxElement AttackCollider;
    public float ColliderRemainTime;
    public bool SpawnInShootingPoint;
    public AttackBoxOption Option;
    public HitEffectData OnHitEffect;
    [ToggleGroup("UseIndicator")] public bool UseIndicator;
    [ToggleGroup("UseIndicator")] public ParticleBinder IndicatorParticle;
    [ToggleGroup("UseIndicator")] public eFXTransformType IndicatorFXTransformType;

    public bool IsNull()
    {
        return AttackCollider == null;
    }
}

[Serializable]
public struct PointSpecifyAttackData
{
    [RequiredIn(PrefabKind.PrefabAsset)] public AttackBoxElement AttackCollider;
    public float ColliderRemainTime;
    public AttackBoxOption Option;
    public HitEffectData OnHitEffect;
    public ePointSpecifyAttackSpawnType SpawnPosition;
    [ShowIf("SpawnPosition", ePointSpecifyAttackSpawnType.TargetAimAssistance)] public float MaxRange;
    [ShowIf("SpawnPosition", ePointSpecifyAttackSpawnType.SpreadRandomCircle)] public float SpreadRadius;
    [ToggleGroup("IsMultiSpawn")] public bool IsMultiSpawn;
    [ToggleGroup("IsMultiSpawn")] public int MultiSpawnCount;
    [ToggleGroup("UseIndicator")] public bool UseIndicator;
    [ToggleGroup("UseIndicator")] public float IndicatingDuration;

    public bool IsNull()
    {
        return AttackCollider == null;
    }
}

[Serializable]
public struct HitEffectData
{
    public eHitEffectType HitffectType;
    [ShowIf("HitffectType", eHitEffectType.Knockback)] public KnockbackData KnockbackData;
    [ShowIf("HitffectType", eHitEffectType.Deaccelerate)] public float Deacceleration;
    public float DamagedDuration;

    [ToggleGroup("UseCameraActing")] public bool UseCameraActing;
    [ToggleGroup("UseCameraActing")] public CameraActingEventData CameraActingOnHit;
    [ToggleGroup("UseCameraActing")] public int CameraActInvokingHitCountThreshold;

    [ToggleGroup("UseBulletTime")] public bool UseBulletTime;
    [ToggleGroup("UseBulletTime")] public BulletTimeData BulletTime;
    [ToggleGroup("UseBulletTime")] public int BulletTimeInvokingHitCountThreshold;

    [ToggleGroup("UseHitStop")] public bool UseHitStop;
    [ToggleGroup("UseHitStop")] public HitStopData HitStopOption;


    [ToggleGroup("GiveDebuff")] public bool GiveDebuff;
    [ToggleGroup("GiveDebuff")] public BuffData BuffDataOnHitOrNull;
    [ToggleGroup("GiveDebuff")] public float BuffDuration;
    [ToggleGroup("GiveDebuff")] public float BuffPower1;
    [ToggleGroup("GiveDebuff")] public float BuffPower2;
    [ToggleGroup("GiveDebuff")] public float BuffPower3;
    [ToggleGroup("GiveDebuff")] public int BuffStackPerHit;

    [AssetsOnly] public ParticleBinder OnHitFX;
    public EventReference OnHitSound;
    public TMP_ColorGradient DamagedTextColorGradientOrNull;

    [NonSerialized] public Vector3 AttackerPosition;
    [NonSerialized] public Vector3 ColliderPostiion;
    [NonSerialized] public eCountableType CountableType;

    public static HitEffectData GetCounterHitEffectData()
    {
        HitEffectData hitEffectData = new HitEffectData();
        hitEffectData.HitffectType = eHitEffectType.Pause;
        hitEffectData.DamagedDuration = 1.0f;
        return hitEffectData;
    }

    public static HitEffectData GetDebuffDamageHitEffectData()
    {
        HitEffectData hitEffectData = new HitEffectData();
        hitEffectData.HitffectType = eHitEffectType.None;
        return hitEffectData;
    }

    public static HitEffectData GetTickForForceCounterInvoking()
    {
        HitEffectData hitEffectData = new HitEffectData();
        hitEffectData.CountableType = eCountableType.EasyCountable;
        return hitEffectData;
    }
}

[Serializable]
public struct KnockbackData
{
    public eKnockbackType KnockbackType;
    [ShowIf("@KnockbackType == eKnockbackType.PushToAttackDir || KnockbackType == eKnockbackType.CircularToHitPoint")] public float NockbackPower;
    [ShowIf("@KnockbackType == eKnockbackType.PushToAttackDir || KnockbackType == eKnockbackType.CircularToHitPoint")] public float NockbackTime;
    [ShowIf("@KnockbackType == eKnockbackType.PushToAttackDir || KnockbackType == eKnockbackType.CircularToHitPoint")] public int BilladableCount;
    [ShowIf("KnockbackType", eKnockbackType.CircularToHitPoint)]
    public bool IsInverseDir;
    [ShowIf("@KnockbackType == eKnockbackType.PushToAttackDir || KnockbackType == eKnockbackType.CircularToHitPoint")]
    [AssetsOnly]
    public ParticleBinder NockBackFX;



    public Vector3 KnockbackDir
    {
        get; private set;
    }

    public void SetKnockbackDir(Vector3 dir)
    {
        KnockbackDir = dir;
    }

#if UNITY_EDITOR
    private void InitDefault()
    {
        NockBackFX = GlobalSkillConfig.Instance.DefaultNockbackFX;
    }

#endif
}

[Serializable]
public struct AttackTransitionData
{
    public eAttackTransitionType ActionType;
    [ShowIf("ActionType", eAttackTransitionType.SetDecellationFromCurrentSpeed), Range(0, 1)] public float DecellationAmount;
    [ShowIf("ActionType", eAttackTransitionType.MoveToSpecificDest)] public MoveToSpecificDestData[] MoveToSpecificDest;
    [ShowIf("ActionType", eAttackTransitionType.RushToPlayer)] public float RushDelay;
    [ShowIf("ActionType", eAttackTransitionType.RushToPlayer)] public float RushSpeed;
    [ShowIf("ActionType", eAttackTransitionType.RushToPlayer), ShowIf("ActionType", eAttackTransitionType.LookPlayer), Range(0.0f, 20.0f)] public float TrackRotatingSpeed;
    [ShowIf("ActionType", eAttackTransitionType.RushToPlayer), ShowIf("ActionType", eAttackTransitionType.LookPlayer)] public float OffsetAngle;
    [ShowIf("ActionType", eAttackTransitionType.ConstantRotate)] public float RotatePerSecond;
}

[Serializable]
public struct MoveToSpecificDestData
{
    public enum eDir
    {
        Front,
        Left,
        Right,
    }

    public float DestDistance;
    public float Duration;
    public eDir Dir;
    public bool NeedAssistedDestSubtraction;
    [InfoBox("이동 데이터를 가진 공격이 보정되어 몬스터를 타게팅 할 경우, 몬스터의 위치 보다 해당 값만큼 더 뒤로 떨어진 위치를 공격지점으로 삼습니다.")
    , ShowIf("NeedAssistedDestSubtraction")]
    public float AssistedDestSubtraction;

    [InfoBox("몬스터의 경우 플레이어가 이동거리 안에 있는 경우 플레이어를 타게팅 할 수 있는 옵션입니다.")]
    public bool TargetingPlayerIfInRange;
    [ShowIf("TargetingPlayerIfInRange")]
    public float PlayerTagettingSubstraction;
}

[Serializable]
public struct CameraActingEventData
{
    public eCameraActingType ActingType;

    [ShowIf("@ActingType.HasFlag(eCameraActingType.Shake)"), OnValueChanged("updateCameraShakeCurve")] public eShakeType ShakeType;
    [ShowIf("@ActingType.HasFlag(eCameraActingType.Shake)")] public float ShakeDuration;
    [ShowIf("@ActingType.HasFlag(eCameraActingType.Shake)")] public float ShakePower;
    [ShowIf("@ActingType.HasFlag(eCameraActingType.Shake)")] public float ShakeFrequency;
    [ShowIf("@ActingType.HasFlag(eCameraActingType.Shake)"), ShowIf("ShakeType", eShakeType.Custom)] public AnimationCurve ShakeCurve;

    [ShowIf("@ActingType.HasFlag(eCameraActingType.ZoomInAndOutCurve)")] public float ZoomInAndOutDuration;
    [ShowIf("@ActingType.HasFlag(eCameraActingType.ZoomInAndOutCurve)")] public AnimationCurve ZoomInOutCurve;

    public ePostProcessingType PostProcessingType;
    [ShowIf("@PostProcessingType.HasFlag(ePostProcessingType.ChromaticAberration)")] public PostProcessingInvokingConfig ChromaticAberrationInvokeSetting;
    [ShowIf("@PostProcessingType.HasFlag(ePostProcessingType.Bright)")] public PostProcessingInvokingConfig BrightingInvokeSetting;
    [ShowIf("@PostProcessingType.HasFlag(ePostProcessingType.EdgeGrayscale)")] public PostProcessingInvokingConfig EdgeGrayscaleInvokeSetting;
    [ShowIf("@PostProcessingType.HasFlag(ePostProcessingType.RadialBlur)")] public PostProcessingInvokingConfig RadialBlurInvokeSetting;




#if UNITY_EDITOR
    [OnInspectorGUI]
    private void updateCameraShakeCurve()
    {
        switch (ShakeType)
        {
            case eShakeType.Recoil:
                ShakeCurve = GlobalSkillConfig.Instance.RecoilCameraCurve;
                break;
            case eShakeType.Bump:
                ShakeCurve = GlobalSkillConfig.Instance.BumpCameraCurve;
                break;
            case eShakeType.Explosion:
                ShakeCurve = GlobalSkillConfig.Instance.ExplosionCameraCurve;
                break;
            case eShakeType.Rumble:
                ShakeCurve = GlobalSkillConfig.Instance.RumbleCameraCurve;
                break;
        }
    }
#endif

    public enum eShakeType
    {
        Recoil,
        Bump,
        Explosion,
        Rumble,
        Impact,
        Custom
    }
}

[Serializable]
public struct FXSpawnData
{
    public eFXTransformType FXTransformType;
    public bool SpawnInShootingPoint;
    [RequiredIn(PrefabKind.PrefabAsset)]public GameObject EffectPrefab;

    [NonSerialized] public float SizeMultiplier;

    public bool IsNull()
    {
        return EffectPrefab == null;
    }
}

[Serializable]
public struct ManualSkillEvent
{
    public eSkillEventMarkerType EventType;
    [Range(0, 1)] public float InvokeTiming;
}

[Serializable]
public struct PostProcessingInvokingConfig
{
    public eVolumnWeightSettingType SettingType;
    public float Duration;
    [ShowIf("SettingType", eVolumnWeightSettingType.CustomCurve)] public AnimationCurve PowerCurve;
    [Range(0, 1)] public float Power;
}

[Serializable]
public struct HitStopData
{
    [Range(0.1f, 0.9f)] public float SlowDownAmount;
    public float SlowTime;
}

[Serializable]
public struct AttackBoxOption
{
    public eAttackBoxType AttackBoxHitType;
    [ShowIf("AttackBoxHitType", eAttackBoxType.MultiHit), Range(1, 300)] public int MaxHitCounts;
    [ShowIf("AttackBoxHitType", eAttackBoxType.MultiHit)] public float HitInterval;
    [ShowIf("AttackBoxHitType", eAttackBoxType.MultiHit)] public bool InvokeDamagedActorPerHit;
    [ShowIf("AttackBoxHitType", eAttackBoxType.MultiHit)] public bool DivideFinalDamageByMaxHitCount;

    public eAttackboxSpawnType SpawnType;
    public eAttackBoxLifetimeType LifetimeType;
    [ShowIf("LifetimeType", eAttackBoxLifetimeType.HandleWithManual)] public float RemainDuration;
    [ToggleGroup("IsMeleeboxProjectile")] public bool IsMeleeboxProjectile;
    [ToggleGroup("IsMeleeboxProjectile")] public float Speed;

    [ToggleGroup("IsThrowingMeleebox")] public bool IsThrowingMeleebox;
    [ToggleGroup("IsThrowingMeleebox")] public float ThrowingDuration;

    [NonSerialized] public Vector3 ThrowingDest;
    [NonSerialized] public float SizeMultiplier;
}

public enum eVolumnWeightSettingType
{
    Constant,
    CustomCurve,
}

[Flags]
public enum eCameraActingType
{
    None = 0,
    Shake = 1 << 0,
    ZoomInAndOutCurve = 1 << 1,
}

[Flags]
public enum ePostProcessingType
{
    None = 0,
    ChromaticAberration = 1 << 0,
    EdgeGrayscale = 1 << 1,
    Bright = 1 << 2,
    RadialBlur = 1 << 3,
    FullGrayScale = 1 << 4,
}

public enum eAttackTransitionType
{
    None,
    SetDecellationFromCurrentSpeed,
    MoveToSpecificDest,
    Pause,
    RushToPlayer,
    LookPlayer,
    ConstantRotate,
}

public enum eSkillRangeType
{
    None = -1,
    Melee,
    Rangend,
}

public enum eKnockbackType
{
    PushToAttackDir,
    CircularToHitPoint,
}

public enum eHitEffectType
{
    None,
    Knockback,
    Stunned,
    Deaccelerate,
    Pause,
}

public enum eSkillProgressState
{
    None = -1,
    Preparing,
    OnAttack,
    Cancelable,
}

public enum eTargetTag
{
    Player,
    Enemey,
    All,
    PlayerAndObject,
}

public enum eStunType
{
    DefaultStone,
}

public enum eCountableType
{
    NoneCountable,
    EasyCountable,
    HardCountable
}

public enum eSkillRotateType
{
    Default,
    Immediate,
    Fix,
}

public enum eAttackBoxType
{
    OneHitOnly,
    MultiHit,
}


public enum eAttackboxSpawnType
{
    Default,
    SpawnInProjectile,
    SpawnInGlobalAttackBox,
}

public enum ePointSpecifyAttackSpawnType
{
    TargetAimAssistance,
    SpreadRandomCircle,
    TargetPlayer,
}