using UnityEngine;
using UnityEngine.Playables;
using System.Collections.Generic;
using UnityEngine.UI;

public class PlayerManager : SingletonClass<PlayerManager>
{
    public ObservedData<eFloatingInfoMessageTag> FloatingInfomessageTrigger;
    public ObservedData<PlayerCharacterController> CurrentPlayer;

    public Dictionary<eCharacterName, PlayerCharacterController> CharacterDic
    {
        get; private set;
    }

    public List<PlayerCharacterController> ActivatedCharacters
    {
        get; private set;
    }

    public int CurrentActivatedCharacterIndex
    {
        get; private set;
    }

    public bool IsInputEnabled
    {
        get
        {
            return mIsInputEnabled;
        }
        set
        {
            mIsInputEnabled = value;
        }
    }

    public GlobalPlayerStatus GlobalPlayerStatus
    {
        get; private set;
    }

    public GameObjectPool GlobalFXPool
    {
        get; private set;
    }

    public Transform AcitvatedPlayerTrans 
    {
        get
        {
            if (CurrentPlayer.Value == null)
            {
                return null;
            }
            return CurrentPlayer.Value.Translator.Trans;
        }
    }

    public Cinemachine.CinemachineVirtualCamera PlayerVCam
    {
        get
        {
            return sfPlayerVCam;
        }
    }

    public PlayerCharacterController[] AllPlayers
    {
        get; private set;
    }

    public static List<GrowthAbilityData> sPreOwnedAbilNameIDs;

    private readonly eCharacterName[] mMainPlayerOrder = { eCharacterName.Glanda, eCharacterName.Hypo, eCharacterName.Shiv };
    [SerializeField] private Cinemachine.CinemachineVirtualCamera sfPlayerVCam;
    [SerializeField] private PlayableDirector sfDeathTimeline;
    private bool mIsInputEnabled;
    private int mDeathPlayerCount;
    protected override void Awake()
    {
        base.Awake();
        GlobalPlayerStatus = new GlobalPlayerStatus();
        AllPlayers = GetComponentsInChildren<PlayerCharacterController>(true);
        CharacterDic = new Dictionary<eCharacterName, PlayerCharacterController>(AllPlayers.Length);
        for (int i = 0; i < AllPlayers.Length; i++)
        {
            CharacterDic.Add(AllPlayers[i].CharName, AllPlayers[i]);
        }

        sfDeathTimeline.stopped += (pd) => UIManager.Instance.SFGameOverUI.SetActive(true);
        ActivatedCharacters = new List<PlayerCharacterController>();
        ActivatedCharacters.Add(CharacterDic[eCharacterName.Glanda]);

        GlobalFXPool = GameObject.FindWithTag("GlobalFX").GetComponent<GameObjectPool>();
        IsInputEnabled = true;
    }

    private void Start()
    {
        for (int i = 1; i < AllPlayers.Length; i++)
        {
            AllPlayers[i].TogglePlayerOff();
        }

        switch (SceneSwitchingManager.Instance.OnSceneSwitchingStart.Value)
        {
            case eSceneName.Scene_Stage1:
                break;
            case eSceneName.Scene_Stage2:
                ActivatedCharacters.Add(CharacterDic[eCharacterName.Shiv]);
                break;
            case eSceneName.Scene_Stage3:
                ActivatedCharacters.Add(CharacterDic[eCharacterName.Hypo]);
                ActivatedCharacters.Add(CharacterDic[eCharacterName.Shiv]);
                break;
            default:
                break;
        }

        if(sPreOwnedAbilNameIDs == null)
        {
            sPreOwnedAbilNameIDs = new List<GrowthAbilityData>();
        }
        else 
        {
            int maxOwnedCount = 0;
            switch (SceneSwitchingManager.Instance.OnSceneSwitchingStart.Value)
            {
                case eSceneName.Scene_Stage1:
                    maxOwnedCount = 0;
                    break;
                case eSceneName.Scene_Stage2:
                    maxOwnedCount = 1;
                    break;
                case eSceneName.Scene_Stage3:
                    maxOwnedCount = 3;
                    break;
                default:
                    Debug.LogError($"YOu Cannot load player manager in scene {SceneSwitchingManager.Instance.CurrentScene.Value}");
                    break;
            }
            for (int i = 0; i < sPreOwnedAbilNameIDs.Count; i++)
            {
                if (i >= maxOwnedCount)
                {
                    sPreOwnedAbilNameIDs.RemoveRange(i, sPreOwnedAbilNameIDs.Count - i);
                    break;
                }

                GrowthManager.Instance.AddAbilData(sPreOwnedAbilNameIDs[i]);
            }

            UpdateCurrentCharacter(false);
        }
    }

