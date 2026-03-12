using Sirenix.OdinInspector;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class BilliardHelper : MonoBehaviour
{
    private const float M_BILLIARD_COOL_DOWN = 0.1f;

    [SerializeField] private float sfCollitionCheckingRaduis;
    [SerializeField, Range(0, 1)] private float sfSpeedReduction = 0.75f;
    [SerializeField, Range(0, 1)] private float sfNockbackTimeReduction = 0.75f;
    [SerializeField, Range(0, 1)] private float sfDamageReduction = 0.75f;

    [SerializeField, AssetsOnly] private ParticleBinder sfOnHitParticle;
    [SerializeField] private Transform sfBilliardRayOrigin;
    private IDamagable mOnObjectHit;
    private DamageInfo mOriginalDamageInfo;
    private int mBilliardCount
    {
        get
        {
            return _BilliardCount;
        }
        set
        {
            _BilliardCount = value;
            //Debug.Log($"Billiard Count:  { _BilliardCount }");
        }
    }
    private int _BilliardCount;

    private  void Awake()
    {
        mOnObjectHit = GetComponentInParent<IDamagable>();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(mBilliardCount == 0)
        {
            return;
        }

        Vector3 surfaceNormal = collision.contacts[0].normal;
        if (Vector3.Dot(-collision.relativeVelocity, surfaceNormal) >= 0f)
        {
            return;
        }

        DamageInfo newDamageInfo = mOriginalDamageInfo;
        newDamageInfo.HitData.KnockbackData.NockbackPower *= sfSpeedReduction;
        newDamageInfo.HitData.KnockbackData.NockbackTime *= sfNockbackTimeReduction;
        newDamageInfo.HitData.KnockbackData.BilladableCount--;
        newDamageInfo.HitData.OnHitFX = sfOnHitParticle;

        IDamagable obstalce = collision.gameObject.GetComponentInChildren<IDamagable>();
        if (obstalce != null)
        {
            //newDamageInfo.Damage *= M_ON_HIT_DAMAGE_REDUCTION;
            //newDamageInfo.HitData.KnockbackData.SetKnockbackDir(surfaceNormal);
            //obstalce.OnHit(newDamageInfo);
        }
        else
        {
            Vector3 inDirection = -collision.relativeVelocity;
            surfaceNormal *= -1;
            Vector3 refVector = Vector3.Reflect(inDirection, surfaceNormal);
            newDamageInfo.Damage *= sfDamageReduction;

            newDamageInfo.HitData.KnockbackData.SetKnockbackDir(refVector.normalized);
            mOnObjectHit.OnHit(newDamageInfo);
        }
        SetBillardInfo(newDamageInfo);
    }

    public void SetBillardInfo(DamageInfo originalBilliardInfo)
    {
        mOriginalDamageInfo = originalBilliardInfo;
        mBilliardCount = mOriginalDamageInfo.HitData.KnockbackData.BilladableCount;
    }

    public void StopBilliard()
    {
        mBilliardCount = 0;
    }
}
