using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckPlayerDeathTimeline : MonoBehaviour
{
    [SerializeField] GameObject sfPlayerDeathTimeline;

    private RuntimePlayData mPlayData
    {
        get
        {
            return RuntimeDataLoader.RuntimePlayData;
        }
    }

    private void Start()
    {
        GetComponentInParent<PlayerCharacterController>().CharacterStatus.IsDead.AddListener(playPlayerDeathTimeline, true);
    }

    private void playPlayerDeathTimeline(bool isPlayerDead)
    {
        if(isPlayerDead)
        {
            mPlayData.IsScenePlaying.Value = true;
            sfPlayerDeathTimeline.SetActive(true);
        }
    }
}
