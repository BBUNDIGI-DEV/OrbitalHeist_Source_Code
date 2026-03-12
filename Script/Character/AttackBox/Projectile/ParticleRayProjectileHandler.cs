using UnityEngine;

[RequireComponent(typeof(AttackBoxElement))]
public class ParticleRayProjectileHandler : ProjectileHandler
{
    [SerializeField] private float sfRayRadius;
    [SerializeField] private ParticleProjectilePositionTracker sfBodyPartilcePositionTracker;

    private Vector3 mPrevPos;
    private LayerMask mLayerMask;
    private RaycastHit[] mCashedHitResult;

    public override void InitializeProjectile(Vector3 attackDir, eTargetTag targetTag, ProjectileInfo info, Vector3 shootingPoint = default)
    {
        if(mLayerMask == 0)
        {
            mLayerMask = LayerMask.GetMask(eLayerName.HitBox.ToString(), eLayerName.Obstacle.ToString(), eLayerName.PassableObstacle.ToString());
            mCashedHitResult = new RaycastHit[16];
        }

        base.InitializeProjectile(attackDir, targetTag, info, shootingPoint);

        sfBodyPartilcePositionTracker.SetParticleProjectileSpeed(mInfo.Speed);
    }

    protected override void FixedUpdate()
    {
        base.FixedUpdate();

        if (mCurrentTime < mInfo.Delay)
        {
            return;
        }

        if (IsHit == false)
        {
            if (mPrevPos == Vector3.zero)
            {
                mPrevPos = transform.position;
            }
            sfBodyPartilcePositionTracker.UpdatePos();

            Vector3 deltaPos = sfBodyPartilcePositionTracker.Pos - mPrevPos;
            int count = Physics.SphereCastNonAlloc(mPrevPos, sfRayRadius, deltaPos.normalized, mCashedHitResult, deltaPos.magnitude, mLayerMask, QueryTriggerInteraction.Collide);
            if (count != 0)
            {
                for (int i = 0; i < count; i++)
                {
                    if(mCashedHitResult[i].colliderInstanceID == GetComponent<AttackBoxElement>().OwnerHitbox.GetInstanceID())
                    {
                        continue;
                    }
                    onProjectileHit(mCashedHitResult[i].collider.gameObject, mCashedHitResult[i].point);
                }
            }
        }
        else
        {
            sfBodyPartilcePositionTracker.ClearProjectile();
        }
    }

    private void LateUpdate()
    {
        mPrevPos = sfBodyPartilcePositionTracker.Pos;
    }
}
