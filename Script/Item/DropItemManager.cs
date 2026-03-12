using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DropItemManager : MonoBehaviour
{
    public void Start()
    {
        SceneSwitchingManager.Instance.CurrentScene.AddListener(ClearAllItems);
    }

    public void OnDestroy()
    {
        if(SceneSwitchingManager.IsExist)
        {
            SceneSwitchingManager.Instance.CurrentScene.RemoveListener(ClearAllItems);
        }
    }

    public void ClearAllItems()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            Destroy(transform.GetChild(i).gameObject);
        }
    }
}
