using static UnityEngine.InputSystem.InputAction;


namespace UnityEngine.UI
{
    public class SettingUI : MonoBehaviour
    {
        const string SHOW_SETTING_UI_ANIM = "ANI_ShowSetting";
        const string HIDE_SETTING_UI_ANIM = "ANI_HideSetting";

        [SerializeField] public GameObject[] sfSettingPanels;
        [SerializeField] public GameObject sfSettingUI;
        [SerializeField] public Animation sfSettingUIAnim;
        [SerializeField] public Button sfSettingButton;


        private int mCurrentSettingIndex = -1;

        private void Awake()
        {
            SwapSettingIndex(0);
        }

        private void Start()
        {
            InputManager.Instance.AddInputCallback(eInputSections.BattleGamePlay, eBattleInputName.PauseGame.ToString(), OnSettingInputPressed);
        }

        private void OnDestroy()
        {
            if (InputManager.IsExist)
            {
                InputManager.Instance.RemoveInputCallback(eInputSections.BattleGamePlay, eBattleInputName.PauseGame.ToString(), OnSettingInputPressed);
            }
        }

        public void OnSettingInputPressed(CallbackContext context)
        {
            if (sfSettingUI.activeInHierarchy)
            {
                ToggleSetting();
            }

            if (!PlayerManager.Instance.IsInputEnabled)
            {
                return;
            }

            ToggleSetting();
        }

        public void ToggleSetting()
        {
            if (sfSettingUIAnim.isPlaying)
            {
                return;
            }

            if (sfSettingUI.activeInHierarchy)
            {
                sfSettingButton.interactable = false;
                sfSettingUIAnim.GetComponent<LegacyAnimationUnscaletimer>().Play(HIDE_SETTING_UI_ANIM);
                TimeScaleUtil.Instance.RemoveTimeScale("Setting");
                PlayerManager.Instance.IsInputEnabled = true;
            }
            else
            {
                sfSettingButton.interactable = true;
                sfSettingUI.SetActive(true);
                sfSettingUIAnim.GetComponent<LegacyAnimationUnscaletimer>().Play(SHOW_SETTING_UI_ANIM);
                TimeScaleUtil.Instance.AddTimeScale("Setting", new PriorityAndTimeScalePair(eTimeScaleTrigger.PauseGame, 0.0f));
                PlayerManager.Instance.IsInputEnabled = false;
            }
        }


        public void SwapSettingIndex(int newIndex)
        {
            if (mCurrentSettingIndex != -1)
            {
                sfSettingPanels[mCurrentSettingIndex].SetActive(false);
            }

            mCurrentSettingIndex = newIndex;
            sfSettingPanels[mCurrentSettingIndex].SetActive(true);
        }
    }
}