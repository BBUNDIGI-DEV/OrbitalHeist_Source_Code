using UnityEditor;
using UnityEngine;

[CreateAssetMenu(menuName = "DataContainer/SkillAnimEventParamter")]
public class SkillAnimEventParameter : ScriptableObject
{
    public eSkillEventMarkerType SkillEventType;

#if UNITY_EDITOR
    public static SkillAnimEventParameter GetSkillAnimEventParameter(eSkillEventMarkerType actorType)
    {
        string animEventParam = $"Assets/Settings/SkillAnimEventParameter/SEP_{actorType}.asset";
        SkillAnimEventParameter paramConfig = AssetDatabase.LoadAssetAtPath<SkillAnimEventParameter>(animEventParam);
        Debug.Assert(paramConfig != null, $"You have to SkillAnimEventParameter {animEventParam}");
        return paramConfig;
    }
#endif
}
