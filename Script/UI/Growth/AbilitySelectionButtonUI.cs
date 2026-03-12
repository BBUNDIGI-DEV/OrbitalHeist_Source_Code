namespace UnityEngine.UI
{
    [RequireComponent(typeof(Button)), RequireComponent(typeof(Animator))]
    public class AbilitySelectionButtonUI : MonoBehaviour
    {
        [SerializeField] private TMPro.TMP_Text sfNameText;
        [SerializeField] private TMPro.TMP_Text sfDescriptionText;
        [SerializeField] private UIImageSwitcher sfBackgroundImage;
        [SerializeField] private UIImageSwitcher sfIconBG;
        [SerializeField] private Image sfIconImage;

        private void Awake()
        {
            GetComponent<Button>().onClick.AddListener(onSelected);
        }

        public void SetSelectionButton(GrowthAbilityData data)
        {
            gameObject.SetActive(true);
            sfNameText.text = data.NameForUI;
            sfDescriptionText.text = data.DescriptionForUI;
            GetComponent<Animator>().SetTrigger("Enabled");
            sfBackgroundImage.SwitchSprite(data.AbilGradeEnum);
            sfIconBG.SwitchSprite(data.AbilGradeEnum);
            sfIconImage.sprite = data.IconSprite;
            SetRaycastable(false);
            SetInteractable(true);
        }

        public void SetInteractable(bool toggle)
        {
            GetComponent<CanvasGroup>().interactable = toggle;
        }

        public void SetRaycastable(bool toggle)
        {
            GetComponent<CanvasGroup>().blocksRaycasts = toggle;
        }

        private void onSelected()
        {
            SetRaycastable(false);
            SetInteractable(false);
            GetComponent<Animator>().SetTrigger("Selected");
        }
    }
}
