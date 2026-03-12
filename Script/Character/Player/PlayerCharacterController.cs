using UnityEngine;
using UnityEngine.InputSystem;
using Sirenix.OdinInspector;
using System.Collections;
using DG.Tweening;

public class PlayerCharacterController : CharacterBase
{
    [field: SerializeField]
    public Collider BodyCollider
    {
        get; private set;
    }

    public RuntimePlayerStatus PlayerStatus
    {
        get; private set;
    }

    public eCharacterName CharName
    {
        get
        {
            return sfCharacterName;
        }
    }

    public RenderToggle RenderToggle
    {
        get; private set;
    }

    [SerializeField] private PlayerConfig sfConfig;
    [SerializeField] private eCharacterName sfCharacterName;

#if UNITY_EDITOR
    [ShowInInspector]
    private eActorType CurrentActorDebugging
    {
        get
        {
            if (SM == null)
            {
                return eActorType.None;
            }

            return SM.CurrentActorType;
        }
    }
#endif

    protected override void Awake()
    {
        base.Awake();
        RenderToggle = GetComponentInChildren<RenderToggle>();
        PlayerStatus = new RuntimePlayerStatus(sfConfig);
        CharacterStatus = PlayerStatus;
        PlayerStatus.CurrentHP.AddListener(onDamaged);
        PlayerStatus.CurrentHP.AddListener(onHealed);
    }

    protected override void Start()
    {
        base.Start();
        initializeCharacterStateMachine();
        addControllerInputCallback();
        Translator.SwitchComponent(eTranslatorType.Rigidbody);
    }

    private void OnEnable()
    {
        Hitbox.enabled = true;
    }

    protected void OnDestroy()
    {
        if (GlobalTimer.IsExist)
        {
            GlobalTimer.Instance.RemoveAllTimerByInstance(gameObject.GetInstanceID());
        }

        if (InputManager.IsExist)
        {
            removeControllerInputCallback();
        }
    }

    public void MoveToAnotherScene(Vector3 approachPoint, Vector3 exitPoint, eSceneName sceneName)
    {
        StartCoroutine(moveAnotherSceneRoutine(approachPoint, exitPoint, sceneName));
    }

    public void UpdatePlayer()
    {
        if (!PlayerManager.Instance.IsInputEnabled || InputManager.Instance.CurrentInputSection != eInputSections.BattleGamePlay)
        {
            if(SM.CurrentActorType != eActorType.UltimateAttack)
            {
                Translator.RB.GetLayeredRigidbody().DisEnrollVelocityAll();
            }
            if (!Translator.RB.isKinematic)
            {
                Translator.UpdateTransform();
                Anim.SetMovementBlendFactor(0.0f);
            }
            return;
        }

        SM.UpdateActor(Time.fixedDeltaTime);
        Translator.UpdateTransform();
    }

    public void OnPlayerToggled(Vector3 initialPos)
    {
        enabled = true;
        Translator.RB.position = initialPos;
    }

    public override void OnHit(DamageInfo damageInfo)
    {
        if(CharacterStatus.IsPowerOverwalming.GetValue())
        {
            return;
        }

        if(damageInfo.HitData.CountableType != eCountableType.NoneCountable)
        {
            if(SM.CurrentActorType == eActorType.TryCounterAttack)
            {
                damageInfo.AttackerOrNull.SM.TrySwitchActor(eActorType.Damaged, HitEffectData.GetCounterHitEffectData(), null);
                tryUsingSkill(sfConfig.CounterAttack);
                return;
            }
        }

        if (damageInfo.HitData.OnHitFX != null)
        {
            ParticleBinder effect = FXPool.GetGameobject(damageInfo.HitData.OnHitFX);
            effect.SetFXTransformType(eFXTransformType.PositionOnlyWorld, mHitFXSpawnPoint.localPosition, Translator.Trans);
        }

        SFXManager.PlayPlayerPublicHitSound();
        float damageReduction = Mathf.Clamp((1 - (CharacterStatus.Defense + PlayerManager.Instance.GlobalPlayerStatus.GlobalDefense)), 0.2f, 1f);
        float damage = damageInfo.Damage * damageReduction;

        CharacterStatus.LastDamageInfo.Value = damageInfo;
        PlayerStatus.DecreaseHP(damage, out bool isForceShieldDecreased);

        if(CharacterStatus.CurrentHP > 0.0f && !isForceShieldDecreased)
        {
            SM.TrySwitchActor(eActorType.Damaged, damageInfo.HitData, null);
        }
    }