    private void FixedUpdate()
    {
        CurrentPlayer.Value?.UpdatePlayer();
    }

    private void Update()
    {
        for (int i = 0; i < ActivatedCharacters.Count; i++)
        {
            ActivatedCharacters[i].BuffHandler.UpdateBuff(Time.deltaTime);
        }
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
    }

    public void ToggleAllPlayer(bool toggle)
    {
        for (int i = 0; i < ActivatedCharacters.Count; i++)
        {
            ActivatedCharacters[i].RenderToggle.Toggle(toggle);
        }
    }

    public void ReviveAllCharacter(float reviveHP)
    {
        for (int i = 0; i < ActivatedCharacters.Count; i++)
        {
            if(ActivatedCharacters[i].CharacterStatus.IsDead)
            {
                ActivatedCharacters[i].ReviveCharacter(reviveHP);
            }
        }

        for (int i = 0; i < ActivatedCharacters.Count; i++)
        {
            int priorityI = checkPriority(ActivatedCharacters[i].CharName);
            for (int j = 0; j <= i; j++)
            {
                if(i == j)
                {
                    continue;
                }

                int priorityJ = checkPriority(ActivatedCharacters[j].CharName);
                if (priorityI < priorityJ)
                {
                    PlayerCharacterController temp = ActivatedCharacters[i];
                    ActivatedCharacters[i] = ActivatedCharacters[j];
                    ActivatedCharacters[j] = temp;
                }
            }
        }
        mDeathPlayerCount = 0;
        UpdateCurrentCharacter();
    }

    public void WarpCurrentPlayer(Vector3 warpPos)
    {
        CurrentPlayer.Value.Translator.Trans.position = warpPos;
    }

    public void AddBuffAllActivatedCharacter(eBuffNameID data, float duration, params float[] powers)
    {
        for (int i = 0; i < ActivatedCharacters.Count; i++)
        {
            ActivatedCharacters[i].BuffHandler.AddBuff(data, duration, powers);
        }
        
    }

    public void AddActivatedCharacter(eCharacterName newCharacter)
    {
        if(ActivatedCharacters.Contains(CharacterDic[newCharacter]))
        {
            return;
        }

        switch (newCharacter)
        {
            case eCharacterName.Hypo:
                FloatingInfomessageTrigger.Value = eFloatingInfoMessageTag.GetHypo;
                break;
            case eCharacterName.Shiv:
                FloatingInfomessageTrigger.Value = eFloatingInfoMessageTag.GetShiv;
                break;
            default:
                break;
        }
        int insertPriority = checkPriority(newCharacter);

        for (int i = 0; i < ActivatedCharacters.Count; i++)
        {
            int priority = checkPriority(ActivatedCharacters[i].CharName);
            if (insertPriority < priority)
            {
                ActivatedCharacters.Insert(i, CharacterDic[newCharacter]);
                return;
            }
        }
        ActivatedCharacters.Add(CharacterDic[newCharacter]);

    }

