using UnityEngine.InputSystem;
using DG.Tweening;

namespace UnityEngine.UI
{
    public class StoryCutSceneManager : MonoBehaviour
    {
        [SerializeField] private float sfCutSceneOffset;
        [SerializeField] private RectTransform sfCutSceneTrans;
        [SerializeField] private eSceneName sfLoadingScene;
        private StoryCutSceneElement[] mCutScenes;
        private int mCutSceneIndex;
        private const float TEXT_COOL_DOWN = 0.5f;
        private float mTouchCoolDown = 0.0f;

        private void Awake()
        {
            mCutScenes = GetComponentsInChildren<StoryCutSceneElement>();
        }

        private void Start()
        {
            for (int i = 0; i < mCutScenes.Length; i++)
            {
                if (i == 0)
                {
                    mCutScenes[i].EnableCutScene();
                }
                else
                {
                    mCutScenes[i].DisableCutScene();
                }
            }


            InputManager.Instance.AddInputCallback(eInputSections.CutScene, eCutSceneInputName.CutSceneInteraction.ToString(), OnClick);
            InputManager.Instance.AddInputCallback(eInputSections.CutScene, eCutSceneInputName.Skip.ToString(), SkipcutScene);

            InputManager.Instance.SwitchInputSection(eInputSections.CutScene);
            InputManager.Instance.IsInputEnabled = true;
        }

        private void Update()
        {
            if (mTouchCoolDown < 0.0f)
            {
                return;
            }

            mTouchCoolDown -= Time.deltaTime;
        }

        private void OnDestroy()
        {
            if (!InputManager.IsExist)
            {
                return;
            }
            InputManager.Instance.RemoveInputCallback(eInputSections.CutScene, eCutSceneInputName.CutSceneInteraction.ToString(), OnClick);
            InputManager.Instance.RemoveInputCallback(eInputSections.CutScene, eCutSceneInputName.Skip.ToString(), SkipcutScene);

        }

        public void OnClick(InputAction.CallbackContext context)
        {
            StoryCutSceneElement currentCut = mCutScenes[mCutSceneIndex];

            if (mTouchCoolDown >= 0.0f)
            {
                return;
            }

            if (currentCut.IsTextBoxSkipable)
            {
                currentCut.SkipTextBox();
                mTouchCoolDown = 0.0f;
                return;
            }


            bool isExit = currentCut.InvokeClicked(out eCutSceneActType actType);

            if (isExit)
            {
                mTouchCoolDown = 0.5f;
                if (mCutSceneIndex == mCutScenes.Length - 1)
                {
                    SceneSwitchingManager.Instance.LoadOtherScene(sfLoadingScene, true);
                }
                else
                {
                    moveToNextCutScene();
                }
                return;
            }

            mTouchCoolDown = 0.5f;
        }

        public void SkipcutScene(InputAction.CallbackContext context)
        {
            SkipcutScene();
        }

        public void SkipcutScene()
        {
            SceneSwitchingManager.Instance.LoadOtherScene(sfLoadingScene, true);
        }

        private void moveToNextCutScene()
        {
            sfCutSceneTrans.DOAnchorPosX(sfCutSceneTrans.anchoredPosition.x - sfCutSceneOffset, 0.5f);

            mCutScenes[mCutSceneIndex].DisableCutScene();
            mCutScenes[mCutSceneIndex + 1].EnableCutScene();
            mCutSceneIndex++;
        }
    }
}