    public void TogglePlayerOn(PlayerCharacterController prevCharacterOrNull, bool withSwitchingAttack) 
    {
        if (enabled == true)
        {
            return;
        }

        addControllerInputCallback();
        if(prevCharacterOrNull != null)
        {
            Translator.Trans.position = prevCharacterOrNull.Translator.Trans.position;
        }
        Translator.Trans.gameObject.SetActive(true);
        enabled = true;


        if(withSwitchingAttack)
        {
            float damage = calculateDamage();
            SM.TrySwitchActor(eActorType.SwitchAttack, damage, true);
        }
    }

    public void TogglePlayerOff()
    {
        if(enabled == false)
        {
            return;
        }

        removeControllerInputCallback();
        SM.PauseAllActor();
        Translator.Trans.gameObject.SetActive(false);
        enabled = false;
    }

    public void PlayUltimateSkillTimeline()
    {
        PlayerManager.Instance.IsInputEnabled = false;
        GetComponentInChildren<UltimateTimelineHandler>().PlayUltimateSkillTimeline(Translator.Trans.position, RenderToggle);
        PlayerStatus.SetUltimateGuageZero();
    }

    public void OnItemReceived(ItemData itemData)
    {
        switch (itemData.ItemNameIDEnum)
        {
            case eItemNameID.PersonalHealingHPLow:
            case eItemNameID.PersonalHealingHPMiddle:
            case eItemNameID.PersonalHealingHPMax:
                BuffHandler.AddBuff(eBuffNameID.InstanceHealing, itemData.Duration, itemData.Power1, itemData.Power2, itemData.Power3);
                break;
            case eItemNameID.PersonalUltimateGaugeIncarease:
                PlayerManager.Instance.FloatingInfomessageTrigger.Value = eFloatingInfoMessageTag.GetUltimateGuageSmall;
                BuffHandler.AddBuff(eBuffNameID.UltimateGuageUp, itemData.Duration, itemData.Power1, itemData.Power2, itemData.Power3);
                break;
            case eItemNameID.PersonalUltimateGaugeMax:
                PlayerManager.Instance.FloatingInfomessageTrigger.Value = eFloatingInfoMessageTag.GetUltimateGuageBig;
                BuffHandler.AddBuff(eBuffNameID.UltimateGuageUp, itemData.Duration, itemData.Power1, itemData.Power2, itemData.Power3);
                break;
            case eItemNameID.PowerUp:
                PlayerManager.Instance.AddBuffAllActivatedCharacter(eBuffNameID.PowerUp, itemData.Duration, itemData.Power1, itemData.Power2, itemData.Power3);
                break;
            case eItemNameID.AttackSpeedUp:
                PlayerManager.Instance.AddBuffAllActivatedCharacter(eBuffNameID.AttackSpeedUp, itemData.Duration, itemData.Power1, itemData.Power2, itemData.Power3);
                break;
            case eItemNameID.SuperMode:
                PlayerManager.Instance.AddBuffAllActivatedCharacter(eBuffNameID.PowerOverwalming, itemData.Duration, itemData.Power1, itemData.Power2, itemData.Power3);
                break;
        }
    }

    public void ReviveCharacter(float reviveHP)
    {
        CharacterStatus.IsDead.Value = false;
        PlayerStatus.AddHP(reviveHP);
    }

    private void initializeCharacterStateMachine()
    {
        SM = new ActorStateMachine(this, true);
        addSkillActor(SM, sfConfig.NormalAttack);
        addSkillActor(SM, sfConfig.NormalChargeAttack);
        addSkillActor(SM, sfConfig.SpecialAttack);
        addSkillActor(SM, sfConfig.DashAttack);
        addSkillActor(SM, sfConfig.SwitchingAttack);
        addSkillActor(SM, sfConfig.UltimateAttack, false);
        addSkillActor(SM, sfConfig.TryCounterAttack, false);
        addSkillActor(SM, sfConfig.CounterAttack, false);

        DashActor dashActor = new DashActor(SM, sfConfig.DashConfig, null);
        SM.AddActor(dashActor);
        InputMovementActor inputActor = new InputMovementActor(SM, sfConfig.MovementConfig, CharacterStatus);
        SM.AddActor(inputActor);
        DamagedActor damagedActor = new DamagedActor(SM, sfConfig.DamagedActorConfig, null);
        SM.AddActor(damagedActor);

        if (sfConfig.DeadActorConfig != null)
        {
            DeadActor deadActor = new DeadActor(SM, sfConfig.DeadActorConfig);
            SM.AddActor(deadActor);
        }
    }


