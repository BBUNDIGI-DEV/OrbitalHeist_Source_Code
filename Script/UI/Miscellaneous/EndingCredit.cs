namespace UnityEngine.UI
{
    public class EndingCredit : MonoBehaviour
    {
        [SerializeField] private Animation sfCreditAnim;
        [SerializeField] private TMPro.TMP_Text sfSpeedText;
        private int mSpeed = 1;
        public void IncreaseSpeed()
        {
            if (mSpeed == 32)
            {
                mSpeed = 1;
            }
            else
            {
                mSpeed = mSpeed * 2;
            }

            sfSpeedText.text = $"x{mSpeed}";
            sfCreditAnim.GetComponent<LegacyAnimSpeedMultipilier>().SetSpeed(mSpeed);
        }

        public void OnEndingCreditEnd()
        {
            SceneSwitchingManager.Instance.LoadOtherScene(eSceneName.Scene_Title, true);
        }
    }
}