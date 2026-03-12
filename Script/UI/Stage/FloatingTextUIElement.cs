
using TMPro;

namespace UnityEngine.UI
{
    public class FloatingTextUIElement : PoolableMono
    {
        [SerializeField] private TMP_Text sfText;
        [SerializeField] private Animation sfAnimation;
        private float mDefaultFontSize = -1;
        public void SetText(FloatingInfoMessageConfig config)
        {
            if(mDefaultFontSize == -1)
            {
                mDefaultFontSize = sfText.fontSize;
            }
            sfText.text = config.Contents;
            sfText.colorGradientPreset = config.TextColor;
            sfText.fontSize = mDefaultFontSize * config.FontSizeMulitplier;
            sfText.gameObject.SetActive(true);
        }

        public void DisableUnityEvent()
        {
            sfText.gameObject.SetActive(false);
        }
    }
}