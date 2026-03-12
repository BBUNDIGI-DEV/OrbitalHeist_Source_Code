using System.Collections;

namespace UnityEngine.UI
{
    public class SceneFadeIn : MonoBehaviour
    {
        private Image mFadeImg;

        private void Awake()
        {
            mFadeImg = GetComponent<Image>();
        }

        void Start()
        {
            StartCoroutine(fadeIn());
        }

        IEnumerator fadeIn()
        {
            float fadeCount = 1f;
            while (fadeCount >= 0f)
            {
                fadeCount -= 0.01f;
                yield return new WaitForSeconds(0.01f);
                mFadeImg.color = new Color(0, 0, 0, fadeCount);
            }
        }
    }

}