    public void UpdateCurrentCharacter(bool withSwitchignAttack = false)
    {
        PlayerCharacterController prevCharacterOrNull = null;
        if (CurrentPlayer.Value != null)
        {
            prevCharacterOrNull = CurrentPlayer;
            prevCharacterOrNull.TogglePlayerOff();
        }

        PlayerCharacterController newPlayer = ActivatedCharacters[0];

        newPlayer.TogglePlayerOn(prevCharacterOrNull, withSwitchignAttack);
        PlayerVCam.Follow = newPlayer.Translator.Trans;
        CurrentPlayer.Value = newPlayer;
    }

    public bool IsPlayerInRange(Vector3 checkPos, float range)
    {
        if (AcitvatedPlayerTrans == null)
        {
            return false;
        }


        float playerToEnemey = (AcitvatedPlayerTrans.position - checkPos).sqrMagnitude;
        float sqrRange = range * range;

        return playerToEnemey < sqrRange;
    }

    public Vector3 GetRotateTowardVectorToPlayer(Vector3 from, Vector3 pos, float trackSpeed)
    {
        if (AcitvatedPlayerTrans == null)
        {
            return Vector3.zero;
        }

        Vector3 targetToPlayer = AcitvatedPlayerTrans.position - pos;
        targetToPlayer = targetToPlayer.normalized;
        targetToPlayer.y = 0.0f;
        return Vector3.RotateTowards(from, targetToPlayer, trackSpeed, 0.0f);
    }

    public void SetMonsterColliderIgnorePlayer(Collider monsterCollider)
    {
        for (int i = 0; i < AllPlayers.Length; i++)
        {
            Physics.IgnoreCollision(monsterCollider, AllPlayers[i].BodyCollider);
        }
    }

    public void InitializeOnNewStage(Vector3 entrancePosition)
    {
        for (int i = 0; i < ActivatedCharacters.Count; i++)
        {
            ActivatedCharacters[i].Translator.Trans.position = entrancePosition;
            ActivatedCharacters[i].Translator.RB.MovePosition(entrancePosition);

        }
        PlayerVCam.PreviousStateIsValid = false;
    }

    public void OnPlayerDead()
    {
        if (!checkGameOver())
        {
            return;
        }

        GameOver();
    }

    public void OnPlayerDeadActorEnd()
    {
        if(checkGameOver())
        {
            return;
        }
        SwitchCharacterToNext(false);
    }

    public void GameOver()
    {
        mIsInputEnabled = false;
        sfDeathTimeline.Play();
    }

    public void SwitchCharacterToNext(bool usingSwitchAttack)
    {
        if(ActivatedCharacters.Count <= 1)
        {
            return;
        }

        if(CurrentPlayer.Value.CharacterStatus.IsDead)
        {
            ActivatedCharacters.Add(ActivatedCharacters[0]);
        }
        else
        {
            ActivatedCharacters.Insert(ActivatedCharacters.Count - mDeathPlayerCount, ActivatedCharacters[0]);
        }

        ActivatedCharacters.RemoveAt(0);
        UpdateCurrentCharacter(usingSwitchAttack);
    }


    private bool checkGameOver()
    {
        int deathCount = 0;

        for (int i = 0; i < ActivatedCharacters.Count; i++)
        {
            if (ActivatedCharacters[i].CharacterStatus.IsDead)
            {
                deathCount++;
            }
        }
        mDeathPlayerCount = deathCount;
        return deathCount == ActivatedCharacters.Count;

    }

    private int checkPriority(eCharacterName charName)
    {
        for (int i = 0; i < mMainPlayerOrder.Length; i++)
        {
            if (mMainPlayerOrder[i] == charName)
            {
                return i;
            }
        }
        Debug.Assert(false);
        return -1;
    }
}



public class GlobalPlayerStatus
{
    public float GlobalDamage = 0.0f;
    public float GlobalDefense = 0.0f;
    public float GlobalSpeed = 0.0f;
    public int GlobalAdditionalDashableCount = 0;
    public float GlobalAdditionalDashDistance = 0;
    public float GlobalAttackSpeedMultiplier = 0;
    public ObservedData<eInteractableType> DetectedInteractableObject;
    public ObservedData<int> ForceShieldAmount;

}

