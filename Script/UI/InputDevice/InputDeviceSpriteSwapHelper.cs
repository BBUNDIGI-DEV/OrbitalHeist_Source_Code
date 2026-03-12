namespace UnityEngine.UI
{
    public class InputDeviceSpriteSwapHelper : DestoryOnlyIfAwakeMonoBehavior
    {
        [SerializeField] private Sprite sfKeyboardSprite;
        [SerializeField] private Sprite sfGamePadSprite;
        [SerializeField] private Sprite sfMobileSprite;

        private void Start()
        {
            InputManager.Instance.LoadedInputHandler.CurrentInputDevice.AddListener(spriteSwap, true);
        }

        protected override void onDisable()
        {
        }

        protected override void onDestory()
        {
            if (InputManager.IsExist)
            {
                InputManager.Instance.LoadedInputHandler.CurrentInputDevice.RemoveListener(spriteSwap);
            }
        }

        public void spriteSwap(eInputDeviceType isGamePad)
        {
            Sprite convertedSprite = null;
            switch (isGamePad)
            {
                case eInputDeviceType.KeyboardAndMouse:
                    convertedSprite = sfKeyboardSprite;
                    break;
                case eInputDeviceType.GamePad:
                    convertedSprite = sfGamePadSprite;
                    break;
                case eInputDeviceType.Mobile:
                    convertedSprite = sfMobileSprite;
                    break;
                default:
                    break;
            }

            if (convertedSprite == null)
            {
                gameObject.SetActive(false);
                return;
            }

            Image image = GetComponent<Image>();

            if (image != null)
            {
                image.sprite = convertedSprite;
            }

            SpriteRenderer sr = GetComponent<SpriteRenderer>();
            if (sr != null)
            {
                sr.sprite = convertedSprite;
            }
        }
    }
}