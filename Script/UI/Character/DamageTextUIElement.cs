using Sirenix.OdinInspector;
using TMPro;

namespace UnityEngine.UI
{
    public class DamageTextUIElement : PoolableMono
    {
        [SerializeField] private DamageUITextConfig sfDamageTextConfig;
        [SerializeField] private Animation sfAnimation;
        [SerializeField] private TMP_Text sfText;
        
        private bool mIsSnapShoted;
        private Vector3 mOriginalScale;
        private bool mIsPlayed;

        private void OnDisable()
        {
            transform.localScale = mOriginalScale;
            if(gameObject.activeSelf)
            {
                gameObject.SetActive(false);
            }
        }

        public void SetDamageText(float lastDamage, eDamagedColorPreset colorPreset, bool isCriticalAttack)
        {
            SetDamageText(((int)lastDamage).ToString(), convertDamageToFactor(lastDamage), colorPreset, isCriticalAttack);
        }

        public void SetDamageText(string contents, float factor, eDamagedColorPreset colorPreset, bool isCriticalAttack)
        {
            if (!mIsSnapShoted)
            {
                mOriginalScale = transform.localScale;
                mIsSnapShoted = true;
            }
            sfText.text = contents;
            sfText.colorGradientPreset = RuntimeDataLoader.DamagedGradientPreset[(int)colorPreset];
            setText(factor);
        }

        public void SetHealedText(float lastDamage)
        {
            if (!mIsSnapShoted)
            {
                mOriginalScale = transform.localScale;
                mIsSnapShoted = true;
            }
            sfText.text = ((int)lastDamage).ToString();
            setText(0);
        }


        [Button]
        private void setText(float factor)
        {
            float scaleMultiplier = convertMultiplierByFactor(sfDamageTextConfig.sfSizeUpMultiplier, sfDamageTextConfig.sfSizeDownMultiplier, factor);
            transform.localScale = mOriginalScale * scaleMultiplier;

            float speedMultiplier = convertMultiplierByFactor(sfDamageTextConfig.sfSizeUpMultiplier, sfDamageTextConfig.sfSizeDownMultiplier, factor * -1);
            sfAnimation[sfAnimation.clip.name].speed = speedMultiplier;
            sfAnimation.Rewind();
            sfAnimation.Play();
        }


        private float convertDamageToFactor(float damage)
        {
            float factor = (damage - sfDamageTextConfig.sfMinDamageThreshold) / (sfDamageTextConfig.sfMaxDamageThreshold - sfDamageTextConfig.sfMinDamageThreshold);
            factor = Mathf.Clamp01(factor);
            float remapRange = (factor - 0.5f) * 2;
            return remapRange;
        }

        private float convertMultiplierByFactor(float maxMultiplier, float minMultiplier, float factor)
        {
            if(factor > 0)
            {
                return Mathf.Lerp(1, maxMultiplier, factor);
            }
            else if(factor < 0)
            {
                factor = Mathf.Abs(factor);
                return Mathf.Lerp(1, 1 / minMultiplier, factor);
            }
            else
            {
                return 1;
            }
        }
    }
}
