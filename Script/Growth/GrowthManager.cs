using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using UnityEngine.UI;
using System;

public class GrowthManager : SingletonClass<GrowthManager>
{
    [NonSerialized, ShowInInspector] public List<GrowthAbilityData> OwnedAbilList;
    private int mSelectableGrowthCount = 3;
    private Action mOnDialogueEnd;

    protected override void Awake()
    {
        base.Awake();
        GrowthDataUtil.LoadDatas();
        OwnedAbilList = new List<GrowthAbilityData>();
    }

    public void AddAbilData(GrowthAbilityData newData)
    {
        #region Obsolute
        //CharacterStatus currentCharStatus = PlayerManager.Instance.CurrentPlayer.Value.CharacterStatus;
        //PlayerCharacterController targetCharacter = null;
        //if (newData.ApplyingTargetCharacterEnum != eCharacterName.None)
        //{
        //    targetCharacter = PlayerManager.Instance.CharacterDic[newData.ApplyingTargetCharacterEnum];
        //}
        #endregion

        switch (newData.AbilNameIDEnum)
        {
            case eAbilNameID.GlobalComboAttackUpgrade:
                for (eCharacterName i = 0; i < eCharacterName.Shiv; i++)
                {
                    ActorStateMachine actorStateMachine = PlayerManager.Instance.CharacterDic[i].SM;
                    ComboSkillActor normalAttackActor = actorStateMachine.GetActor<ComboSkillActor>(eActorType.NormalAttack);
                    normalAttackActor.SwapComboConfig(RuntimeDataLoader.AdvancedFirstAttackSkillConfigs[i], 0);
                    normalAttackActor.SwapComboConfig(RuntimeDataLoader.AdvancedSecondAttackSkillConfigs[i], 1);
                }

                break;
            case eAbilNameID.GlobalLastAttackUpgrade:
                for (eCharacterName i = 0; i < eCharacterName.Shiv; i++)
                {
                    ActorStateMachine actorStateMachine = PlayerManager.Instance.CharacterDic[i].SM;
                    ComboSkillActor normalAttackActor = actorStateMachine.GetActor<ComboSkillActor>(eActorType.NormalAttack);
                    if(i == eCharacterName.Glanda)
                    {
                        normalAttackActor.SwapComboConfig(RuntimeDataLoader.AdvancedLastAttackSkillConfigs[i], 3);
                    }
                    else
                    {
                        normalAttackActor.SwapComboConfig(RuntimeDataLoader.AdvancedLastAttackSkillConfigs[i], 2);
                    }
                }
                break;
            case eAbilNameID.GlobalSwitchingAttackUpgrade:
                for (eCharacterName i = 0; i < eCharacterName.Shiv; i++)
                {
                    ActorStateMachine actorStateMachine = PlayerManager.Instance.CharacterDic[i].SM;
                    SkillActor switchingAttackActor = actorStateMachine.GetActor<SkillActor>(eActorType.SwitchAttack);
                    switchingAttackActor.TryChangeConfigOrStackedIn(RuntimeDataLoader.AdvancedSwitchingAttackSkillConfigs[i]);
                }
                break;
            #region obsolute
            //case eAbilNameID.GlobalBasicAttackDamageIncrease:
            //    PlayerManager.Instance.GlobalPlayerStatus.GlobalDamage += float.Parse(newData.DynamicParam1);
            //    break;
            //case eAbilNameID.GlobalBasicDefenceIncrease:
            //    PlayerManager.Instance.GlobalPlayerStatus.GlobalDefense += float.Parse(newData.DynamicParam1);
            //    break;
            //case eAbilNameID.GlobalMaxHPIncrease:
            //    for (int i = 0; i < PlayerManager.Instance.ActivatedCharacters.Count; i++)
            //    {
            //        PlayerManager.Instance.AllPlayers[i].CharacterStatus.MaxHP.Value += float.Parse(newData.DynamicParam1);
            //    }
            //    break;
            //case eAbilNameID.GlobalMovementSpeedIncrease:
            //    PlayerManager.Instance.GlobalPlayerStatus.GlobalSpeed += float.Parse(newData.DynamicParam1);
            //    break;
            //case eAbilNameID.GlobalBasicAllStatIncrease:
            //    PlayerManager.Instance.GlobalPlayerStatus.GlobalDamage += float.Parse(newData.DynamicParam1);
            //    PlayerManager.Instance.GlobalPlayerStatus.GlobalDefense += float.Parse(newData.DynamicParam2);
            //    for (int i = 0; i < PlayerManager.Instance.ActivatedCharacters.Count; i++)
            //    {
            //        PlayerManager.Instance.AllPlayers[i].CharacterStatus.MaxHP.Value += float.Parse(newData.DynamicParam3);
            //    }
            //    PlayerManager.Instance.GlobalPlayerStatus.GlobalSpeed += float.Parse(newData.DynamicParam4);
            //    break;
            //case eAbilNameID.GlobalAddExtraDash:
            //    PlayerManager.Instance.GlobalPlayerStatus.GlobalAdditionalDashableCount += int.Parse(newData.DynamicParam1);
            //    break;
            //case eAbilNameID.GlobalSwapSkillCooldownDecrease:
            //    float cooldownAmount = float.Parse(newData.DynamicParam1);
            //    foreach (var character in PlayerManager.Instance.AllPlayers)
            //    {
            //        SkillActor swapSkillActor = character.SM.GetActor<SkillActor>(eActorType.SwitchAttack);
            //        if(swapSkillActor == null)
            //        {
            //            continue;
            //        }
            //        float cooldown = swapSkillActor.SkillConfig.CoolTime;
            //        float newCoolDown = Mathf.Max(cooldown - cooldownAmount, 0.0f);
            //        swapSkillActor.SkillConfig.CoolTime = newCoolDown;
            //    }
            //    break;
            //case eAbilNameID.GlobalAddAbilityOption:
            //    mSelectableGrowthCount++;
            //    break;
            //case eAbilNameID.GlandaUpgradeBaseAttack:
            //    float additionalRange = float.Parse(newData.DynamicParam1);
            //    ActorStateMachine glandaSM = targetCharacter.SM;
            //    ComboSkillActor normalAttackActor = glandaSM.GetActor<ComboSkillActor>(eActorType.NormalAttack);

            //    for (int i = 0; i < normalAttackActor.ComboSkillConfigs.Length; i++)
            //    {
            //        SkillConfig sfConfig = normalAttackActor.ComboSkillConfigs[i];

            //        for (int j = 0; j < sfConfig.MeleeAttackData.Length; j++)
            //        {
            //            if (sfConfig.MeleeAttackData[j].Option.IsMeleeboxProjectile)
            //            {
            //                float projectileDuration = sfConfig.MeleeAttackData[j].Option.RemainDuration;
            //                float newProjectileDuration = projectileDuration + projectileDuration * additionalRange;
            //                sfConfig.MeleeAttackData[j].Option.RemainDuration = newProjectileDuration;

            //                float colliderDuration = sfConfig.MeleeAttackData[j].ColliderRemainTime;
            //                float newColliderDuration = colliderDuration + colliderDuration * additionalRange;
            //                sfConfig.MeleeAttackData[j].ColliderRemainTime = newColliderDuration;
            //            }
            //        }
            //    }
            //    break;
            //case eAbilNameID.GlandaUpgradeLastAttack:
            //    float additionalRushDistance = float.Parse(newData.DynamicParam1);
            //    glandaSM = targetCharacter.SM;
            //    normalAttackActor = glandaSM.GetActor<ComboSkillActor>(eActorType.NormalAttack);

            //    SkillConfig lastAttackConfig = normalAttackActor.ComboSkillConfigs[normalAttackActor.ComboSkillConfigs.Length - 1];
            //    lastAttackConfig.TransitionData.MoveToSpecificDest[0].DestDistance += additionalRushDistance;
            //    break;
            //case eAbilNameID.GlandaUpgradeSwapSkill:
            //    glandaSM = targetCharacter.SM;
            //    SkillActor switchingSkillActor = glandaSM.GetActor<SkillActor>(eActorType.SwitchAttack);

            //    switchingSkillActor.SkillConfig.TransitionData.ActionType = eAttackTransitionType.SetDecellationFromCurrentSpeed;
            //    switchingSkillActor.SkillConfig.TransitionData.DecellationAmount = 0.85f;
            //    break;
            //case eAbilNameID.ShivUpgradeBaseAttack:
            //    ActorStateMachine shivSM = targetCharacter.SM;
            //    normalAttackActor = shivSM.GetActor<ComboSkillActor>(eActorType.NormalAttack);
            //    //Todo After Buff System//
            //    break;
            //case eAbilNameID.ShivUpgradeLastAttack:
            //    float sizeMultiplier = float.Parse(newData.DynamicParam1);
            //    shivSM = targetCharacter.SM;
            //    normalAttackActor = shivSM.GetActor<ComboSkillActor>(eActorType.NormalAttack);
            //    lastAttackConfig = normalAttackActor.ComboSkillConfigs[normalAttackActor.ComboSkillConfigs.Length - 1];

            //    for (int i = 0; i < lastAttackConfig.MeleeAttackData.Length; i++)
            //    {
            //        if(lastAttackConfig.MeleeAttackData[i].AttackCollider.name.Contains("ShivNormalAttack"))
            //        {
            //            lastAttackConfig.MeleeAttackData[i].Option.SizeMultiplier += sizeMultiplier;
            //        }
            //    }

            //    for (int i = 0; i < lastAttackConfig.FXSpawnData.Length; i++)
            //    {
            //        if (lastAttackConfig.FXSpawnData[i].EffectPrefab.name.Contains("ShivNormalAttack"))
            //        {
            //            lastAttackConfig.FXSpawnData[i].SizeMultiplier += sizeMultiplier;
            //        }
            //    }
            //    break;
            //case eAbilNameID.ShivUpgradeSwapSkill:
            //    shivSM = targetCharacter.SM;
            //    switchingSkillActor = shivSM.GetActor<SkillActor>(eActorType.SwitchAttack);
            //    //Todo After Buff System//
            //    break;
            //case eAbilNameID.HypoUpgradeBaseAttack:
            //    ActorStateMachine hypoSM = targetCharacter.SM;
            //    normalAttackActor = hypoSM.GetActor<ComboSkillActor>(eActorType.NormalAttack);

            //    for (int i = 0; i < normalAttackActor.ComboSkillConfigs.Length; i++)
            //    {
            //        SkillConfig sfConfig = normalAttackActor.ComboSkillConfigs[i];

            //        for (int j = 0; j < sfConfig.ProjectileData.Length; j++)
            //        {
            //            if (sfConfig.ProjectileData[j].IsMultiShot)
            //            {
            //                sfConfig.ProjectileData[j].ShootingAmount += 1;
            //            }
            //            else if(sfConfig.ProjectileData[j].ShootingType == ProjectileAttackData.eShootingType.ShotGun)
            //            {
            //                sfConfig.ProjectileData[j].ShotgunAmount += 1;
            //            }
            //        }
            //    }
            //    break;
            //case eAbilNameID.HypoUpgradeLastAttack:
            //    hypoSM = targetCharacter.SM;
            //    normalAttackActor = hypoSM.GetActor<ComboSkillActor>(eActorType.NormalAttack);
            //    lastAttackConfig = normalAttackActor.ComboSkillConfigs[normalAttackActor.ComboSkillConfigs.Length - 1];

            //    for (int i = 0; i < lastAttackConfig.MeleeAttackData.Length; i++)
            //    {
            //        lastAttackConfig.MeleeAttackData[i].OnHitEffect.KnockbackData.NockbackPower *= -1;
            //    }
            //    break;
            //case eAbilNameID.HypoUpgradeSwapSkill:
            //    sizeMultiplier = float.Parse(newData.DynamicParam1);
            //    hypoSM = targetCharacter.SM;
            //    switchingSkillActor = hypoSM.GetActor<SkillActor>(eActorType.SwitchAttack);

            //    for (int i = 0; i < switchingSkillActor.SkillConfig.MeleeAttackData.Length; i++)
            //    {
            //        if (switchingSkillActor.SkillConfig.MeleeAttackData[i].AttackCollider.name.Contains("HypoSwitchingAttack"))
            //        {
            //            switchingSkillActor.SkillConfig.MeleeAttackData[i].Option.SizeMultiplier += sizeMultiplier;
            //        }
            //    }

            //    for (int i = 0; i < switchingSkillActor.SkillConfig.FXSpawnData.Length; i++)
            //    {
            //        if (switchingSkillActor.SkillConfig.FXSpawnData[i].EffectPrefab.name.Contains("HypoSwitchingAttack"))
            //        {
            //            switchingSkillActor.SkillConfig.FXSpawnData[i].SizeMultiplier += sizeMultiplier;
            //        }
            //    }
            //    break;
            #endregion
            default:
                Debug.Assert(false, $"Default switch detected [{newData.AbilNameIDEnum}]");
                break;
        }
        OwnedAbilList.Add(newData);
    }

