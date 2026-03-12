using UnityEngine;

public class RuntimeStageData : ScriptableObject
{
    public ObservedData<int> TotalMonsterCount;
    public ObservedData<int> LastMonsterCount;
    public ObservedData<int> CurrentWaveIndex;
    public ObservedData<bool> IsStageStarted;
    public ObservedData<bool> IsStageCleared;
    public ObservedData<int> MaxToxicCount;
    public ObservedData<int> CurrentToxicCount;
    public ObservedData<float> ToxicGuage;
    [HideInInspector] public ObservedData<Vector3> EndPlayerPosition;
}
