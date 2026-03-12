using UnityEngine;
using DG.Tweening;

public class ThrowingAttackBox : MonoBehaviour
{
    [SerializeField] private Collider sfTargetAttackBox;

    private void OnEnable()
    {
        sfTargetAttackBox.gameObject.SetActive(false);
    }

    public void MoveToDest(Vector3 destPos, float duration)
    {
        GetComponentInChildren<LegacyAnimSpeedMultipilier>().SetSpeed(1 / duration);
        transform.DOMove(destPos, duration).OnComplete(onMoveComplete);
    }

    private void onMoveComplete()
    {
        sfTargetAttackBox.gameObject.SetActive(true);
    }
}

