using Sirenix.OdinInspector;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class MonsterSpawnManager : MonoBehaviour
{
    public bool Cleared
    {
        get
        {
            if(mSpawners == null)
            {
                return mSpawnedMonsterCount == 0;
            }
            return mSpawnedMonsterCount == 0 && (mSpawnerIndex == mSpawners.Length || mIsForceStopped);
        }
    }

    private static Transform SM_MONSTER_PARENT;

    [SerializeField, LabelText("Spawn Started")] private UnityEvent sfOnSpawnStarted;
    [SerializeField, LabelText("Spawn Delay Per Each entity spawn")] private float DELAY_PER_EACH_SPAWN;

    [SerializeField, LabelText("SpawnStrategy")] private eSpawnStrategy M_SPAWN_STRATEGY;
    [ShowIf("M_SPAWN_STRATEGY", eSpawnStrategy.Time)] public float M_SPAWN_DELAY;
    [ShowIf("M_SPAWN_STRATEGY", eSpawnStrategy.Time)] public int M_SPAWN_AMOUNT_PER_TIME;

    [ShowIf("M_SPAWN_STRATEGY", eSpawnStrategy.UnderCount)] public int M_INITIAL_SPAWN_COUNT;
    [ShowIf("M_SPAWN_STRATEGY", eSpawnStrategy.UnderCount)] public int M_LEAST_SPAWN_COUNT;
    [ShowIf("M_SPAWN_STRATEGY", eSpawnStrategy.UnderCount)] public int M_REFILL_COUNT = 1;
    [ShowIf("M_SPAWN_STRATEGY", eSpawnStrategy.UnderCount)] public float M_REFILL_DELAY = 0.1f;

    private bool mIsForceStopped;
    private Coroutine mSpawnRoutine;
    private MonsterSpawner[] mSpawners;
    private int mSpawnedMonsterCount;
    private int mSpawnerIndex;

    private void Awake()
    {
        mSpawners = GetComponentsInChildren<MonsterSpawner>(true);
        if (SM_MONSTER_PARENT == null)
        {
            SM_MONSTER_PARENT = GameObject.FindGameObjectWithTag("MonsterParent").transform;
        }

        MonsterBase[] prespawnedMonsters = GetComponentsInChildren<MonsterBase>(true);

        for (int i = 0; i < prespawnedMonsters.Length; i++)
        {
            prespawnedMonsters[i].SetAsSleepMode();
        }
    }

    public void StartSpawnManager()
    {
        sfOnSpawnStarted?.Invoke();
        MonsterBase[] prespawnedMonsters = GetComponentsInChildren<MonsterBase>(true);
        for (int i = 0; i < prespawnedMonsters.Length; i++)
        {
            prespawnedMonsters[i].transform.SetParent(SM_MONSTER_PARENT);
            Vector3 spawnPosition = prespawnedMonsters[i].transform.position;
            Quaternion spawnRotation = prespawnedMonsters[i].transform.rotation;
            prespawnedMonsters[i].transform.position = Vector3.zero;
            prespawnedMonsters[i].transform.rotation = Quaternion.identity;
            prespawnedMonsters[i].Initialized(spawnPosition, spawnRotation);
            prespawnedMonsters[i].OnMonsterDead += onMonsterDead;
            mSpawnedMonsterCount++;
        }

        if (mSpawners == null)
        {
            return;
        }

        mSpawnRoutine = StartCoroutine(spawnRoutine());
    }

    public void StopSpawn_UE() //Called by stop spawning when section goal is archived
    {
        StopCoroutine(mSpawnRoutine);
        mIsForceStopped = true;
    }

    private IEnumerator spawnRoutine()
    {
        yield return new WaitForSeconds(DELAY_PER_EACH_SPAWN);
        switch (M_SPAWN_STRATEGY)
        {
            case eSpawnStrategy.SpawnAtOnce:
                for (mSpawnerIndex = 0; mSpawnerIndex < mSpawners.Length;)
                {
                    trySpawnMonster();
                    if (DELAY_PER_EACH_SPAWN <= 0.0f)
                    {
                        continue;
                    }
                    yield return new WaitForSeconds(DELAY_PER_EACH_SPAWN);
                }
                break;
            case eSpawnStrategy.Time:
                while (mSpawnerIndex < mSpawners.Length)
                {
                    for (int i = 0; i < M_SPAWN_AMOUNT_PER_TIME; i++)
                    {
                        if(mSpawnerIndex == mSpawners.Length)
                        {
                            break;
                        }

                        trySpawnMonster();
                        if (DELAY_PER_EACH_SPAWN <= 0.0f)
                        {
                            continue;
                        }
                        yield return new WaitForSeconds(DELAY_PER_EACH_SPAWN);
                    }
                    yield return new WaitForSeconds(M_SPAWN_DELAY);
                }
                break;
            case eSpawnStrategy.UnderCount:
                for (int i = 0; i < M_INITIAL_SPAWN_COUNT; i++)
                {
                    trySpawnMonster();
                    if (DELAY_PER_EACH_SPAWN <= 0.0f)
                    {
                        continue;
                    }
                    yield return new WaitForSeconds(DELAY_PER_EACH_SPAWN);
                }

                while (mSpawnerIndex < mSpawners.Length)
                {
                    if(mSpawnedMonsterCount < M_LEAST_SPAWN_COUNT)
                    {
                        yield return new WaitForSeconds(M_REFILL_DELAY);
                        for (int i = 0; i < M_REFILL_COUNT; i++)
                        {
                            if (mSpawnerIndex == mSpawners.Length)
                            {
                                break;
                            }
                            trySpawnMonster();
                            yield return new WaitForSeconds(DELAY_PER_EACH_SPAWN);
                        }
                    }
                    yield return null;
                }
                break;
            default:
                break;
        }
    }

    private void trySpawnMonster()
    {
        MonsterBase spawnedMonster = mSpawners[mSpawnerIndex].SpawnMonster();
        spawnedMonster.OnMonsterDead += onMonsterDead;
        mSpawnedMonsterCount++;
        if (mSpawners[mSpawnerIndex].IsMultiSpawn && !mSpawners[mSpawnerIndex].IsFullSpawned)
        {
            return;
        }
        mSpawnerIndex++;
    }

    private void onMonsterDead(MonsterBase deadMonster)
    {
        mSpawnedMonsterCount--;
        SectionElement.sLastDeadMonster = deadMonster;
    }
}


public enum eSpawnStrategy
{
    SpawnAtOnce,
    Time,
    UnderCount,
}

