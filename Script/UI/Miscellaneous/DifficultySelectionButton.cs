
namespace UnityEngine.UI
{
    public class DifficultySelectionButton : MonoBehaviour
    {
        public void SelectDifficulty(int level)
        {
            switch (level)
            {
                case 0:
                    MonsterBase.sGlobalData = Resources.Load<GlobalMonsterBalanceData>("BalanceData/Setting_001BalanceEasy");
                    break;
                case 1:
                    MonsterBase.sGlobalData = Resources.Load<GlobalMonsterBalanceData>("BalanceData/Setting_002BalanceNormal");
                    break;
                case 2:
                    MonsterBase.sGlobalData = Resources.Load<GlobalMonsterBalanceData>("BalanceData/Setting_003BalanceHard");
                    break;
                default:
                    Debug.Assert(false, $"Wrong Difficulty {level}");
                    break;
            }
        }
    }
}
