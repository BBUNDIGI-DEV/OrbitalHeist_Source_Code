using UnityEngine.EventSystems;

namespace UnityEngine.UI
{
    public class MobileJoystick : MobileDraggingUI
    {
        public Vector2 DeltaStickPos
        {
            get
            {
                Vector2 deltaStickPos = (Vector2)sfStickRect.position - mOriginPos;
                if (deltaStickPos.sqrMagnitude < DEAD_ZONE * DEAD_ZONE)
                {
                    return Vector2.zero;
                }
                else
                {
                    return deltaStickPos;
                }
            }
        }

        private const float DEAD_ZONE = 0.1f;
        [SerializeField] private RectTransform sfStickRect;
        [SerializeField] private float sfMaxRadius;
        private Vector2 mOriginPos;

        public void Awake()
        {
            Canvas canavs = GetComponentInParent<Canvas>();
            mOriginPos = sfStickRect.position * (Vector2)canavs.transform.localScale;
            Debug.Log(sfStickRect.lossyScale);
            Debug.Log(canavs.transform.localScale);
            Debug.Log(mOriginPos);
        }

        public override void OnBeginDrag(PointerEventData eventData)
        {
            sfStickRect.position = clampInRadius(sfMaxRadius, mOriginPos, eventData.position);
        }

        public override void OnDrag(PointerEventData eventData)
        {
            sfStickRect.position = clampInRadius(sfMaxRadius, mOriginPos, eventData.position);
        }

        public override void OnEndDrag(PointerEventData eventData)
        {
            sfStickRect.position = mOriginPos;
        }

        private Vector2 clampInRadius(float radius, Vector2 origin, Vector2 target)
        {
            Vector2 deltaVector = target - origin;
            deltaVector = Vector2.ClampMagnitude(deltaVector, sfMaxRadius);
            return deltaVector + origin;
        }
    }
}
