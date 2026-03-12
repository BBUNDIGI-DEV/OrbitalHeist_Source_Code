using UnityEngine;

[CreateAssetMenu(menuName = "DataContainer/AimAssistanceConfig")]
public class AimAssistanceConfig : DataConfigBase
{
    [Range(0, 180)] public float AssistanceDegree;
    public AnimationCurve AdjustCurve;
    public float MaxAssistanceDistance;

    public float EvaluateAngle(float factor)
    {
        factor = Mathf.Clamp01(factor);
       float curveValue = AdjustCurve.Evaluate(factor);
       return Mathf.Lerp(0, AssistanceDegree, curveValue);
    }
}