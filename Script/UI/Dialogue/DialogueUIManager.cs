using Sirenix.OdinInspector;
using System.Collections;

namespace UnityEngine.UI
{
    public class DialogueUIManager : MonoBehaviour
    {
        private const string SHOW_UP_KEY = "ANI_DialogueShowUp";
        private const string CLOSE_KEY = "ANI_DialogueCloseDown";
        [SerializeField] private Animation sfDialogueAnim;
        [SerializeField] private DialogueCharacter sfLeftDialogue;
        [SerializeField] private DialogueCharacter sfRightDialogue;
        [SerializeField] private DialogueTextBox sfTextBox;
        [SerializeField] private Button sfSkipButton;

        private DialogueCharacter mActivatedCharacter
        {
            get
            {
                switch (mDir)
                {
                    case eDialougeDir.Left:
                        return sfLeftDialogue;
                    case eDialougeDir.Right:
                        return sfRightDialogue;
                }
                return null;
            }
        }

        private DialogueCharacter mDeactivatedCharacter
        {
            get
            {
                switch (mDir)
                {
                    case eDialougeDir.Left:
                        return sfRightDialogue;
                    case eDialougeDir.Right:
                        return sfLeftDialogue;
                }
                return null;
            }
        }
        private eDialogueTag mCurrentTag;
        private eDialougeDir mDir;
        private bool mIsPlayerClick;
        private System.Action mOnDialogueEnd;
        private Coroutine mDialgoueRoutine;
        private GameObject mMainGameobject
        {
            get
            {
                return transform.GetChild(0).gameObject;
            }
        }


        private void Awake()
        {
            mMainGameobject.SetActive(false);
        }

        public void Start()
        {
            InputManager.Instance.AddInputCallback(eInputSections.Dialouge, eDialougeInputName.DialogueInteraction.ToString(), clickDialogue);
            InputManager.Instance.AddInputCallback(eInputSections.Dialouge, eDialougeInputName.Skip.ToString(), skipDialogue);
        }
        private void OnDestroy()
        {
            if (InputManager.IsExist)
            {
                InputManager.Instance.RemoveInputCallback(eInputSections.Dialouge, eDialougeInputName.DialogueInteraction.ToString(), clickDialogue);
                InputManager.Instance.RemoveInputCallback(eInputSections.Dialouge, eDialougeInputName.Skip.ToString(), skipDialogue);
            }
        }

        [Button]
        public void PlayDialouge(eDialogueTag tag)
        {
            PlayDialouge(tag, null);
        }

        public void PlayDialouge(eDialogueTag tag, System.Action onDialogueEnd)
        {
            mCurrentTag = tag;
            mOnDialogueEnd = onDialogueEnd;
            InputManager.Instance.SwitchInputSection(eInputSections.Dialouge);
            sfSkipButton.gameObject.SetActive(true);
            sfSkipButton.interactable = true;
            mDir = eDialougeDir.None;
            if(mDialgoueRoutine != null)
            {
                StopCoroutine(mDialgoueRoutine);
            }
            mDialgoueRoutine = StartCoroutine(PlayRoutine(RuntimeDataLoader.DialogueConfigDic[tag]));
        }

        private IEnumerator PlayRoutine(DialogueConfig[] configs)
        {
            mMainGameobject.SetActive(true);
            sfDialogueAnim.Play(SHOW_UP_KEY);
            sfTextBox.ShowTextBox();
            for (int i = 0; i < configs.Length; i++)
            {
                DialogueConfig data = configs[i];
                eDialogueCharacterType activateCharacterType = eDialogueCharacterType.None;
                switch (data.Dir)
                {
                    case eDialougeDir.Left:
                        activateCharacterType = configs[i].LeftCharacter;
                        break;
                    case eDialougeDir.Right:
                        activateCharacterType = configs[i].RightCharacter;
                        break;
                    default:
                        break;
                }

                if (data.LeftCharacter != sfLeftDialogue.CurrentCharacter)
                {
                    sfLeftDialogue.SwapCharacter(data.LeftCharacter);
                }

                if (data.RightCharacter != sfRightDialogue.CurrentCharacter)
                {
                    sfRightDialogue.SwapCharacter(data.RightCharacter);
                }

                if (mDir != data.Dir)
                {
                    mDir = data.Dir;
                    if(mActivatedCharacter != null)
                    {
                        mDeactivatedCharacter.DisableCharacter();
                        mActivatedCharacter.EnableCharacter();
                    }
                    mActivatedCharacter.transform.SetAsLastSibling();
                }

                if (data.Name != null && data.Name != "")
                {
                    sfTextBox.SetName(data.Name, mDir);
                }
                else
                {
                    sfTextBox.SetName(convertDialogueTypeToName(activateCharacterType), mDir);
                }

                if (data.Contents != null && data.Contents != "")
                {
                    sfTextBox.SetText(data.Contents);
                }

                mActivatedCharacter.PlayGesture(data.GestureType);
                mActivatedCharacter.PlayEmoji(data.EmojiType);
                mActivatedCharacter.SwitchEmotion(data.EmotionType);

                yield return new WaitUntil(() => mIsPlayerClick);
                mIsPlayerClick = false;
                if (sfTextBox.TrySkipPlaying())
                {
                    yield return new WaitForSeconds(0.25f);
                    yield return new WaitUntil(() => mIsPlayerClick);
                    mIsPlayerClick = false;
                }
            }
            closeDialouge();
        }

        public void SkipDialogue()
        {
            StopCoroutine(mDialgoueRoutine);
            closeDialouge();
        }

        public void DeactivatedDialogueAnimEvent()
        {
            mMainGameobject.SetActive(false);
        }

        private void closeDialouge()
        {
            sfLeftDialogue.CloseCharacter();
            sfRightDialogue.CloseCharacter();
            sfTextBox.CloseTextBox();

            sfDialogueAnim.Play(CLOSE_KEY);
            sfSkipButton.gameObject.SetActive(false);
            InputManager.Instance.SwitchInputSection(eInputSections.BattleGamePlay);
            mOnDialogueEnd?.Invoke();
            switch (mCurrentTag)
            {
                case eDialogueTag.Stage1_01_ShivEntrance:
                    PlayerManager.Instance.AddActivatedCharacter(eCharacterName.Shiv);
                    break;
                case eDialogueTag.Stage2_01_HypoEntrance:
                    PlayerManager.Instance.AddActivatedCharacter(eCharacterName.Hypo);
                    break;
            }
        }


        private void skipDialogue(InputSystem.InputAction.CallbackContext context)
        {
            SkipDialogue();
        }

        private void clickDialogue(InputSystem.InputAction.CallbackContext context)
        {
            mIsPlayerClick = true;
        }

        private string convertDialogueTypeToName(eDialogueCharacterType characterType)
        {
            switch (characterType)
            {
                case eDialogueCharacterType.Glanda:
                    return "트릭시";
                case eDialogueCharacterType.Shiv:
                    return "시브";
                case eDialogueCharacterType.Hypo:
                    return "하이포";
                case eDialogueCharacterType.Blin:
                    return "블린";
                case eDialogueCharacterType.Moab:
                    return "모압";
                case eDialogueCharacterType.MoabSilhouette:
                    return "???";
            }
            return "";
        }
    }
}