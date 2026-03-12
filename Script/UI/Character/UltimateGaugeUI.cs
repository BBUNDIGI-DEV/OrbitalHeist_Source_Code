using UnityEngine;
using UnityEngine.UI;

namespace UnityEngine.UI
{ 
    public class UltimateGaugeUI : MonoBehaviour
    {
        [SerializeField] private Image sfUltimateGuageIndicator;
        [SerializeField] private Image sfExtraImageOnFull;
        [SerializeField] private Image sfIcon;
        [SerializeField] private UIImageSwitcher sfIconSwitcher;
        [SerializeField] private UIImageSwitcher sfGuageIndicatorSwitcher;
        [SerializeField] private UIImageSwitcher sfExtraImageONFullSwitcher;
        [SerializeField] private ParticleSystem[] sfOnFullCharagedParticels;
        private ParticleSystem mFullChargedParticles;

        public void ChangeIcon(eCharacterName charName)
        {
            sfIconSwitcher.SwitchSprite(charName);
            sfGuageIndicatorSwitcher.SwitchSprite(charName);
            sfExtraImageONFullSwitcher.SwitchSprite(charName);
            if (sfOnFullCharagedParticels == null || sfOnFullCharagedParticels.Length == 0)
            {
                return;
            }

            if (mFullChargedParticles != null)
            {
                mFullChargedParticles.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
            }
            mFullChargedParticles = sfOnFullCharagedParticels[(int)charName];
        }

        public void UpdateUltimateGuage(Gauge normalizedGuage)
        {
            sfUltimateGuageIndicator.fillAmount = normalizedGuage.Normalize;
            if (normalizedGuage.Normalize >= 1.0f)
            {
                if (!sfExtraImageOnFull.gameObject.activeInHierarchy)
                {
                    sfUltimateGuageIndicator.fillAmount = 0.0f;
                    sfExtraImageOnFull.gameObject.SetActive(true);
                    sfIcon.color = new Color(1.0f, 1.0f, 1.0f, 1.0f);
                }

                if (sfOnFullCharagedParticels == null || sfOnFullCharagedParticels.Length == 0)
                {
                    return;
                }

                if (!mFullChargedParticles.gameObject.activeInHierarchy)
                {
                    mFullChargedParticles.gameObject.SetActive(true);
                }

            }
            else if (normalizedGuage.Normalize < 1.0f)
            {
                if (sfExtraImageOnFull.gameObject.activeInHierarchy)
                {
                    sfExtraImageOnFull.gameObject.SetActive(false);
                    sfIcon.color = new Color(1.0f, 1.0f, 1.0f, 0.5f);
                }

                if (sfOnFullCharagedParticels == null || sfOnFullCharagedParticels.Length == 0)
                {
                    return;
                }

                if (mFullChargedParticles.gameObject.activeInHierarchy)
                {
                    mFullChargedParticles.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
                }
            }
        }
    }
}