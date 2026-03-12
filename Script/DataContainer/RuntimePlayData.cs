using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "RuntimeData/RuntimePlayData")]
public class RuntimePlayData : ScriptableObject
{
    [HideInInspector] public ObservedData<bool> IsQuitGame;
    [HideInInspector] public ObservedData<bool> IsPaused;
    [HideInInspector] public ObservedData<bool> IsScenePlaying;
    [HideInInspector] public ObservedData<bool> IsMonsterActorBlocked;
    //[HideInInspector] public ObservedData<bool> IsFadeBackground;
    //[HideInInspector] public ObservedData<bool> IsStageStarted;
    //[HideInInspector] public ObservedData<bool> IsStageCleared;
}