    private void addControllerInputCallback()
    {
        InputManager.Instance.AddInputCallback(eInputSections.BattleGamePlay, eBattleInputName.Dash.ToString(), tryDash);
        InputManager.Instance.AddInputCallback(eInputSections.BattleGamePlay, eBattleInputName.NormalAttack.ToString(), tryNormalAttack);
        InputManager.Instance.AddInputCallback(eInputSections.BattleGamePlay, eBattleInputName.NormalAttack.ToString(), tryNormalChargeAttackPressed, InputActionPhase.Performed);
        InputManager.Instance.AddInputCallback(eInputSections.BattleGamePlay, eBattleInputName.NormalAttack.ToString(), tryNormalChargeAttackReleased, InputActionPhase.Canceled);
        InputManager.Instance.AddInputCallback(eInputSections.BattleGamePlay, eBattleInputName.UltimateSkill.ToString(), tryUsingUltimateSkill);
        InputManager.Instance.AddInputCallback(eInputSections.BattleGamePlay, eBattleInputName.TryCounter.ToString(), tryCounter);
        InputManager.Instance.AddInputCallback(eInputSections.BattleGamePlay, eBattleInputName.SpeicalAttack.ToString(), trySpecialAttack);
        InputManager.Instance.AddInputCallback(eInputSections.BattleGamePlay, eBattleInputName.SwitchPlayerToNext.ToString(), trySwitchingAttack);
        GetComponent<PlayerInteractiveManager>().EnableInputCallback();

    }

    private void removeControllerInputCallback()
    {
        InputManager.Instance.RemoveInputCallback(eInputSections.BattleGamePlay, eBattleInputName.Dash.ToString(), tryDash);
        InputManager.Instance.RemoveInputCallback(eInputSections.BattleGamePlay, eBattleInputName.NormalAttack.ToString(), tryNormalAttack);
        InputManager.Instance.RemoveInputCallback(eInputSections.BattleGamePlay, eBattleInputName.NormalAttack.ToString(), tryNormalChargeAttackPressed, InputActionPhase.Performed);
        InputManager.Instance.RemoveInputCallback(eInputSections.BattleGamePlay, eBattleInputName.NormalAttack.ToString(), tryNormalChargeAttackReleased, InputActionPhase.Canceled);
        InputManager.Instance.RemoveInputCallback(eInputSections.BattleGamePlay, eBattleInputName.UltimateSkill.ToString(), tryUsingUltimateSkill);
        InputManager.Instance.RemoveInputCallback(eInputSections.BattleGamePlay, eBattleInputName.TryCounter.ToString(), tryCounter);
        InputManager.Instance.RemoveInputCallback(eInputSections.BattleGamePlay, eBattleInputName.SpeicalAttack.ToString(), trySpecialAttack);
        InputManager.Instance.RemoveInputCallback(eInputSections.BattleGamePlay, eBattleInputName.SwitchPlayerToNext.ToString(), trySwitchingAttack);
        GetComponent<PlayerInteractiveManager>().DisableInputCallback();
    }

    private void tryDash(InputAction.CallbackContext context)
    {
        if(!PlayerManager.Instance.IsInputEnabled)
        {
            return;
        }

        DashActor dashActor = SM.GetActor<DashActor>(eActorType.Dash);


        if (dashActor.IsExtraDashPressed)
        {
            return;
        }

        if (!dashActor.CanDash)
        { 
            return;
        }


        if(dashActor.IsInHolding)
        {
            dashActor.IsExtraDashPressed = true;
        }
        else
        {
            bool needPausePrevActor = true;
            if (SM.CurrentActorType.IsAttackType())
            {
                SkillActor skillActor = SM.CurrentActorOrNull as SkillActor;

                if (skillActor is ChargeSkillActor chargeActor)
                {
                    if(chargeActor.IsOnCharging)
                    {
                        chargeActor.ChargingShot();
                        needPausePrevActor = false;
                    }
                }
            }


            SM.TrySwitchActor(eActorType.Dash, needPausePrevActor);
        }
    }

    private void tryNormalAttack(InputAction.CallbackContext context)
    {
        if (!PlayerManager.Instance.IsInputEnabled)
        {
            return;
        }

        if (sfConfig.NormalAttack == null)
        {
            return;
        }

        if(SM.CurrentActorType == eActorType.Dash)
        {
            DashActor dashActor = SM.CurrentActorOrNull as DashActor;
            if (dashActor.CanDashAttack)
            {
                SkillActor dashSkillActor = SM.TryGetActorOrNull<SkillActor>(eActorType.DashAttack);

                if (dashSkillActor != null && dashSkillActor.CanAttack)
                {
                    if (dashSkillActor.CanAttack)
                    {
                        SM.TrySwitchActor(eActorType.DashAttack, calculateDamage(), null, false);
                        return;
                    }
                }
            }
        }

        tryUsingSkill(sfConfig.NormalAttack);
    }

