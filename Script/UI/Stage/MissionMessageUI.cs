using TMPro;

namespace UnityEngine.UI
{
    public class MissionMessageUI : MonoBehaviour
    {
        private const string SHOW_UP_KEY = "ANI_MissionMessageShowUp";
        private const string CLOSE_DOWN_KEY = "ANI_MissionMessageCloseDown";

        private bool isShowUP;
        [SerializeField] private Animation sfAnim;
        [SerializeField] private TMP_Text sfText;
        [SerializeField] private UIImageSwitcher sfImageSwitcher;


        public void MissionMessageShowUP(eCharacterName speaker)
        {
            if (isShowUP)
            {
                return;
            }

            isShowUP = true;
            if (sfAnim.IsPlaying(CLOSE_DOWN_KEY))
            {
                sfAnim.Blend(SHOW_UP_KEY);
            }
            else
            {
                sfAnim.Play(SHOW_UP_KEY);
            }
            sfImageSwitcher.SwitchSprite(speaker);
        }

        public void MissionMessageCloseDown()
        {
            if (!isShowUP)
            {
                return;
            }

            isShowUP = false;
            if (sfAnim.IsPlaying(SHOW_UP_KEY))
            {
                sfAnim.Blend(CLOSE_DOWN_KEY);
            }
            else
            {
                sfAnim.Play(CLOSE_DOWN_KEY);
            }

        }

        public void SetText(string text)
        {
            sfText.text = text;
        }
    }
}

