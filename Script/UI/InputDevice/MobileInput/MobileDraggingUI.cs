using UnityEngine.EventSystems;

namespace UnityEngine.UI
{
    public abstract class MobileDraggingUI : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler
    {
        public Vector2 DeltaPointerPos
        {
            get; private set;
        }

        [SerializeField] private float sfReleaseTime;
        private Vector2 mPrevMousePos;

        public virtual void OnBeginDrag(PointerEventData eventData)
        {
            DeltaPointerPos = eventData.position;
            mPrevMousePos = eventData.position;
        }

        public virtual void OnDrag(PointerEventData eventData)
        {
            DeltaPointerPos = eventData.position - mPrevMousePos;
            mPrevMousePos = eventData.position;
        }

        public virtual void OnEndDrag(PointerEventData eventData)
        {
            mPrevMousePos = eventData.position;
        }
    }
}