using UnityEngine.Events;

namespace UnityEngine.UI
{
    [ExecuteAlways]
    [RequireComponent(typeof(Slider))]
    public class StepSlider : MonoBehaviour
    {
        public int Step
        {
            get; private set;
        }

        public UnityEvent<float> OnStepChanged;

        [SerializeField] private int sfStepCount;

        public void OnValueChanged(float normalizedValue)
        {
            float strechedValue = normalizedValue * sfStepCount;
            int round = (int)Mathf.Round(strechedValue);
            float snappedValue = round / (float)sfStepCount;

            GetComponent<Slider>().value = snappedValue;
            Step = round;
            OnStepChanged?.Invoke(Step);
        }
    }

}
