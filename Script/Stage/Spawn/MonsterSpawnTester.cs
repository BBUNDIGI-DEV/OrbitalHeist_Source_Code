using System.Collections;
using UnityEngine;

public class MonsterSpawnTester : MonoBehaviour
{
    public void SpawnMonster_UE(Object originalMonster)
    {
        MonsterBase monster = Instantiate(originalMonster as GameObject, transform.position, transform.rotation).GetComponent<MonsterBase>();
        monster.Initialized();
    }
}
 