using UnityEngine.EventSystems;

namespace UnityEngine.UI
{
    [RequireComponent(typeof(Animator)), RequireComponent(typeof(Button))]
    public class UICustomButtonAnimator : MonoBehaviour, IPointerEnterHandler, IPointerClickHandler, IPointerExitHandler
    {
        private Button mButton;
        private Animator mAnim;
        private bool mIsInteractable;
        private void Awake()
        {
            mButton = GetComponent<Button>();
            mAnim = GetComponent<Animator>();
        }

        private void Update()
        {
            if (mIsInteractable == false && mButton.IsInteractable() == true)
            {
                mAnim.SetTrigger("Enabled");
                mIsInteractable = true;
            }
            else if (mIsInteractable == true && mButton.IsInteractable() == false)
            {
                mAnim.SetTrigger("Disabled");
                mIsInteractable = false;
            }
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            mAnim.SetTrigger("Pressed");
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            mAnim.SetTrigger("Highlighted");
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            mAnim.SetTrigger("Normal");
        }
    }
}