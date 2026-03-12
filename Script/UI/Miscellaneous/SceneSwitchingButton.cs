namespace UnityEngine.UI
{
    [RequireComponent(typeof(Button))]
    public class SceneSwitchingButton : MonoBehaviour
    {
        [SerializeField] private eSceneName sfSceneName;

        private void Awake()
        {
            GetComponent<Button>().onClick.AddListener(OnButtonClicked);
        }

        public void OnButtonClicked()
        {
            GetComponent<Button>().interactable = false;
            SceneSwitchingManager.Instance.LoadOtherScene(sfSceneName, true);
        }
    }
}