    private void tryNormalChargeAttackPressed(InputAction.CallbackContext context)
    {
        if (!PlayerManager.Instance.IsInputEnabled)
        {
            return;
        }

        if (sfConfig.NormalChargeAttack == null)
        {
            return;
        }


        tryUsingChargeAttack(sfConfig.NormalChargeAttack, true);
    }

    private void tryNormalChargeAttackReleased(InputAction.CallbackContext context)
    {
        if (!PlayerManager.Instance.IsInputEnabled)
        {
            return;
        }

        if (sfConfig.NormalChargeAttack == null)
        {
            return;
        }


        tryUsingChargeAttack(sfConfig.NormalChargeAttack, false);
    }

    private void trySpecialAttack(InputAction.CallbackContext context)
    {
        if (!PlayerManager.Instance.IsInputEnabled)
        {
            return;
        }

        if (sfConfig.SpecialAttack == null)
        {
            return;
        }

        tryUsingSkill(sfConfig.SpecialAttack);
    }

    private void trySwitchingAttack(InputAction.CallbackContext context)
    {
        if (!PlayerManager.Instance.IsInputEnabled)
        {
            return;
        }

        if (sfConfig.SwitchingAttack == null)
        {
            PlayerManager.Instance.SwitchCharacterToNext(true);
            return;
        }

        tryUsingSkill(sfConfig.SwitchingAttack);
    }

    private void tryCounter(InputAction.CallbackContext context)
    {
        if (!PlayerManager.Instance.IsInputEnabled)
        {
            return;
        }

        if (sfConfig.TryCounterAttack == null)
        {
            return;
        }

        tryUsingSkill(sfConfig.TryCounterAttack);
    }

    private void tryUsingUltimateSkill(InputAction.CallbackContext context)
    {
        if (!SectionElement.sIsOnBattle)
        {
            return;
        }

        if (sfConfig.UltimateAttack == null)
        {
            return;
        }

        if (PlayerStatus.UltimateGuage.Value.Normalize < 1.0f)
        {
            return;
        }



        bool ultimateSkillInvoked = tryUsingSkill(sfConfig.UltimateAttack);
    }


    private void addSkillActor(ActorStateMachine SM, SkillConfig skillConfigOrNull, bool addUltimateGuageIncreaseCallback = true)
    {
        if(skillConfigOrNull == null)
        {
            return;
        }

        if (skillConfigOrNull.IsComboAttack)
        {
            ComboSkillActor skillActor = new ComboSkillActor(SM, skillConfigOrNull, addUltimateGuageIncreaseCallback ? onDamagingSomething : null, null);
            SM.AddActor(skillActor);
        }
        else if(skillConfigOrNull.IsChargingAttack)
        {
            ChargeSkillActor skillActor = new ChargeSkillActor(SM, skillConfigOrNull, addUltimateGuageIncreaseCallback ? onDamagingSomething : null, null);
            SM.AddActor(skillActor);
        }
        else if(skillConfigOrNull.IsChannellingSkill)
        {
            ChannelingSkillActor skillActor = new ChannelingSkillActor(SM, skillConfigOrNull, addUltimateGuageIncreaseCallback ? onDamagingSomething : null, null);
            SM.AddActor(skillActor);
        }
        else if(skillConfigOrNull.BaseConfig.ActorType == eActorType.SwitchAttack)
        {
            SwitchingSkillActor skillActor = new SwitchingSkillActor(SM, skillConfigOrNull, addUltimateGuageIncreaseCallback ? onDamagingSomething : null, null);
            SM.AddActor(skillActor);
        }
        else
        {
            SkillActor skillActor = new SkillActor(SM, skillConfigOrNull, addUltimateGuageIncreaseCallback ? onDamagingSomething : null, null);
            SM.AddActor(skillActor);
        }
    }

