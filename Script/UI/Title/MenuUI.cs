namespace UnityEngine.UI
{
    public class MenuUI : MonoBehaviour
    {
        [SerializeField] private GameObject sfMenuUI;
        private bool mIsPaused;

        private void Awake()
        {
            sfMenuUI.gameObject.SetActive(false);
        }

        private void Start()
        {
            InputManager.Instance.AddInputCallback(eInputSections.BattleGamePlay, eBattleInputName.PauseGame.ToString(), setGamePausedInputEvent);
        }

        private void OnEnable()
        {
            InputManager.Instance.AddInputCallback(eInputSections.UI, eUIInputName.PauseGame.ToString(), setResumeInputEvent);
        }

        private void OnDisable()
        {
            if (InputManager.IsExist)
            {
                InputManager.Instance.RemoveInputCallback(eInputSections.UI, eBattleInputName.PauseGame.ToString(), setResumeInputEvent);
            }
        }

        private void OnDestroy()
        {
            if (InputManager.IsExist)
            {
                InputManager.Instance.RemoveInputCallback(eInputSections.BattleGamePlay, eUIInputName.PauseGame.ToString(), setGamePausedInputEvent);
            }
        }

        public void ResumeGameUnityEvent()
        {
            sfMenuUI.SetActive(false);
            mIsPaused = false;
            Time.timeScale = 1.0f;
            InputManager.Instance.SwitchInputSection(eInputSections.BattleGamePlay);
        }

        public void BackToTitleUnityEvent()
        {
            ResumeGameUnityEvent();
            SceneSwitchingManager.Instance.LoadOtherScene(eSceneName.Scene_Title, true);
        }

        private void setGamePaused()
        {
            sfMenuUI.SetActive(true);
            mIsPaused = true;
            Time.timeScale = 0.0f;
            InputManager.Instance.SwitchInputSection(eInputSections.UI);
        }

        private void setGamePausedInputEvent(UnityEngine.InputSystem.InputAction.CallbackContext context)
        {

            setGamePaused();
        }

        private void setResumeInputEvent(UnityEngine.InputSystem.InputAction.CallbackContext context)
        {
            if (mIsPaused)
            {
                ResumeGameUnityEvent();
            }
        }
    }
}
