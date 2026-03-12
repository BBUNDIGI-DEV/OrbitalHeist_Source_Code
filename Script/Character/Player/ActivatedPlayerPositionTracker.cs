using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActivatedPlayerPositionTracker : MonoBehaviour
{
    // Update is called once per frame
    void Update()
    {
        if(PlayerManager.Instance.AcitvatedPlayerTrans == null)
        {
            return;
        }

        transform.position = PlayerManager.Instance.AcitvatedPlayerTrans.position;
    }
}
