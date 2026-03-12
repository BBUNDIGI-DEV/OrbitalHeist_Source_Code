
namespace UnityEngine.UI
{
    public class DialogueCharacter : MonoBehaviour
    {
        public bool IsActivated
        {
            get; private set;
        }

        public bool IsEnabled
        {
            get; private set;
        }

        public eDialogueCharacterType CurrentCharacter
        {
            get; private set;
        }

        [SerializeField] private DialogueSpriteDataConfig[] sfSpriteData;
        [SerializeField] private Animator sfChracterAnim;
        [SerializeField] private UIImageSwitcher sfFrontImage;
        [SerializeField] private UIImageSwitcher sfBackImage;
        [SerializeField] private SimpleRecttransformSwitcher sfEmojiTransform;


        private void OnEnable()
        {
            sfChracterAnim.Play(eDefaultActing.Close.ToString(), 0, 1);
            CurrentCharacter = eDialogueCharacterType.None;
            IsActivated = false;
        }

        public void ShowCharacter(eDialogueCharacterType charName)
        {
            if (IsActivated)
            {
                return;
            }

            int imageIndex = (int)charName;
            sfFrontImage.SwitchSprite(imageIndex);
            sfEmojiTransform.SwitchPos(imageIndex);

            sfChracterAnim.Play(eDefaultActing.Show.ToString());
            IsActivated = true;
            CurrentCharacter = charName;
        }

        public void CloseCharacter()
        {
            if(!IsActivated)
            {
                return;
            }
            IsActivated = false;
            sfChracterAnim.Play(eDefaultActing.Close.ToString());
            CurrentCharacter = eDialogueCharacterType.None;
            for (int i = 0; i < sfEmojiTransform.transform.childCount; i++)
            {
                sfEmojiTransform.transform.GetChild(i).gameObject.SetActive(false);
            }
        }

        public void SwapCharacter(eDialogueCharacterType charName)
        {
            if(CurrentCharacter == charName)
            {
                return;
            }

            if (CurrentCharacter == eDialogueCharacterType.None)
            {
                ShowCharacter(charName);
                return;
            }

            if (charName == eDialogueCharacterType.None)
            {
                CloseCharacter();
                return;
            }

            int currentImageIndex = (int)CurrentCharacter;
            sfBackImage.SwitchSprite(currentImageIndex);

            int imageIndex = (int)charName;
            sfFrontImage.SwitchSprite(imageIndex);
            sfEmojiTransform.SwitchPos(imageIndex);
            sfChracterAnim.Play(eDefaultActing.Swap.ToString());
            CurrentCharacter = charName;
        }

        public void EnableCharacter()
        {
            sfChracterAnim.Play(eDefaultActing.Enable.ToString(), (int)eDialogueCharacterAnimLayer.EnabledOrDisabled);
        }

        public void DisableCharacter()
        {
            sfChracterAnim.Play(eDefaultActing.Disable.ToString(), (int)eDialogueCharacterAnimLayer.EnabledOrDisabled);
        }

        public void DisableCharacterImediate()
        {
            sfChracterAnim.Play(eDefaultActing.Disable.ToString(), 0, 1.0f);
        }

        public void SwitchEmotion(eEmotionType emotion)
        {
            if(sfSpriteData.Length <= (int)emotion)
            {
                return;
            }
            if(CurrentCharacter == eDialogueCharacterType.None)
            {
                return;
            }

            Sprite charSprite = sfSpriteData[(int)CurrentCharacter].GetCharacterSprite(emotion);
            sfFrontImage.GetComponent<Image>().sprite = charSprite;
        }

        public void PlayEmoji(eEmojiType emoji)
        {
            if (emoji == eEmojiType.None)
            {
                return;
            }
            Debug.Log(emoji);
            sfEmojiTransform.transform.GetChild((int)emoji - 1).gameObject.SetActive(true);
            sfChracterAnim.Play(emoji.ToString(), (int)eDialogueCharacterAnimLayer.Emoji);
        }

        public void PlayGesture(eGestureType gesture)
        {
            if(gesture == eGestureType.None)
            {
                return;
            }
            sfChracterAnim.Play(gesture.ToString(), (int)eDialogueCharacterAnimLayer.Gesture);
        }
    }

}
