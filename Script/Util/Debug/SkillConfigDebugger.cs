using UnityEngine;
using UnityEditor;
[RequireComponent(typeof(Rigidbody))]
public class SkillConfigDebugger : MonoBehaviour
{
    [SerializeField] private SkillConfig sfConfig;
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

        Vector3 attackDir = transform.forward;
        if (Application.isPlaying)
        {
            attackDir = InputManager.Instance.GetAttackAim(GetComponent<Transform>());
        }

        Handles.color = Color.green;
        const float DRAW_FREQUENCY = 30;
        for (int i = 0; i < DRAW_FREQUENCY; i++)
        {
            float factor = i / DRAW_FREQUENCY;
            float angle = sfConfig.AimAisstanceConfig.EvaluateAngle(factor) * 2;
            float curDistance = Mathf.Lerp(0, sfConfig.AimAisstanceConfig.MaxAssistanceDistance, factor);
            HandlesDrawUtil.DrawWireArc(transform.position, Vector3.up, attackDir, angle, curDistance);
        }

        Handles.color = Color.red;
        Handles.DrawWireDisc(transform.position, Vector3.up, sfConfig.AimAisstanceConfig.MaxAssistanceDistance);

        //Handles.DrawLine(transform.position, transform.position + attackDir * sfConfig.AimAisstanceConfig.MaxAssistanceDistance);
    }
#endif
}