namespace UnityEngine.UI
{
    [RequireComponent(typeof(HorizontalLayoutGroup))]
    public class HoriLayoutoutGroupSpaceSetterByChildCount : MonoBehaviour
    {
        [SerializeField] private int[] sfSpacingByChildCount;
        private HorizontalLayoutGroup mLayoutGroup;

        public void OnEnable()
        {
            updateSpacing();
        }

        public void UpdateSpacing()
        {
            updateSpacing();
        }

        [Sirenix.OdinInspector.Button]
        private void updateSpacing()
        {
            if (mLayoutGroup == null)
            {
                mLayoutGroup = GetComponent<HorizontalLayoutGroup>();
            }


            int childCount = mLayoutGroup.transform.GetActiveChildCount();

            childCount--;

            if (childCount < 0 || childCount >= sfSpacingByChildCount.Length)
            {
                return;
            }

            mLayoutGroup.spacing = sfSpacingByChildCount[childCount];
        }
    }
}
