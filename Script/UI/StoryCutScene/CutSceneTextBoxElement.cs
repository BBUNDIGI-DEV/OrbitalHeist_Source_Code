using Febucci.UI.Core;

namespace UnityEngine.UI
{
    public class CutSceneTextBoxElement : MonoBehaviour
    {
        public bool IsSkipable
        {
            get
            {
                if (!mIsIntialized)
                {
                    return false;
                }
                return ANIM.isPlaying || TYPE_WRITER.isShowingText;
            }
        }

        private bool mIsOnSkipping;
        private const string SHOW_UP_ANIM_KEY = "ANI_StoryCutSceneTextBoxShowUP";
        private const string HIDE_DOWN_ANIM_KEY = "ANI_StoryCutSceneTextBoxHideDown";
        private Animation ANIM;
        private TypewriterCore TYPE_WRITER;

        private bool mIsIntialized = false;

        private void Awake()
        {
            if (!mIsIntialized)
            {
                gameObject.SetActive(false);
            }
        }

        private void Initialize()
        {
            ANIM = GetComponentInChildren<Animation>(true);
            TYPE_WRITER = GetComponentInChildren<TypewriterCore>(true);
            mIsIntialized = true;
        }

        public void SkipTextBox()
        {
            mIsOnSkipping = true;
            if (ANIM.isPlaying)
            {
                ANIM[ANIM.clip.name].time = ANIM[ANIM.clip.name].length;
            }

            if (TYPE_WRITER.isShowingText)
            {
                TYPE_WRITER.SkipTypewriter();
            }
        }

        public void ShowUp()
        {
            if (ANIM == null)
            {
                Initialize();
            }
            ANIM.Play(SHOW_UP_ANIM_KEY);
            gameObject.SetActive(true);
        }

        public void HideTextBox()
        {
            ANIM.Play(HIDE_DOWN_ANIM_KEY);
        }

        public void PlayTextAnimator_AnimEvent()
        {
            if (mIsOnSkipping)
            {
                mIsOnSkipping = false;
                return;
            }
            TYPE_WRITER.StartShowingText(true);
        }

        public void CloseAnimEnd_AnimEvent()
        {
            gameObject.SetActive(false);
        }
    }
}