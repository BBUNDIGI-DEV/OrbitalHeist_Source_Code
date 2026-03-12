using UnityEngine;

public static class ItemDataUtil 
{
    public static int sSpawnLimit = -1; 
    public static ItemData[] sAllDatas;
    public static GameObject[] sItemPrefabs;
    public static Transform sItemParent
    {
        get
        {
            if(msItemParent == null)
            {
                msItemParent = GameObject.FindGameObjectWithTag("DropItemParent").transform;
            }
            return msItemParent;
        }
    }

    public static Transform msItemParent;


    public static void LoadData()
    {
        sAllDatas = Resources.LoadAll<ItemData>("ItemData");
        sItemPrefabs = Resources.LoadAll<GameObject>("ItemData/Prefab");
    }

    public static ItemElement TrySpawnItem(Vector3 spawnPoint)
    {
        if(sSpawnLimit == 0)
        {
            return null;
        }

        if(sAllDatas == null)
        {
            LoadData();
        }

        for (int i = 0; i < sAllDatas.Length; i++)
        {
            float randomChance = Random.Range(0.0f, 1.0f);
            if (randomChance < sAllDatas[i].DropRate)
            {
                ItemElement clonedItem = Object.Instantiate(sItemPrefabs[i], sItemParent).GetComponent<ItemElement>();
                clonedItem.transform.position = spawnPoint;
                clonedItem.EnableItem();
                sSpawnLimit--;
                return clonedItem;
            }
        }
        return null;
    }
}
