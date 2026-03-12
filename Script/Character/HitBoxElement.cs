using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class HitBoxElement : MonoBehaviour
{
    public Collider Collider
    {
        get
        {
            if(mCollider == null)
            {
                mCollider = GetComponent<Collider>();
            }

            return mCollider;
        }
    }

    private Collider mCollider;
}