    public void RemoveAbilData(GrowthAbilityData newData)
    {
        OwnedAbilList.Remove(newData);
    }

    public Coroutine StartGrowthSelection()
    {
        return StartGrowthSelection(null, null);
    }

    public Coroutine StartGrowthSelection(Action onDialogueEnd)
    {
        return StartGrowthSelection(onDialogueEnd, null);
    }

    public Coroutine StartGrowthSelection(Action onDialogueEnd, AbilityRankChacneData chacne)
    {
        mOnDialogueEnd = onDialogueEnd;
        return StartCoroutine(startGrowthRoutine(mSelectableGrowthCount, chacne));
    }

    [Button(Name = "Debug StartGrowth Selection"), HideInEditorMode]
    public Coroutine StartGrowthSelection(int count)
    {
        return StartCoroutine(startGrowthRoutine(count));
    }

    private IEnumerator startGrowthRoutine(int count, AbilityRankChacneData chance = null)
    {
        Debug.Log("HI");
        PlayerManager.Instance.IsInputEnabled = false;
        GrowthAbilityData[] datas = GrowthDataUtil.GetRandomDataList(count, OwnedAbilList);
        GrowthUI growthUI = UIManager.Instance.SFGrowthUI;
        growthUI.ShowGrowthUI(datas);
        yield return new WaitUntil(() => growthUI.SelectedDataOrNull != null);
        growthUI.HideGrowthUI();
        AddAbilData(growthUI.SelectedDataOrNull);
        PlayerManager.sPreOwnedAbilNameIDs.Add(growthUI.SelectedDataOrNull);
        PlayerManager.Instance.IsInputEnabled = true;
        mOnDialogueEnd?.Invoke();
        mOnDialogueEnd = null;
    }

#if UNITY_EDITOR
    [Button(Name = "Add Ability"), HideInEditorMode]
    private void addAbilDebug(GrowthAbilityData data)
    {
        if (!GrowthDataUtil.CheckMoreOwnable(data, OwnedAbilList))
        {
            return;
        }

        AddAbilData(data);
    }
#endif
}

