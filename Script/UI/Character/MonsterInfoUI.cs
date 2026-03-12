using TMPro;

namespace UnityEngine.UI
{
    public class MonsterInfoUI : MonoBehaviour
    {
        [SerializeField] private TMP_Text sfMonsterCountText;

        private int mTotalMonsterCount;
        private int mCurrentMonsterCount;

        private void Awake()
        {
        }

        public void Start()
        {
            RuntimeDataLoader.RuntimeStageData.LastMonsterCount.AddListener(updateLastMonsterCount, true);
            RuntimeDataLoader.RuntimeStageData.TotalMonsterCount.AddListener(updateTotalMonsterCount, true);
        }

        public void OnDestroy()
        {
            RuntimeDataLoader.RuntimeStageData.LastMonsterCount.RemoveListener(updateLastMonsterCount);
            RuntimeDataLoader.RuntimeStageData.TotalMonsterCount.RemoveListener(updateTotalMonsterCount);
        }

        private void updateLastMonsterCount(int newCurrentMonsterCount)
        {
            mCurrentMonsterCount = newCurrentMonsterCount;
            updateMonsterCountUI();
        }

        private void updateTotalMonsterCount(int newTotalMonsterCount)
        {
            mTotalMonsterCount = newTotalMonsterCount;
            updateMonsterCountUI();
        }

        private void updateMonsterCountUI()
        {
            sfMonsterCountText.text = mCurrentMonsterCount + "/" + mTotalMonsterCount;
        }
    }
}