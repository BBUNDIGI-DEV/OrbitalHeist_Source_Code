using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;

public class MonsterSpawner : MonoBehaviour
{
    public bool IsFullSpawned
    {
        get
        {
            return mCurrentSpawnIndex == mSpawnCount;
        }
    }

    [field:SerializeField] public bool IsMultiSpawn
    {
        get; private set;
    }

    [SerializeField, ShowIf("IsMultiSpawn")] private int mSpawnCount;
    private static Transform SM_MONSTER_PARENT;

    [SerializeField, LabelText("Spawn Type")] private eSpawnType mSpawnType;
    [SerializeField, ShowIf("mSpawnType", eSpawnType.RandomCustom)] private eMonsterName[] mCustomMonsterList;
    [SerializeField, ShowIf("mSpawnType", eSpawnType.Specify)] private eMonsterName mSpecifyiedMonster;

    [SerializeField, LabelText("Spawn Point Type")] private eSpawnPointType mSpawnPointType;
    [SerializeField, ShowIf("mSpawnPointType", eSpawnPointType.RandomInCircle)] private float mRange;
    private int mCurrentSpawnIndex;

    private void Awake()
    {
        if (SM_MONSTER_PARENT == null)
        {
            SM_MONSTER_PARENT = GameObject.FindGameObjectWithTag("MonsterParent").transform;
        }
    }


    public MonsterBase SpawnMonster()
    {
        MonsterBase instantiatedMonster = null;
        switch (mSpawnType)
        {
            case eSpawnType.RandomNormalMonster:
                instantiatedMonster = spawnMonster(RuntimeDataLoader.GlobalMonsterConfig.NormalMonsters);
                break;
            case eSpawnType.RandomEliteMonster:
                instantiatedMonster = spawnMonster(RuntimeDataLoader.GlobalMonsterConfig.EliteMonsters);
                break;
            case eSpawnType.RandomCustom:
                instantiatedMonster = spawnMonster(mCustomMonsterList);
                break;
            case eSpawnType.Specify:
                instantiatedMonster = spawnMonster(mSpecifyiedMonster);
                break;
        }

        if(IsMultiSpawn)
        {
            mCurrentSpawnIndex++;
        }

        return instantiatedMonster;
    }


    private MonsterBase spawnMonster(eMonsterName[] monsterArray)
    {
        int randomMonsterIndex = Random.Range(0, monsterArray.Length);
        return spawnMonster(monsterArray[randomMonsterIndex]);
    }

    private MonsterBase spawnMonster(eMonsterName monsterName)
    {
        MonsterBase originalMonster = RuntimeDataLoader.GlobalMonsterConfig.GetMonsterByName(monsterName);
        Vector3 position = transform.position;

        switch (mSpawnPointType)
        {
            case eSpawnPointType.None:
                break;
            case eSpawnPointType.RandomInCircle:
                bool result = NavmeshExtension.GetCircularRandomPoint(transform.position, mRange, out position);
                if (result == false)
                {
                    position = transform.position;
                }
                break;
            default:
                break;
        }

        MonsterBase spawnedMonster = Instantiate(originalMonster, SM_MONSTER_PARENT);
        spawnedMonster.Initialized(position, transform.rotation);
        return spawnedMonster;
    }
#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        if(mSpawnPointType == eSpawnPointType.RandomInCircle)
        {
            Handles.color = Color.cyan;
            Handles.DrawWireDisc(transform.position + Vector3.up, Vector3.up, mRange);
            return;
        }

        if (mSpawnType != eSpawnType.Specify)
        {
            return;
        }

        if (RuntimeDataLoader.GlobalMonsterConfig == null)
        {
            return;
        }
        MonsterBase monster = null;
        MonsterBase[] fullMonsterArray = RuntimeDataLoader.GlobalMonsterConfig.FullMonsterArray;
        for (int i = 0; i < fullMonsterArray.Length; i++)
        {
            if (fullMonsterArray[i].MonsterConfig.MonsterName == mSpecifyiedMonster)
            {
                monster = fullMonsterArray[i];
                break;
            }
        }

        if(monster == null)
        {
            return;
        }

        Handles.matrix =  transform.localToWorldMatrix;
        Handles.DrawWireCube(Vector3.zero, new Vector3(1.0f, 5.0f, 1.0f));
        Handles.matrix = Matrix4x4.identity;
        Handles.color = Color.yellow;
        Handles.DrawWireDisc(transform.position + Vector3.up, Vector3.up, monster.MonsterConfig.AIMovementConfig.DetectionRange);

        Handles.color = Color.red;
        if(monster.MonsterConfig.AISkillConfig == null)
        {
            return;
        }
        switch (monster.MonsterConfig.AISkillConfig.CombatType)
        {
            case eCombatType.MeleeAttackInRange:
                Handles.DrawWireDisc(transform.position + Vector3.up, Vector3.up, monster.MonsterConfig.AISkillConfig.MeleeAttackRange);
                break;
            case eCombatType.ShootProjectileTowardPlayer:
                Handles.DrawWireDisc(transform.position + Vector3.up, Vector3.up, monster.MonsterConfig.AISkillConfig.ProjectileMinRange);
                Handles.DrawWireDisc(transform.position + Vector3.up, Vector3.up, monster.MonsterConfig.AISkillConfig.ProjectileMaxRange);
                break;
        }
    }

    [Button]
    private void selectMonsterPrefab()
    {
        if (mSpawnType != eSpawnType.Specify)
        {
            return;
        }

        MonsterBase monster = null;
        MonsterBase[] fullMonsterArray = RuntimeDataLoader.GlobalMonsterConfig.FullMonsterArray;
        for (int i = 0; i < fullMonsterArray.Length; i++)
        {
            if (fullMonsterArray[i].MonsterConfig.MonsterName == mSpecifyiedMonster)
            {
                monster = fullMonsterArray[i];
                break;
            }
        }

        Selection.activeObject = monster;

    }
#endif
}


public enum eSpawnType
{
    RandomNormalMonster,
    RandomEliteMonster,
    RandomCustom,
    Specify,
}

public enum eSpawnPointType
{
    None,
    RandomInCircle,
}

