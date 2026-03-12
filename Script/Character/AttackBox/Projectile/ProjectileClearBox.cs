using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileClearBox : MonoBehaviour
{
    private ProjectileHandler mHandler;

    private void Awake()
    {
        mHandler = GetComponentInParent<ProjectileHandler>();
        Collider attackBoxCollider = GetComponentInParent<AttackBoxElement>().GetComponent<Collider>();

        if(attackBoxCollider != null)
        {
            Physics.IgnoreCollision(attackBoxCollider, GetComponent<Collider>());
        }
    }


    public void OnTriggerEnter(Collider other)
    {
        
    }

    public void ClearPorjectile()
    {
        mHandler.ClearProjectile();
    }
}
