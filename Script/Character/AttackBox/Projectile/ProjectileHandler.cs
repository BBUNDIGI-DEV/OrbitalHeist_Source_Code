using UnityEngine;
using Sirenix.OdinInspector;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(AttackBoxElement))]
public abstract class ProjectileHandler : PoolableMono
{
    public delegate void OnProjectileHit(GameObject other, Vector3 hitPoint);
    public bool IsHit
    {
        get; private set;
    }

    [SerializeField] protected float sfTimeLimit = 10.0f;
    [SerializeField] protected ParticleSystem sfProjectileBodyParticle;
    [SerializeField] protected ParticleSystem sfOnHitParticle;

    protected ProjectileInfo mInfo;
    protected Vector3 mThrowingDir;
    protected eTargetTag mTargetTag;
    protected float mCurrentTime;
    protected float mPiearcingCount;


    protected virtual void FixedUpdate()
    {
        mCurrentTime += Time.fixedDeltaTime;

        if (mCurrentTime < mInfo.Delay)
        {
            return;
        }

        if (mCurrentTime >= sfTimeLimit)
        {
            if (sfOnHitParticle != null && sfOnHitParticle.IsAlive() == true)
            {
                return;
            }

            gameObject.SetActive(false);
        }
    }

    public virtual void InitializeProjectile(Vector3 attackDir, eTargetTag targetTag, ProjectileInfo info, Vector3 shootingPoint = default)
    {
        transform.forward = attackDir;
        mTargetTag = targetTag;
        mCurrentTime = 0.0f;
        IsHit = false;
        if(shootingPoint != default)
        {
            transform.position = shootingPoint;
        }

        sfProjectileBodyParticle.gameObject.SetActive(true);
        if (sfOnHitParticle != null)
        {
            sfOnHitParticle.gameObject.SetActive(false);
        }
        mInfo = info;
        if(mInfo.MaxLifetime != 0.0f)
        {
            sfTimeLimit = mInfo.MaxLifetime;
        }
        mPiearcingCount = mInfo.PiearcingCount;
    }

    public void ClearProjectile()
    {
        mCurrentTime = sfTimeLimit;
    }

    protected void onProjectileHit(GameObject hitObject, Vector3 hitPoint)
    {
         if (IsHit)
        {
            return;
        }

        if (mCurrentTime < mInfo.Delay)
        {
            return;
        }

        bool isTarget = mTargetTag.CheckTarget(hitObject.tag);
        bool isObstacle = hitObject.layer == LayerMask.NameToLayer(eLayerName.Obstacle.ToString());

        if (!isTarget && !isObstacle)
        {
            return;
        }

        if(isObstacle)
        {
            IsHit = true;
            mCurrentTime = sfTimeLimit;
            sfProjectileBodyParticle.gameObject.SetActive(false);
            if (sfOnHitParticle != null)
            {
                sfOnHitParticle.gameObject.SetActive(true);
                sfOnHitParticle.transform.position = hitPoint;
            }
            return;
        }



        if (isTarget)
        {
            AttackBoxElement attackBoxElement = GetComponent<AttackBoxElement>();

            if(attackBoxElement.IsInHitStack(hitObject))
            {
                return;
            }

            if (sfOnHitParticle != null)
            {
                sfOnHitParticle.gameObject.SetActive(true);
                sfOnHitParticle.transform.position = hitPoint;
            }
            attackBoxElement.ProcessOnHit(hitObject);

            mPiearcingCount--;
            if(mPiearcingCount <= 0)
            {
                IsHit = true;
                mCurrentTime = sfTimeLimit;
                sfProjectileBodyParticle.gameObject.SetActive(false);
            }
        }
    }
}

[System.Serializable]
public struct ProjectileInfo
{
    public float Delay;
    public float Speed;
    public float MaxLifetime;
    public float PiearcingCount;
}