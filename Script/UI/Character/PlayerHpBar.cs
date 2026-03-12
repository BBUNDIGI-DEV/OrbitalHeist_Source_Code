using System.Collections;
using System.Collections.Generic;
using TMPro;

namespace UnityEngine.UI
{
    public class PlayerHpBar : MonoBehaviour
    {
        [SerializeField] private GameObject[] sfSwapInfos; 
        [SerializeField] private AnimatedFillUpBarUI[] sfHPBar;
        [SerializeField] private UltimateGaugeUI[] sfUltimateBar;
        [SerializeField] private UIImageSwitcher[] sfFaceImages;
        [SerializeField] private CanvasGroup[] sfFaceCanvasGroup;
        [SerializeField] private TMP_Text sfText;

        private void Start()
        {
            PlayerManager.Instance.CurrentPlayer.AddListener(onPlayerChanged, true);
        }

        private void OnDestroy()
        {
            if (PlayerManager.IsExist)
            {
                PlayerManager.Instance.CurrentPlayer.RemoveListener(onPlayerChanged);
            }
        }

        private void Update()
        {
            if(!PlayerManager.IsExist)
            {
                return;
            }

            List<PlayerCharacterController> activatedPlayers = PlayerManager.Instance.ActivatedCharacters;

            for (int i = 0; i < sfHPBar.Length; i++)
            {
                if (i >= activatedPlayers.Count)
                {
                    sfSwapInfos[i - 1].SetActive(false);
                    continue;
                }

                if (i >= 1 && !sfSwapInfos[i - 1].activeInHierarchy)
                {
                    sfSwapInfos[i - 1].SetActive(true);
                }
            }

            for (int i = 0; i < activatedPlayers.Count; i++)
            {
                if (activatedPlayers[i].PlayerStatus.IsDead)
                {
                    sfFaceCanvasGroup[i].alpha = 0.4f;
                }
                else
                {
                    sfFaceCanvasGroup[i].alpha = 1.0f;
                }
                sfHPBar[i].EnqueueValue(activatedPlayers[i].CharacterStatus.NormalizedHP);
                sfUltimateBar[i].UpdateUltimateGuage(activatedPlayers[i].PlayerStatus.UltimateGuage);
                sfFaceImages[i].SwitchSprite(activatedPlayers[i].CharName);
            }
            sfText.text = $"{(int)PlayerManager.Instance.CurrentPlayer.Value.PlayerStatus.CurrentHP}/{(int)PlayerManager.Instance.CurrentPlayer.Value.PlayerStatus.MaxHP}";
        }

        private void onPlayerChanged()
        {
            List<PlayerCharacterController> activatedPlayers = PlayerManager.Instance.ActivatedCharacters;

            for (int i = 0; i < sfHPBar.Length; i++)
            {
                if (i >= activatedPlayers.Count)
                {
                    break;
                }
                sfHPBar[i].WarpValue(activatedPlayers[i].CharacterStatus.NormalizedHP);
                sfUltimateBar[i].ChangeIcon(activatedPlayers[i].CharName);
                sfUltimateBar[i].UpdateUltimateGuage(activatedPlayers[i].PlayerStatus.UltimateGuage);
                sfFaceImages[i].SwitchSprite(activatedPlayers[i].CharName);

            }
        }
    }
}
