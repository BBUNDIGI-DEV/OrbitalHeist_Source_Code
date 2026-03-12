using Sirenix.OdinInspector;
using UnityEngine;

namespace UnityEngine.UI
{
    [RequireComponent(typeof(RectTransform))]
    public class SimpleRecttransformSwitcher : MonoBehaviour
    {
        [SerializeField] private RectSettingData[] sfSwapData;


        public void SwitchPos<T>(T enumIndex) where T : System.Enum
        {
            SwitchPos((int)(object)enumIndex);
        }

        [Button]
        public void SwitchPos(int index)
        {
            RectTransform rt = GetComponent<RectTransform>();

            switch (sfSwapData[index].SwitchingMode)
            {
                case RectSettingData.eSwitchingMode.Default:
                    rt.anchoredPosition = sfSwapData[index].AnchorPos;
                    rt.anchoredPosition = sfSwapData[index].SizeDelta;
                    break;
                case RectSettingData.eSwitchingMode.OnlyPos:
                    rt.anchoredPosition = sfSwapData[index].AnchorPos;
                    break;
                default:
                    Debug.LogError("Default switch detected");
                    break;
            }
        }
    }

    [System.Serializable]
    public struct RectSettingData
    {
        public enum eSwitchingMode
        {
            Default,
            OnlyPos,
        }

        public eSwitchingMode SwitchingMode;

        public Vector2 AnchorPos;
        [HideIf("SwitchingMode", eSwitchingMode.OnlyPos)]
        public Vector2 SizeDelta;

#if UNITY_EDITOR
        [SerializeField, HideLabel] RectTransform sfRecttransform;

        [Button]
        private void parsingFromRT()
        {
            if(sfRecttransform == null)
            {
                return;
            }

            AnchorPos = sfRecttransform.anchoredPosition;
            SizeDelta = sfRecttransform.sizeDelta;
        }
#endif

    }
}