    private bool tryUsingSkill(SkillConfig skillConfig)
    {
        SkillActor skillActor = SM.TryGetActorOrNull<SkillActor>(skillConfig.BaseConfig.ActorType);

        if (skillActor == null)
        {
            return false;
        }


        float damage = calculateDamage();
        if (skillConfig.IsComboAttack)
        {
            ComboSkillActor comboSkillActor = skillActor as ComboSkillActor;
            if (comboSkillActor.CanComboAttack)
            {
                SM.TrySwitchActor(skillConfig.BaseConfig.ActorType, damage, null, true);
                return true;
            }
        }

        if (!skillActor.CanAttack)
        {
            return false;
        }


        if (skillConfig.BaseConfig.ActorType == eActorType.SwitchAttack)
        {
            SM.TrySwitchActor(skillConfig.BaseConfig.ActorType, damage, false, true);
        }
        else
        {
            SM.TrySwitchActor(skillConfig.BaseConfig.ActorType, damage, null, true);
        }
        return true;
    }


    private void tryUsingChargeAttack(SkillConfig chargeSkillConfig, bool pressedOrRelease)
    {
        Debug.Assert(chargeSkillConfig.IsChargingAttack, "You can using charge attack only chargeSkillConfig");

        ChargeSkillActor skillActor = SM.TryGetActorOrNull<ChargeSkillActor>(chargeSkillConfig.BaseConfig.ActorType);

        if(pressedOrRelease == false)
        {
            if (skillActor.IsOnCharging)
            {
                skillActor.ChargingShot();
                return;
            }
        }
        else
        {
            if (!skillActor.CanAttack)
            {
                return;
            }

            if (!skillActor.IsOnCharging && pressedOrRelease == false)
            {
                return;
            }
            else
            {
                float damage = calculateDamage();
                SM.TrySwitchActor(eActorType.NormalChargeAttack, damage, null);
            }
        }
    }


    private void onDamagingSomething(float damage)
    {
        PlayerStatus.IncreaseUltimateGuage(damage * PlayerStatus.UltimateGuageIncreasementPerDamage);
    }

    private void onDamaged(float prevHP, float currentHP)
    {
        if(prevHP <= currentHP)
        {
            return;
        }

        if (currentHP <= 0)
        {
            SM.TrySwitchActor(eActorType.Dead, default(HitEffectData), new System.Action(PlayerManager.Instance.OnPlayerDeadActorEnd));
            PlayerManager.Instance.OnPlayerDead();
        }
        else
        {
            CameraManager.Instance.Actor.PlayOnScreenVFX(eOnScreenVFXType.OnHurt);
        }
    }

    private void onHealed(float prevHP, float currentHP)
    {
        if (prevHP > currentHP)
        {
            return;
        }

        if(enabled == true)
        {
            CameraManager.Instance.Actor.PlayOnScreenVFX(eOnScreenVFXType.OnHealing);
        }
    }

    private float calculateDamage()
    {
        float damage = CharacterStatus.Power + CharacterStatus.Power * PlayerManager.Instance.GlobalPlayerStatus.GlobalDamage;
        return damage;
    }

    private IEnumerator moveAnotherSceneRoutine(Vector3 approachPoint, Vector3 exitPoint, eSceneName sceneName)
    {
        Translator.RB.GetLayeredRigidbody().DisEnrollVelocityAll();
        Translator.RB.GetLayeredRigidbody().DisEnrollRotationAll();

        PlayerManager.Instance.IsInputEnabled = false;
        Translator.RB.isKinematic = true;
        float speed = 7.25f;
        enabled = false;
        Tween moveTween = Translator.Trans.DOMove(approachPoint, speed).SetEase(Ease.Linear).SetSpeedBased();

        Vector3 toward = new Vector3(approachPoint.x, 0.0f, approachPoint.z);
        Translator.Trans
            .DOLookAt(toward, 0.2f).SetEase(Ease.Linear);

        while (moveTween.IsActive())
        {
            Anim.UpdateMovementAnim(Vector3.one, false);
            yield return null;
        }

        moveTween = Translator.Trans.DOMove(exitPoint, speed).SetEase(Ease.Linear).SetSpeedBased();

        toward = new Vector3(exitPoint.x, 0.0f, exitPoint.z);
        Translator.Trans
            .DOLookAt(toward, 0.2f).SetEase(Ease.Linear);
        while (moveTween.IsActive())
        {
            Anim.UpdateMovementAnim(Vector3.one, false);
            yield return null;
        }

        PlayerManager.Instance.IsInputEnabled = true;
        Translator.RB.isKinematic = false;
        enabled = true;
        SceneSwitchingManager.Instance.LoadOtherScene(sceneName, true);
    }

#if UNITY_EDITOR
    [Button]
    private void moveToPlayerEntrance()
    {
        transform.FindRecursive("PlayerTrans").position = GameObject.FindGameObjectWithTag("PlayerEntrance").transform.position;
    }
#endif
}
