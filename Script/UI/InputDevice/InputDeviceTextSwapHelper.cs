using TMPro;

namespace UnityEngine.UI
{
    public class InputDeviceTextSwapHelper : DestoryOnlyIfAwakeMonoBehavior
    {
        [SerializeField] private string sfKeyBoardText;
        [SerializeField] private string sfGamePadText;
        [SerializeField] private string sfMobileText;

        private void Start()
        {
            InputManager.Instance.LoadedInputHandler.CurrentInputDevice.AddListener(textSwap, true);
        }

        protected override void onDisable()
        {
        }

        protected override void onDestory()
        {
            if (!InputManager.IsExist)
            {
                return;
            }

            InputManager.Instance.LoadedInputHandler.CurrentInputDevice.RemoveListener(textSwap);
        }

        public void textSwap(eInputDeviceType inputDevice)
        {
            string convertedString = "";
            Debug.Log(inputDevice);
            switch (inputDevice)
            {
                case eInputDeviceType.KeyboardAndMouse:
                    convertedString = sfKeyBoardText;
                    break;
                case eInputDeviceType.GamePad:
                    convertedString = sfGamePadText;
                    break;
                case eInputDeviceType.Mobile:
                    convertedString = sfMobileText;
                    break;
                default:
                    break;
            }
            TMP_Text textComponent = GetComponent<TMP_Text>();
            textComponent.text = convertedString;
        }
    }
}
