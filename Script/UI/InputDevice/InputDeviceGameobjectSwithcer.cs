namespace UnityEngine.UI
{
    public class InputDeviceGameobjectSwithcer : DestoryOnlyIfAwakeMonoBehavior
    {
        [SerializeField] private GameObject sfKeyboardGameobject;
        [SerializeField] private GameObject sfGamePadGameobject;
        [SerializeField] private GameObject sfMobileGameobject;

        private void Start()
        {
            InputManager.Instance.LoadedInputHandler.CurrentInputDevice.AddListener(gameobjectSwap, true);
        }

        protected override void onDisable()
        {
        }

        protected override void onDestory()
        {
            if (InputManager.IsExist)
            {
                InputManager.Instance.LoadedInputHandler.CurrentInputDevice.RemoveListener(gameobjectSwap);
            }
        }

        private void gameobjectSwap(eInputDeviceType isGamePad)
        {
            sfKeyboardGameobject.SetActive(false);
            sfGamePadGameobject.SetActive(false);

            switch (isGamePad)
            {
                case eInputDeviceType.KeyboardAndMouse:
                    sfKeyboardGameobject.SetActive(true);
                    break;
                case eInputDeviceType.GamePad:
                    sfGamePadGameobject.SetActive(true);
                    break;
                case eInputDeviceType.Mobile:
                    sfMobileGameobject.SetActive(true);
                    break;
                default:
                    break;
            }
        }


    }
}