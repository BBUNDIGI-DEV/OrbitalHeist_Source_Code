using Febucci.UI;
using TMPro;
using UnityEditor;

namespace UnityEngine.UI
{
    public class DamageTextUIManager : MonoBehaviour
    {
        [SerializeField] private bool sfIsForPlayer;
        [SerializeField] private DamageTextUIElement sfTextUI;
        [SerializeField] private float sfSpawnWidth;
        [SerializeField] private float sfSpawnHeight;
        private GameObjectPool mDamageTextPool;

        private void Awake()
        {
            mDamageTextPool = GetComponent<GameObjectPool>();
        }

        private void Start()
        {
            if(sfIsForPlayer)
            {
                for(int i = 0; i < PlayerManager.Instance.AllPlayers.Length; i++)
                {
                    PlayerManager.Instance.AllPlayers[i].CharacterStatus.LastDamageInfo.AddListener(showDamageUI);
                    PlayerManager.Instance.AllPlayers[i].CharacterStatus.CurrentHP.AddListener(showHealedUI);
                }
            }
            else
            {
                CharacterBase characterBase = GetComponentInParent<CharacterBase>();
                characterBase.CharacterStatus.LastDamageInfo.AddListener(showDamageUI);
                characterBase.CharacterStatus.CurrentHP.AddListener(showHealedUI);
            }
        }

        private void OnDestroy()
        {
            CharacterBase characterBase = GetComponentInParent<CharacterBase>();
            if(characterBase != null)
            {
                characterBase.CharacterStatus.LastDamageInfo.RemoveListener(showDamageUI);
                characterBase.CharacterStatus.CurrentHP.RemoveListener(showHealedUI);
            }
        }

        private void showDamageUI(DamageInfo lastDamage)
        {
            DamageTextUIElement damageUI = mDamageTextPool.GetGameobject(sfTextUI);
            damageUI.GetComponent<RectTransform>().anchoredPosition += new Vector2(Random.Range(-sfSpawnWidth / 2, sfSpawnWidth / 2),
                                                            Random.Range(-sfSpawnHeight / 2, sfSpawnHeight / 2));


            if (sfIsForPlayer && PlayerManager.Instance.GlobalPlayerStatus.ForceShieldAmount != 0)
            {
                damageUI.SetDamageText("¹«Àû", 1, getColorGradient(lastDamage), false);
            }
            else
            {
                damageUI.SetDamageText(lastDamage.Damage, getColorGradient(lastDamage), false);
            }

            damageUI.transform.SetAsLastSibling();
        }

        private void showHealedUI(float prevHP, float newHP)
        {
            if(prevHP >= newHP)
            {
                return;
            }

            float healedAmount = newHP - prevHP;

            DamageTextUIElement damageUI = mDamageTextPool.GetGameobject(sfTextUI);
            damageUI.GetComponent<RectTransform>().anchoredPosition += new Vector2(Random.Range(-sfSpawnWidth / 2, sfSpawnWidth / 2),
                                                            Random.Range(-sfSpawnHeight / 2, sfSpawnHeight / 2));
            damageUI.SetDamageText(((int)healedAmount).ToString(), 1,eDamagedColorPreset.PlayerHelaed, false);
            damageUI.transform.SetAsLastSibling();
        }

        private eDamagedColorPreset getColorGradient(DamageInfo damageInfo)
        {
            if(damageInfo.DamagedByBuff == eBuffNameID.Fire)
            {
                return eDamagedColorPreset.FirDebuffDamage;
            }
            if(sfIsForPlayer)
            {
                if(PlayerManager.Instance.GlobalPlayerStatus.ForceShieldAmount != 0)
                {
                    return eDamagedColorPreset.ForceShieldDecrease;
                }
                return eDamagedColorPreset.PlayerDamaged;
            }
            else
            {
                if(damageInfo.AttackerOrNull != null && damageInfo.AttackerOrNull is PlayerCharacterController pc)
                {
                    switch (pc.CharName)
                    {
                        case eCharacterName.Glanda:
                            return eDamagedColorPreset.MonsterGlandaDamaged;
                        case eCharacterName.Hypo:
                            return eDamagedColorPreset.MonsterHypoDamaged;
                        case eCharacterName.Shiv:
                            return eDamagedColorPreset.MonsterShivDamaged;
                    }
                }
                return eDamagedColorPreset.Default;
            }
        }

#if UNITY_EDITOR
        private void OnDrawGizmosSelected()
        {
            Handles.color = Color.blue;
            Handles.DrawWireCube(transform.position, new Vector3(sfSpawnWidth, sfSpawnHeight));
        }
#endif
    }

    public enum eDamagedColorPreset
    {
        Default,
        MonsterGlandaDamaged,
        MonsterHypoDamaged,
        MonsterShivDamaged,
        PlayerDamaged,
        PlayerHelaed,
        FirDebuffDamage,
        ForceShieldDecrease,
    }
}
