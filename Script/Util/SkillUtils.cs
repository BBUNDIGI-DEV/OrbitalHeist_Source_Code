using UnityEngine;

public static class SkillUtils
{
    public static float ConvertInspectorAccelToMultiplier(float inspectorValue, float maxAcceleration)
    {
        if (inspectorValue == 0.0f)
        {
            return 1.0f;
        }
        else if (inspectorValue > 0.0f)
        {
            return Mathf.Lerp(1, 1 / maxAcceleration, Mathf.Abs(inspectorValue));
        }
        else
        {
            return Mathf.Lerp(1, maxAcceleration, Mathf.Abs(inspectorValue));
        }
    }
}

