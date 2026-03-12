using DG.Tweening;
using Sirenix.OdinInspector;
using System.Collections.Generic;

namespace UnityEngine.UI
{
    public class StoryCutSceneElement : MonoBehaviour
    {
        public bool IsTextBoxSkipable
        {
            get
            {
                if (sfCutSceneActionData.Length == 0)
                {
                    return false;
                }

                if (sfCutSceneActionData.Length == mActIndex)
                {
                    return false;
                }

                if (mActIndex == -1)
                {
                    return false;
                }

                if (!sfCutSceneActionData[mActIndex].ActType.HasFlag(eCutSceneActType.ShowNextTextBox))
                {
                    return false;
                }

                return sfCutSceneActionData[mActIndex].TextBox.IsSkipable;
            }
        }

        private const string ENABLE_KEY = "ANI_StoryCutSceneEnabled";
        private const string DISABLE_KEY = "ANI_StoryCutSceneDisabled";
        [SerializeField] private RectTransform sfMainImageTrans;
        [SerializeField] private StoryCutSceneActionData[] sfCutSceneActionData;
        private List<CutSceneTextBoxElement> mActivatedTextBox;
        private List<CanvasGroup> mActivatedMindBlock;
        private int mActIndex;


        private void Awake()
        {
            mActIndex = -1;
            mActivatedTextBox = new List<CutSceneTextBoxElement>();
            mActivatedMindBlock = new List<CanvasGroup>();
        }

        public void EnableCutScene()
        {
            GetComponent<Animation>().Play(ENABLE_KEY);
        }

        public void DisableCutScene()
        {
            GetComponent<Animation>().Play(DISABLE_KEY);
        }

        public bool InvokeClicked(out eCutSceneActType actType)
        {
            actType = eCutSceneActType.None;
            mActIndex++;
            if (mActIndex == sfCutSceneActionData.Length)
            {
                return true;
            }

            actType = sfCutSceneActionData[mActIndex].ActType;

            if (actType.HasFlag(eCutSceneActType.ShowNextTextBox))
            {
                InvokeTextBox();
            }

            if (actType.HasFlag(eCutSceneActType.ShowSubCutScene))
            {
                InvokeSubCutScene();
            }

            if (actType.HasFlag(eCutSceneActType.ShowOverlayScene))
            {
                InvokeOverlayScene();
            }

            if (actType.HasFlag(eCutSceneActType.ShowMindBlock))
            {
                InvokeMindBlock();
            }


            if (actType.HasFlag(eCutSceneActType.HideAllTextBox))
            {
                InvokeHideAllTextBox();
            }

            if (actType.HasFlag(eCutSceneActType.HideAllMindBlock))
            {
                InvokeHideAllMindBlock();
            }



            return false;
        }

        public void InvokeSubCutScene()
        {
            StoryCutSceneActionData data = sfCutSceneActionData[mActIndex];

            if (data.SubCutScene.GetComponent<Animation>() != null)
            {
                data.SubCutScene.GetComponent<Animation>().Play();
            }
            else
            {
                sfMainImageTrans.DOAnchorPos(data.MainCutSceneDestPos, 0.495f);
                data.SubCutScene.DOAnchorPos(data.SubCutSceneDestPos, 0.495f);
            }
        }

        public void InvokeTextBox()
        {
            CutSceneTextBoxElement textBox = sfCutSceneActionData[mActIndex].TextBox;
            textBox.ShowUp();
            mActivatedTextBox.Add(textBox);
        }

        public void InvokeOverlayScene()
        {
            Image ovelayImage = sfCutSceneActionData[mActIndex].OverlayCutScene;
            ovelayImage.DOFade(1.0f, 0.2f);
        }

        public void InvokeMindBlock()
        {
            CanvasGroup mindBlock = sfCutSceneActionData[mActIndex].OverlayMindBlock;
            mindBlock.DOFade(1.0f, 0.6f);
            mActivatedMindBlock.Add(mindBlock);
        }

        public void InvokeHideAllTextBox()
        {
            for (int i = 0; i < mActivatedTextBox.Count; i++)
            {
                mActivatedTextBox[i].HideTextBox();
            }
            mActivatedTextBox.Clear();
        }

        public void InvokeHideAllMindBlock()
        {
            for (int i = 0; i < mActivatedMindBlock.Count; i++)
            {
                mActivatedMindBlock[i].DOFade(0.0f, 0.6f);
            }
            mActivatedMindBlock.Clear();
        }


        public void SkipTextBox()
        {
            CutSceneTextBoxElement textBox = sfCutSceneActionData[mActIndex].TextBox;
            textBox.SkipTextBox();
        }
    }

    [System.Serializable]
    public struct StoryCutSceneActionData
    {
        [Space(10)]
        public eCutSceneActType ActType;
        [TitleGroup("ShowSubCutScene Data")][ShowIf("@ActType.HasFlag(eCutSceneActType.ShowSubCutScene)")] public Vector3 MainCutSceneDestPos;
        [TitleGroup("ShowSubCutScene Data")][ShowIf("@ActType.HasFlag(eCutSceneActType.ShowSubCutScene)")] public RectTransform SubCutScene;
        [TitleGroup("ShowSubCutScene Data")][ShowIf("@ActType.HasFlag(eCutSceneActType.ShowSubCutScene)")] public Vector3 SubCutSceneDestPos;
        [TitleGroup("ShowNextTextBox Data")][ShowIf("@ActType.HasFlag(eCutSceneActType.ShowNextTextBox)")] public CutSceneTextBoxElement TextBox;
        [TitleGroup("ShowOverlayScene Data")][ShowIf("@ActType.HasFlag(eCutSceneActType.ShowOverlayScene)")] public Image OverlayCutScene;
        [TitleGroup("ShowOverlayScene Data")][ShowIf("@ActType.HasFlag(eCutSceneActType.ShowMindBlock)")] public CanvasGroup OverlayMindBlock;
    }

    [System.Flags]
    public enum eCutSceneActType
    {
        None = -1,
        ShowSubCutScene = 1 << 0,
        ShowNextTextBox = 1 << 1,
        ShowOverlayScene = 1 << 2,
        ShowMindBlock = 1 << 3,
        HideAllTextBox = 1 << 4,
        HideAllMindBlock = 1 << 5,

    }
}