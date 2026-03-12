namespace UnityEngine.UI
{
    public class GrowthUI : MonoBehaviour
    {
        public GrowthAbilityData SelectedDataOrNull
        {
            get; private set;
        }

        private const string M_GROWTH_UI_SHOW_UP_ANIM = "ANI_GrowthShowUp";
        private const string M_GROWTH_UI_CLOSE_DOWN_ANIM = "ANI_GrowthCloseDown";


        [SerializeField] private GameObject sfGrowthUIParent;
        private AbilitySelectionButtonUI[] mAbilSelectionButtons;
        private GrowthAbilityData[] mCurrentSelectableDatas;

        private void Awake()
        {
            mAbilSelectionButtons = GetComponentsInChildren<AbilitySelectionButtonUI>(true);

            for (int i = 0; i < mAbilSelectionButtons.Length; i++)
            {
                int index = i;
                mAbilSelectionButtons[i].GetComponent<Button>().onClick.AddListener(() => SetSelectedAbilData(index));
            }
        }

        public void ShowGrowthUI(GrowthAbilityData[] selectableAbilList)
        {
            sfGrowthUIParent.SetActive(true);
            SelectedDataOrNull = null;
            Debug.Assert(mAbilSelectionButtons.Length >= selectableAbilList.Length,
                $"You Can't set AbilityData Length[{selectableAbilList.Length}] more than Buttons Length [{mAbilSelectionButtons.Length}]");
            for (int i = 0; i < mAbilSelectionButtons.Length; i++)
            {
                if (i >= selectableAbilList.Length)
                {
                    mAbilSelectionButtons[i].gameObject.SetActive(false);
                }
                else
                {
                    mAbilSelectionButtons[i].SetSelectionButton(selectableAbilList[i]);
                }
            }

            GetComponentInChildren<HoriLayoutoutGroupSpaceSetterByChildCount>().UpdateSpacing();
            mCurrentSelectableDatas = selectableAbilList;
            GetComponent<Animation>().Play(M_GROWTH_UI_SHOW_UP_ANIM);
        }

        public void SetButtonRaycastable_UE()
        {
            for (int i = 0; i < mCurrentSelectableDatas.Length; i++)
            {
                mAbilSelectionButtons[i].SetRaycastable(true);
            }
        }

        public void HideGrowthUI()
        {
            GetComponent<Animation>().Play(M_GROWTH_UI_CLOSE_DOWN_ANIM);
        }

        public void SetSelectedAbilData(int index)
        {
            Debug.Assert(mCurrentSelectableDatas != null,
                "You Cannot call [SetSelectedAbilData] method with [mCurrentSelectableDatas] null");

            Debug.Assert(index >= 0 && index < mCurrentSelectableDatas.Length);

            for (int i = 0; i < mAbilSelectionButtons.Length; i++)
            {
                if (i == index)
                {
                    continue;
                }
                else
                {
                    mAbilSelectionButtons[i].SetInteractable(false);
                }
            }

            SelectedDataOrNull = mCurrentSelectableDatas[index];
            mCurrentSelectableDatas = null;
        }

        public void OnGrowthCloseDownAnimEnd_UE()
        {
            sfGrowthUIParent.SetActive(false);
        }
    }
}