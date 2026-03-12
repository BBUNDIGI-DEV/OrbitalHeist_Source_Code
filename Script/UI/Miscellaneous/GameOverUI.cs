
namespace UnityEngine.UI
{
    public class GameOverUI : MonoBehaviour
    {
        [SerializeField] private GameObject sfGameOverUI;

        private void Awake()
        {
            sfGameOverUI.gameObject.SetActive(false);
        }

        public void RestartGameUnityEvent()
        {
            SceneSwitchingManager.Instance.LoadOtherScene(SceneSwitchingManager.Instance.CurrentScene, true, true);
            sfGameOverUI.SetActive(false);
            return;
        }

        public void ReturnToTitleUnityEvent()
        {
            SceneSwitchingManager.Instance.LoadOtherScene(eSceneName.Scene_Title, true);
            sfGameOverUI.SetActive(false);
        }

    }
}

