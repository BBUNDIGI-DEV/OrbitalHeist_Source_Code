using System.Collections.Generic;

namespace UnityEngine.UI
{
    public class AnimatedFillUpBarUI : MonoBehaviour
    {
        [SerializeField] private Image sfHPBar;
        [SerializeField] private Image sfHpShadowBarOrNull;
        [SerializeField] private RectTransform sfEdgeIamgeOrNull;
        [SerializeField] private float sfEdgeWidth;
        [SerializeField] private float sfInitializeFillUp;
        [SerializeField] private float sfMovementFeed;
        [SerializeField] private float sfShadowFillUpDelay;

        private float mLastFillUp;
        private float mCurrentFillUp;
        private float mCurrentShadowFillUp;
        private float mDestFillUp;
        private bool mIsIncrease;

        private void Awake()
        {
            mCurrentFillUp = sfInitializeFillUp;
            mDestFillUp = float.MinValue;
        }

        [Sirenix.OdinInspector.Button]
        public void EnqueueValue(float dest)
        {
            dest = Mathf.Clamp01(dest);
            mDestFillUp = dest;
        }


        [Sirenix.OdinInspector.Button]
        public void WarpValue(float dest)
        {
            dest = Mathf.Clamp01(dest);
            mCurrentFillUp = dest;
            sfHPBar.fillAmount = dest;
            if(sfHpShadowBarOrNull != null)
            {
                sfHpShadowBarOrNull.fillAmount = dest;
            }
            mDestFillUp = float.MinValue;
        }


        private void Update()
        {
            if ( mDestFillUp == float.MinValue)
            {
                return;
            }

            mLastFillUp = mCurrentFillUp;
            mCurrentShadowFillUp = mCurrentFillUp + sfShadowFillUpDelay;
            mIsIncrease = Mathf.Sign(mDestFillUp - mCurrentFillUp) > 0.0f;

            float newFillUp = Mathf.Lerp(mCurrentFillUp, mDestFillUp, Time.deltaTime * sfMovementFeed);
            float newShadowFillUp;

            if (mIsIncrease)
            {
                newShadowFillUp = mDestFillUp;
            }
            else
            {
                newShadowFillUp = Mathf.Lerp(mCurrentShadowFillUp, mDestFillUp, Time.deltaTime * sfMovementFeed);
            }

            if (Mathf.Abs(newFillUp - mDestFillUp) < 0.005f && Mathf.Abs(newShadowFillUp - mDestFillUp) < 0.005f)
            {
                newFillUp = mDestFillUp;
                newShadowFillUp = mDestFillUp;
                mDestFillUp = float.MinValue;
            }

            sfHPBar.fillAmount = newFillUp;
            if(sfHpShadowBarOrNull != null)
            {
                sfHpShadowBarOrNull.fillAmount = Mathf.Clamp(newShadowFillUp, mDestFillUp, mLastFillUp);
            }
            if (sfEdgeIamgeOrNull != null)
            {
                sfEdgeIamgeOrNull.anchoredPosition =
                    new Vector2(Mathf.Lerp(0.0f, sfEdgeWidth, newFillUp), sfEdgeIamgeOrNull.anchoredPosition.y);
            }

            mCurrentFillUp = newFillUp;
            mCurrentShadowFillUp = newShadowFillUp;
        }
    }
}