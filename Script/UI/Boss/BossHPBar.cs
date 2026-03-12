using UnityEngine;
using TMPro;

namespace UnityEngine.UI
{
    public class BossHPBar : MonoBehaviour
    {
        [SerializeField] private AnimatedFillUpBarUI sfFillImage;
        [SerializeField] private TMP_Text sfText;
        private MonsterStatus mOwnerStatus;
        private void Start()
        {
            MonsterBase bossMonsterBase = GetComponentInParent<MonsterBase>();
            mOwnerStatus = bossMonsterBase.MonsterStatus;
            bossMonsterBase.MonsterStatus.MaxHP.AddListener(updateHP, true);
            bossMonsterBase.MonsterStatus.CurrentHP.AddListener(updateHP, true);
            sfFillImage.EnqueueValue(1.0f);
        }

        private void OnDestroy()
        {
            if (!SceneSwitchingManager.IsExist)
            {
                return;
            }

        }

        private void updateHP(float curHP)
        {
            sfFillImage.EnqueueValue(mOwnerStatus.NormalizedHP);

            if (curHP <= 0.0f)
            {
                gameObject.SetActive(false);
            }
            sfText.text = $"{(int)mOwnerStatus.CurrentHP}/{(int)mOwnerStatus.MaxHP}";
        }
    }
}