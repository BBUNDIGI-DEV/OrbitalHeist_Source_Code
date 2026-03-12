using UnityEngine;

using UnityEngine.Events;
public class ParticleEventReciver : MonoBehaviour
{
    [SerializeField] private UnityEvent sfOnParticleCollison;
    [SerializeField] private UnityEvent sfOnParticleTrigger;

    public void OnParticleCollision(GameObject other)
    {
        sfOnParticleCollison.Invoke();
    }
    public void OnParticleTrigger()
    {
        sfOnParticleTrigger.Invoke();

    }
}
