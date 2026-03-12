
namespace UnityEngine.UI
{
    public class HPBarBadgeIconHandler : MonoBehaviour
    {
        [SerializeField] private GameObject sfShieldBadge;
        private MonsterBase mMonsterBase;

        private void Awake()
        {
            mMonsterBase = GetComponentInParent<MonsterBase>();
            sfShieldBadge.gameObject.SetActive(false);
        }

        private void Start()
        {
            mMonsterBase.MonsterStatus.ShieldAmount.AddListener(updateShieldBadge, true);
        }


        private void updateShieldBadge(Gauge shieldAmount)
        {
            if(shieldAmount.Current <= 0.0f)
            {
                sfShieldBadge.SetActive(false);
            }
            else
            {
                sfShieldBadge.SetActive(true);
            }
        }

    }
}