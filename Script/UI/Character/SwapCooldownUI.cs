using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

namespace UnityEngine.UI
{
    public class SwapCooldownUI : MonoBehaviour
    {
        [SerializeField] private Image sfSwapCooldownIndicator;
        [SerializeField] private TMP_Text sfCooldownTimeText;
        [SerializeField] private UIImageSwitcher sfMaskImageSwitcher;


        private void Awake()
        {
            sfSwapCooldownIndicator.fillAmount = 0.0f;
            sfSwapCooldownIndicator.enabled = false;
            sfCooldownTimeText.enabled = false;

        }

        private void Start()
        {
            PlayerManager.Instance.CurrentPlayer.AddListener(onPlayerChanged);
        }

        private void onPlayerChanged(PlayerCharacterController pc)
        {
            if (PlayerManager.Instance.ActivatedCharacters.Count <= 1)
            {
                return;
            }
            SkillActor switchingSkillActor = pc.SM.GetActor<SkillActor>(eActorType.SwitchAttack);

            if (!switchingSkillActor.IsOnAttack)
            {
                return;
            }

            float cooldown = switchingSkillActor.SkillConfig.CoolTime;
            StartCoroutine(swapCooldownRoutine(cooldown));
            sfMaskImageSwitcher.SwitchSprite(PlayerManager.Instance.ActivatedCharacters[1].CharName);
        }

        private void updateSwapUICooldown(float normalizedTime, float currentTime)
        {
            sfSwapCooldownIndicator.fillAmount = 1 - normalizedTime;
        }

        private IEnumerator swapCooldownRoutine(float duration)
        {
            float currentDuration = duration;
            sfSwapCooldownIndicator.enabled = true;
            sfCooldownTimeText.enabled = true;
            while (currentDuration >= 0.0f)
            {
                currentDuration -= Time.fixedDeltaTime;
                float nomalizeTimePass = 1 - currentDuration / duration;
                sfSwapCooldownIndicator.fillAmount = 1 - nomalizeTimePass;
                sfCooldownTimeText.text = $"{currentDuration:F1}";
                yield return new WaitForFixedUpdate();
            }
            sfSwapCooldownIndicator.enabled = false;
            sfCooldownTimeText.enabled = false;
        }

    }
}