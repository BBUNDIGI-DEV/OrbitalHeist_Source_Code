using UnityEngine;

public class ForceCounterInvokingBox : PoolableMono
{
    public delegate void OnAttackBoxHit(float damage);
    protected eTargetTag mTargetTag;
    private CharacterBase mCounterBoxOwner;

    protected virtual void OnTriggerEnter(Collider other)
    {
        if(!mTargetTag.CheckTarget(other.tag))
        {
            return;
        }


        ProcessOnHit(other.gameObject);


    }

    public void SetCounterBoxData(eTargetTag targetTag, CharacterBase owner)
    {
        mTargetTag = targetTag;
        mCounterBoxOwner = owner;
    }

    public void ProcessOnHit(GameObject hitObject)
    {
        CharacterBase attacker = hitObject.GetComponentInParent<CharacterBase>();
        Debug.Assert(attacker != null, $"You Cannot hit not damagable object [{hitObject}]");

        if(!attacker.SM.CurrentActorType.IsAttackType())
        {
            return;
        }

        SkillActor skillActor = attacker.SM.CurrentActorOrNull as SkillActor;
        if(skillActor.SkillConfig.CountableType == eCountableType.EasyCountable)
        {
            skillActor.InvokeForceCountering(mCounterBoxOwner);
            mCounterBoxOwner.OnHit(new DamageInfo(0, HitEffectData.GetTickForForceCounterInvoking(), attacker));
            gameObject.SetActive(false);
        }
    }
}

