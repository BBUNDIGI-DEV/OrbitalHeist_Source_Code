using UnityEngine;

[RequireComponent(typeof(AttackBoxElement))]
public class ParticleCollisionProjectileHandler : ProjectileHandler
{
    [SerializeField] private Rigidbody sfProjectileParticle;
    protected Rigidbody mRB;


    public override void InitializeProjectile(Vector3 attackDir, eTargetTag targetTag, ProjectileInfo info, Vector3 shootingPoint)
    {
        if (mRB == null)
        {
            mRB = GetComponent<Rigidbody>();
        }

        base.InitializeProjectile(attackDir, targetTag, info, shootingPoint);

        sfProjectileParticle.gameObject.SetActive(true);
        mRB.velocity = transform.forward * mInfo.Speed;
    }

    protected override void FixedUpdate()
    {
        base.FixedUpdate();

        if (IsHit == true)
        {
            mRB.velocity = Vector3.zero;
        }

        if (mCurrentTime < mInfo.Delay)
        {
            return;
        }
    }

    protected void OnTriggerEnter(Collider other)
    {
        int attackBoxLayer = LayerMask.NameToLayer("AttackBox");
        if (other.gameObject.layer == attackBoxLayer)
        {
            return;
        }
        onProjectileHit(other.gameObject, other.ClosestPoint(mRB.position));
    }
}
