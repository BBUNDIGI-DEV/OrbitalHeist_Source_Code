using UnityEngine;
using UnityEditor;

[ExecuteInEditMode]
public class TransfromLocker : MonoBehaviour
{
    [SerializeField] private Transform sfTarget;

#if UNITY_EDITOR
    private void Awake()
    {
        if(Application.isPlaying)
        {
            Destroy(this);
        }
    }

    void Update()
    {
        if(transform.localPosition.magnitude >= 0.1f)
        {
            Debug.LogError($"이동이 불가능한 게임오브젝트 입니다.[{gameObject.name}]의 위치는 항상 (0,0,0)이여야 합니다.");
            transform.localPosition = Vector3.zero;
            if(sfTarget != null)
            {
                Debug.LogError($"대신 [{sfTarget.name}]을 이용해주세요(자동으로 선택됨)");
                Selection.activeTransform = sfTarget;
                Selection.SetActiveObjectWithContext(sfTarget.gameObject, sfTarget);
            }
        }
    }
#endif
}
