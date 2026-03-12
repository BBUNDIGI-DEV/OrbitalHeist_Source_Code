
namespace UnityEngine.UI
{
    public class MonsterHpBar : MonoBehaviour
    {
        [SerializeField] private Image sfHPFillUp;
        [SerializeField] private Image sfShieldFillUp;

        private float mLastUsedMaxHP;
        private float mLastUsedCurrentHP;
        private bool mIsEnabled;

        private void Start()
        {
            MonsterBase  monsterController = GetComponentInParent<MonsterBase>();
            Debug.Assert(monsterController != null, "MonsterHPBar is initailzied without Parent Common monster Controller");

            monsterController.MonsterStatus.MaxHP.AddListener(updateMaxHP, true);
            monsterController.MonsterStatus.CurrentHP.AddListener(updateCurrentHP, true);
            monsterController.MonsterStatus.ShieldAmount.AddListener(updateShield, true);
            monsterController.MonsterStatus.IsDead.AddListener(deactive);

            mIsEnabled = false;
            gameObject.SetActive(false);
        }

        private void updateShield(Gauge newShieldGauge)
        {
            updateShieldUI(newShieldGauge.Normalize);
        }

        private void updateMaxHP(float newMaxHP)
        {
            mLastUsedMaxHP = newMaxHP;
            updateHPUI();
        }

        private void updateCurrentHP(float newHP)
        {
            if (!mIsEnabled)
            {
                gameObject.SetActive(true);
                mIsEnabled = true;
            }
            mLastUsedCurrentHP = newHP;
            updateHPUI();
        }

        private void updateHPUI()
        {
            sfHPFillUp.fillAmount = mLastUsedCurrentHP / mLastUsedMaxHP;
        }

        private void updateShieldUI(float fillAmount)
        {
            if (!mIsEnabled)
            {
                gameObject.SetActive(true);
                mIsEnabled = true;
            }

            sfShieldFillUp.fillAmount = fillAmount;
        }

        private void deactive()
        {
            gameObject.SetActive(false);
        }
    }
}
