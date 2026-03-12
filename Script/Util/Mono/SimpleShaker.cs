using System.Collections;
using UnityEngine;

    public class SimpleShaker : MonoBehaviour
    {
        [SerializeField] private float sfDuration = 1.0f;
        [SerializeField] private float sfShakePower = 1.0f;
        [SerializeField] private AnimationCurve sfShakeCurve;

        public void PlayShake()
        {
            StopAllCoroutines();
            StartCoroutine(Shaking());
        }

        public void PauseShake()
        {
            StopCoroutine(Shaking());
        }

        public IEnumerator Shaking()
        {
            Vector3 startPos = transform.position;
            float elapseTime = 0.0f;
            while (elapseTime < sfDuration)
            {
                elapseTime += Time.deltaTime;
                float strength = sfShakeCurve.Evaluate(elapseTime / sfDuration) * sfShakePower;
                transform.position = startPos + Random.insideUnitSphere * strength;
                yield return null;
            }

            transform.position = startPos;
            enabled = false;
        }
    }
