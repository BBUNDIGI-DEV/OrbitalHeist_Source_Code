using UnityEngine;
using Sirenix.OdinInspector;

[CreateAssetMenu(menuName = "DataContainer/InfoMessageUI")]
public class InfoMessageUIConfig : ScriptableObject
{
    public eInfoMessageInvokeTiming InvokeTiming;

    [ShowIf("@InvokeTiming.HasFlag(eInfoMessageInvokeTiming.OnStageStarted)")] public string StageStartMessage;
    [ShowIf("@InvokeTiming.HasFlag(eInfoMessageInvokeTiming.OnStageCleared)")] public string StageClearMessage;


    public float[] IncreaseMessageInvokeThreshold;
    public string[] IncreaseInfoMessage;

    public float[] DecreaseMessageInvokeThreshold;
    public float[] DecreaseMessageMaxThreshold;
    public string[] DecreaseInfoMessage;

    public float RemaningDuration;

#if UNITY_EDITOR
    private void OnValidate()
    {
        Debug.Assert(IncreaseMessageInvokeThreshold.IsDescendingSorted());
        Debug.Assert(DecreaseMessageInvokeThreshold.IsDescendingSorted());
        Debug.Assert(DecreaseMessageMaxThreshold.IsDescendingSorted());
    }
#endif
}

[System.Flags]
public enum eInfoMessageInvokeTiming
{
    OnStageStarted = 1 << 1,
    OnStageCleared = 1 << 2,
}