using UnityEngine;
using UnityEditor;

public class MonsterActingDebugger : MonoBehaviour
{
    [SerializeField] private MonsterConfig sfConfig;

    private void Awake()
    {
        if(!Application.isEditor)
        {
            Destroy(this);
        }
    }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        if(sfConfig == null)
        {
            return;
        }

        Handles.color = Color.blue;
        Handles.DrawWireDisc(transform.position, Vector3.up, sfConfig.AIMovementConfig.DetectionRange);
        //Handles.color = Color.red;
        //Handles.DrawWireDisc(transform.position, Vector3.up, sfConfig.AISkillConfig.MeleeAttackRange);
    }
#endif
}
