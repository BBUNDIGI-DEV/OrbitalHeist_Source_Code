using Sirenix.OdinInspector;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class SectionElement : MonoBehaviour
{
    public static MonsterBase sLastDeadMonster;
    public static bool sIsOnBattle;

    public bool IsCleared
    {
        get; private set;
    }
    [SerializeField] private bool sfIsBossStage;
    [SerializeField, LabelText("On Section Started")] private UnityEvent sfOnSectionStarted;
    [SerializeField, LabelText("On Battle End")] private UnityEvent sfOnBattleCleared;
    [SerializeField, LabelText("On Section Cleared")] private UnityEvent sfOnSectionCleared;
    [SerializeField, LabelText("ClearType")] private eSectionClearType sfSectionClearType;
    [SerializeField, LabelText("Survival Time"), ShowIf("sfSectionClearType", eSectionClearType.Survive)] private float sfSurviveTime;
    [SerializeField, LabelText("Target Monster"), ShowIf("sfSectionClearType", eSectionClearType.HuntingSpecifiedMonster)] private MonsterBase sfTargetMonster;
    [SerializeField, LabelText("MaxItemSpawnCount")] private int sfMaxItemSpawnCount = -1;


    private GameObject mVirtualWall;
    private GameObject mWaveObject;
    private InteractableGrowthItem mRewardItem;

    private bool mForceStopSpawn;
    private bool mIsWaveRoutineDone;
    private float mCurrentSurviveTime;

    private void Awake()
    {
        mVirtualWall = transform.FindRecursiveGameobjectWithTag("SectionVirtualWall");
        mWaveObject = transform.FindRecursiveGameobjectWithTag("SectionWave");
        GameObject rewardItem = transform.FindRecursiveGameobjectWithTag("SectionRewardItem");
        if(rewardItem != null)
        {
            mRewardItem = rewardItem.GetComponent<InteractableGrowthItem>();
        }
        if (mVirtualWall != null)
        {
            mVirtualWall.SetActive(false);
        }
        sIsOnBattle = false;
    }

    public void OnSectionStarted()
    {
        sIsOnBattle = true;
        if (mVirtualWall != null)
        {
            mVirtualWall.SetActive(true);
        }
        if (sfIsBossStage)
        {
            BGMManager.Instance.SetMainBGMAsBoss();
        }
        else
        {
            BGMManager.Instance.SetMainBGMAsBattle();
        }

        sfOnSectionStarted?.Invoke();
        ItemDataUtil.sSpawnLimit = sfMaxItemSpawnCount;
        switch (sfSectionClearType)
        {
            case eSectionClearType.ClearAllWave:
                StartCoroutine(clearAllWaveRoutine());
                break;
            case eSectionClearType.HuntingSpecifiedMonster:
                StartCoroutine(huntingSpecifiedMonsterRoutine());
                break;
            case eSectionClearType.Survive:
                StartCoroutine(survivalRoutine());
                break;
            default:
                break;
        }
        sLastDeadMonster = null;
        InteractableBase[] interatableObjects = GetComponentsInChildren<InteractableBase>();

        for (int i = 0; i < interatableObjects.Length; i++)
        {
            interatableObjects[i].ToggleInteractableObject(false);
        }
    }

    private void makeMonsterForceTrackingPlayer()
    {
        GameObject monsterParent = GameObject.FindGameObjectWithTag("MonsterParent");
        CommonMonsterController[] mc = monsterParent.GetComponentsInChildren<CommonMonsterController>();
        for (int i = 0; i < mc.Length; i++)
        {
            AIMovementActor movementActor = mc[i].SM.TryGetActorOrNull<AIMovementActor>(eActorType.AIMovement);
            movementActor.OnPlayerFounded();
        }
    }

    private IEnumerator clearAllWaveRoutine()
    {
        //showInfoMessage("적을 섬멸하세요");
        yield return StartCoroutine(spawnWave());
        yield return StartCoroutine(onSectionRoutineEnd());
        onSectionCleared();
    }

    private IEnumerator survivalRoutine()
    {
        StartCoroutine(spawnWave());
        mCurrentSurviveTime = 0.0f;

        while (true)
        {
            mCurrentSurviveTime += Time.deltaTime;
            if (mCurrentSurviveTime >= sfSurviveTime)
            {
                break;
            }

            if (UIManager.IsExist)
            {
                int remainSurviveTime = (int)(sfSurviveTime - mCurrentSurviveTime);
                int minutes = remainSurviveTime / 60;
                int seconds = remainSurviveTime % 60;
                string surviveText;

                if (minutes != 0)
                {
                    surviveText = $"{minutes}분, {seconds}초간 생존하세요";
                }
                else
                {
                    surviveText = $"{seconds}초간 생존하세요";
                }
                //showInfoMessage(surviveText);
            }
            yield return null;
        }

        mForceStopSpawn = true;
        mCurrentSurviveTime = float.MaxValue;
        makeMonsterForceTrackingPlayer();
        yield return new WaitUntil(() => mIsWaveRoutineDone);
        yield return StartCoroutine(onSectionRoutineEnd());
        onSectionCleared();
    }

    private IEnumerator huntingSpecifiedMonsterRoutine()
    {
        //showInfoMessage("강철 포탑을 무너트리세요");
        StartCoroutine(spawnWave());

        yield return new WaitUntil(() => sfTargetMonster.MonsterStatus.IsDead);
        mForceStopSpawn = true;
        makeMonsterForceTrackingPlayer();
        yield return new WaitUntil(() => mIsWaveRoutineDone);

        yield return StartCoroutine(onSectionRoutineEnd());
        onSectionCleared();
    }

    private void onSectionCleared()
    {
        if (mVirtualWall != null)
        {
            mVirtualWall.SetActive(false);
        }
        BGMManager.Instance.SetMainBGMAsNormal();
        IsCleared = true;
        sfOnSectionCleared?.Invoke();
        ItemDataUtil.sSpawnLimit = -1;
        InteractableBase[] interatableObjects = GetComponentsInChildren<InteractableBase>();
        for (int i = 0; i < interatableObjects.Length; i++)
        {
            interatableObjects[i].ToggleInteractableObject(true);
        }
    }

    private IEnumerator onSectionRoutineEnd()
    {
        if (sLastDeadMonster != null)
        {
            SectionClearedPD.Instance.PlaySectionCleared(sLastDeadMonster);
        }
        sIsOnBattle = false;
        yield return new WaitWhile(() => SectionClearedPD.Instance.IsPlayed);
        sfOnBattleCleared?.Invoke();
        if (mRewardItem != null)
        {
            mRewardItem.gameObject.SetActive(true);

            while (mRewardItem.gameObject.activeInHierarchy)
            {
                yield return null;
            }
        }
    }


    private IEnumerator spawnWave()
    {
        mForceStopSpawn = false;
        mIsWaveRoutineDone = false;
        for (int i = 0; i < mWaveObject.transform.childCount; i++)
        {
            if(mForceStopSpawn)
            {
                break;
            }

            Transform currentWave = mWaveObject.transform.GetChild(i);
            if (!currentWave.gameObject.activeInHierarchy)
            {
                continue;
            }

            MonsterSpawnManager[] childSpawnManagers = mWaveObject.transform.GetChild(i).GetComponentsInChildren<MonsterSpawnManager>();

            for (int j = 0; j < childSpawnManagers.Length; j++)
            {
                childSpawnManagers[j].StartSpawnManager();
            }

            while (true)
            {
                if(mForceStopSpawn)
                {
                    for (int j = 0; j < childSpawnManagers.Length; j++)
                    {
                        childSpawnManagers[j].StopSpawn_UE();
                    }
                }

                bool isAllSpawnManagerCleared = true;
                for (int j = 0; j < childSpawnManagers.Length; j++)
                {
                    if (!childSpawnManagers[j].Cleared)
                    {
                        isAllSpawnManagerCleared = false;
                        break;
                    }
                }
                if (isAllSpawnManagerCleared)
                {
                    break;
                }
                yield return null;
            }
        }
        mIsWaveRoutineDone = true;
    }
}

public enum eSectionClearType
{
    ClearAllWave,
    HuntingSpecifiedMonster,
    Survive,
}
