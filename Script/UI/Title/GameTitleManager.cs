
namespace UnityEngine.UI
{
    public class GameTitleManager : MonoBehaviour
    {
        private bool mIsLoading;

        private void Awake()
        {
            mIsLoading = false;
        }

        public void Start()
        {
            InputManager.Instance.SwitchInputSection(eInputSections.Menu);
            GlobalTimer.Instance.ClearAllTimer();
        }

        public void StartGame()
        {
            if (mIsLoading)
            {
                return;
            }

            mIsLoading = true;
            //SceneSwitchingManager.Instance.LoadOtherScene(eSceneName.Scene_StoryCutscene, true);
        }

        public void QuitGame()
        {
            if (mIsLoading)
            {
                return;
            }

            Application.Quit();
        }
    }
}

