using TMPro;
using Febucci.UI.Core;

namespace UnityEngine.UI
{
    public class DialogueTextBox : MonoBehaviour
    {
        public bool mIsActivated
        {
            get; private set;
        }
        private const string SHOW_ANIM_KEY = "ANI_DialogueTextBoxShowUp";
        private const string CLOSE_ANIM_KEY = "ANI_DialogueTextBoxCloseDown";

        [SerializeField] private Animation sfTextAnim;
        [SerializeField] private TypewriterCore sfTextWriter;
        [SerializeField] private TMP_Text sfLeftNametext;
        [SerializeField] private TMP_Text sfRightNameText;
        [SerializeField] private Image sfMoveToNextIndicator;

        private void OnEnable()
        {
            sfTextWriter.GetComponent<TMP_Text>().text = "";
            sfLeftNametext.text = "";
            sfRightNameText.text = "";
            sfTextWriter.onTextShowed.AddListener(() => sfMoveToNextIndicator.enabled = true);
            sfTextAnim.Play(CLOSE_ANIM_KEY, 1.0f);
        }

        public void ShowTextBox()
        {
            if(mIsActivated)
            {
                return;
            }

            sfTextAnim.Play(SHOW_ANIM_KEY);
            mIsActivated = true;
        }

        public void CloseTextBox()
        {
            if (!mIsActivated)
            {
                return;
            }
            mIsActivated = false;
            sfTextAnim.Play(CLOSE_ANIM_KEY);
        }

        public void SetText(string text)
        {
            if (sfMoveToNextIndicator.enabled)
            {
                sfMoveToNextIndicator.enabled = false;
            }
            sfTextWriter.ShowText(text);
        }

        public bool TrySkipPlaying()
        {
            if (sfTextWriter.isShowingText)
            {
                sfTextWriter.SkipTypewriter();
                return true;
            }
            return false;
        }

        public void SetName(string name, eDialougeDir dir)
        {
            if(dir == eDialougeDir.Left)
            {
                sfRightNameText.transform.parent.gameObject.SetActive(false);// include parent iamge

                sfLeftNametext.transform.parent.gameObject.SetActive(true);
                sfLeftNametext.text = name;
            }
            else 
            {
                sfLeftNametext.transform.parent.gameObject.SetActive(false);

                sfRightNameText.transform.parent.gameObject.SetActive(true);
                sfRightNameText.text = name;
            }
        }
    }
}