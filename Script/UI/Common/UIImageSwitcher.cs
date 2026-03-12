using Sirenix.OdinInspector;
using System.Runtime.CompilerServices;

namespace UnityEngine.UI
{
    [RequireComponent(typeof(Image))]
    public class UIImageSwitcher : MonoBehaviour
    {
        [SerializeField] private ImageSwapData[] sfSwapData;


        public void SwitchSprite<T>(T enumIndex) where T : System.Enum
        {
            int index = Unsafe.As<T, int>(ref enumIndex);
            SwitchSprite(index);
        }

        [Button]
        public void SwitchSprite(int index)
        {
            Image image = GetComponent<Image>();
            RectTransform rt = GetComponent<RectTransform>();
            image.sprite = sfSwapData[index].TargetSprite;

            switch (sfSwapData[index].SwitchingMode)
            {
                case ImageSwapData.eSwitchingMode.OnlySprite:
                    break;
                case ImageSwapData.eSwitchingMode.SpriteAndSetNativeSize:
                    image.SetNativeSize();
                    break;
                case ImageSwapData.eSwitchingMode.SpriteAndPos:
                    image.SetNativeSize();
                    image.GetComponent<RectTransform>().anchoredPosition = sfSwapData[index].AnchorPos;
                    break;
                case ImageSwapData.eSwitchingMode.SpriteAndPosNoSetNatvieSize:
                    image.GetComponent<RectTransform>().anchoredPosition = sfSwapData[index].AnchorPos;
                    break;
                case ImageSwapData.eSwitchingMode.SpriteAndPosWithSize:
                    image.GetComponent<RectTransform>().anchoredPosition = sfSwapData[index].AnchorPos;
                    image.GetComponent<RectTransform>().sizeDelta = sfSwapData[index].SizeDelta;
                    break;
                default:
                    Debug.LogError("Default switch detected");
                    break;
            }
        }
    }

    [System.Serializable]
    public struct ImageSwapData
    {
        public enum eSwitchingMode
        {
            OnlySprite,
            SpriteAndSetNativeSize,
            SpriteAndPos,
            SpriteAndPosNoSetNatvieSize,
            SpriteAndPosWithSize,
        }

        public eSwitchingMode SwitchingMode;
        public Sprite TargetSprite;

        [ShowIf("@SwitchingMode == eSwitchingMode.SpriteAndPos || SwitchingMode == eSwitchingMode.SpriteAndPosNoSetNatvieSize || SwitchingMode == eSwitchingMode.SpriteAndPosWithSize")]
        public Vector2 AnchorPos;

        [ShowIf("@SwitchingMode == eSwitchingMode.SpriteAndPosWithSize")]
        public Vector2 SizeDelta;
    }
}






