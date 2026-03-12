using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VampireSurvivalMode : MonoBehaviour
{
    [SerializeField] private float sfSpawnRadius;
    [SerializeField] private MonsterBase[] sfMonsters;
    [SerializeField] private WaveModeSpawnData[] sfSPawnDatas;
    [SerializeField] private Transform sfMonsterParent;
    [SerializeField] private VampireSurvivalModeUI sfVSUI;

    private IEnumerator Start()
    {
        yield return new WaitForSeconds(2.0f);
        for (int i = 0; i < sfSPawnDatas.Length; i++)
        {
            sfVSUI.SetWaveText(i + 1, sfSPawnDatas.Length);
            WaveModeSpawnData curSpawnData = sfSPawnDatas[i];
            for (int l = 0; l < curSpawnData.MaxSpawnCount; l++)
            {
                yield return new WaitForSeconds(curSpawnData.MonsterSpawnTime);
                for (int j = 0; j < curSpawnData.MonsterSpawnAmountPerSpawn; j++)
                {
                    int randomMonster = Random.Range(0, sfMonsters.Length);
                    MonsterBase monsterInstance = Instantiate(sfMonsters[randomMonster], sfMonsterParent);
                    Vector3 spawnPoint = Vector3.zero;
                    for (int k = 0; k < 100; k++)
                    {
                        if (NavmeshExtension.GetCircularRandomPoint(monsterInstance.SM.Translator.Agent, PlayerManager.Instance.AcitvatedPlayerTrans.position, sfSpawnRadius, out spawnPoint))
                        {
                            break;
                        }
                    }

                    monsterInstance.SM.Translator.Trans.position = spawnPoint;
                    sfVSUI.SetRemainMonsterText(sfMonsterParent.childCount);
                    yield return new WaitForSeconds(0.5f);
                }
            }

            while(sfMonsterParent.childCount != 0)
            {
                sfVSUI.SetRemainMonsterText(sfMonsterParent.childCount);
                yield return null;
            }

            sfVSUI.SetRemainMonsterText(sfMonsterParent.childCount);
            yield return GrowthManager.Instance.StartGrowthSelection(3);
        }
    }
}


[System.Serializable]
public struct WaveModeSpawnData
{
    public int MonsterSpawnAmountPerSpawn;
    public float MonsterSpawnTime;
    public int MaxSpawnCount;
}
