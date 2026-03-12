using UnityEngine;


[RequireComponent(typeof(AttackBoxElement))]
public class MeleeboxProjectile : MonoBehaviour
{
    [SerializeField] private ParticleSystem sfProjectileBody;
    private float mSpeed = 1.0f;
    private float mDuration = 1.0f;

    private float mCurrentDuration = 0.0f;

    public void IntializeMeleeboxProjectile(float speed, float duration)
    {
        mSpeed = speed;
        mDuration = duration;
        if (!sfProjectileBody.main.loop)
        {
            ParticleSystem.MainModule main = sfProjectileBody.main;
            float lifeTime = sfProjectileBody.main.startLifetime.constant;
            float multiplier = lifeTime / mDuration;
            main.simulationSpeed = multiplier;
        }
    }

    private void Update()
    {
        if(mCurrentDuration >= mDuration)
        {
            if(!sfProjectileBody.isStopped)
            {
                sfProjectileBody.Stop(true, ParticleSystemStopBehavior.StopEmitting);
            }
            if (!sfProjectileBody.IsAlive(true))
            {
                gameObject.SetActive(false);
            }
            return;
        }


        transform.position = transform.position + transform.forward * mSpeed * Time.deltaTime;
        mCurrentDuration += Time.deltaTime;
    }

    private void OnDisable()
    {
        mCurrentDuration = 0.0f;
    }
}
