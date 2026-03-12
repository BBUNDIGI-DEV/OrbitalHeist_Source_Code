using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractableStageMove : MonoBehaviour
{
    [SerializeField] private eSceneName sfTargetStage;


    private void OnTriggerEnter(Collider other)
    {
        if (other.tag != "Player")
        {
            return;
        }

        SceneSwitchingManager.Instance.LoadOtherScene(sfTargetStage, true);
    }
}
