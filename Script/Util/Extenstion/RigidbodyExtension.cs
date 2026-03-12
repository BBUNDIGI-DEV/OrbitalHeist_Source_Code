using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class RigidbodyExtension
{
    private static Dictionary<int, LayeredRigidbody> mLayeredRBDics;
    static RigidbodyExtension()
    {
        mLayeredRBDics = new Dictionary<int, LayeredRigidbody>();
    }

    public static LayeredRigidbody GetLayeredRigidbody(this Rigidbody rb)
    {
        return getRigidbodyLayerOrAdd(rb);
    }

    public static void DebugSetVelocity(this Rigidbody rb, Vector3 velocity, string user = "")
    {
        if (rb.velocity == velocity) // Only Cover Vector3.zero 
        {
            return;
        }

        rb.velocity = velocity;
    }

    public static void DebugSetRotation(this Rigidbody rb, Quaternion rot, string user)
    {
        if (rb.rotation == rot) // Only Cover Quaternion.Identity
        {
            return;
        }

        rb.transform.rotation = rot;
    }

    public static Vector3 GetForward(this Rigidbody rb)
    {
        return rb.transform.forward;
    }

    public static void EnrollSetVelocity(this Rigidbody rb, Vector3 velocity, eActorType actorType)
    {
        if(actorType == eActorType.UltimateAttack)
        {
            Debug.Log("Check Call Stack Enroll");
        }
        bool isTopPriority = rb.getRigidbodyLayerOrAdd().EnrollVelocity(velocity, actorType);
        if(isTopPriority)
        {
            rb.UpdateVelocity();
        }
    }

    public static void DisEnrollSetVelocity(this Rigidbody rb, eActorType actorType)
    {
        if (actorType == eActorType.UltimateAttack)
        {
            Debug.Log("Check Call Stack dis");
        }
        if (!mLayeredRBDics.ContainsKey(rb.GetInstanceID()))
        {
            return;
        }
        LayeredRigidbody layeredRB = rb.getRigidbodyLayerOrAdd();
        layeredRB.DisEnrollVelocity(actorType);
        rb.UpdateVelocity();
    }

    public static void EnrollLookRotation(this Rigidbody rb, Vector3 lookRotation, eActorType actorType)
    {
        rb.getRigidbodyLayerOrAdd().EnrollLookRotation(lookRotation, actorType);
    }

    public static void EnrollLookRotationAndForceRotating(this Rigidbody rb, Vector3 lookRotation, eActorType actorType)
    {
        rb.getRigidbodyLayerOrAdd().EnrollLookRotation(lookRotation, actorType);
        rb.rotation = Quaternion.LookRotation(lookRotation.normalized, Vector3.up);
    }

    public static void DisEnrollLookRotatoin(this Rigidbody rb, eActorType actorType)
    {
        if (!mLayeredRBDics.ContainsKey(rb.GetInstanceID()))
        {
            return;
        }
        rb.getRigidbodyLayerOrAdd().DisEnrollLookRotation(actorType);
        rb.UpdateRotation();
    }

    public static void UpdateVelocity(this Rigidbody rb)
    {
        int instanceID = rb.GetInstanceID();
        if (!mLayeredRBDics.ContainsKey(instanceID))
        {
            return;
        }
        var velocitySetter = mLayeredRBDics[instanceID];
        Vector3 velocity;
        eActorType actor;
        bool result = velocitySetter.TryGetVelocity(out velocity, out actor);

        if (result)
        {
            if(actor == eActorType.Dead)
            {
                rb.DebugSetVelocity(velocity, actor.ToString());
                return;
            }
            rb.DebugSetVelocity(velocity);
        }
        else
        {
            rb.DebugSetVelocity(Vector3.zero);
        }
    }

    public static void UpdateRotation(this Rigidbody rb)
    {
        int instanceID = rb.GetInstanceID();
        if (!mLayeredRBDics.ContainsKey(instanceID))
        {
            return;
        }
        LayeredRigidbody layeredRB = mLayeredRBDics[instanceID];
        Vector3 lookRotation;
        eActorType actor;

        bool result = layeredRB.TryGetLookRotationToward(rb.GetForward(), out lookRotation, out actor);
        if (result)
        {
            rb.DebugSetRotation(Quaternion.LookRotation(lookRotation, Vector3.up), actor.ToString());
        }
    }


    public static Vector3 GetVelocityByActor(this Rigidbody rb, eActorType actorType)
    {
        int instanceID = rb.GetInstanceID();
        if (!mLayeredRBDics.ContainsKey(instanceID))
        {
            return Vector3.zero;
        }

        return mLayeredRBDics[instanceID].GetVelocity(actorType);
    }

    private static LayeredRigidbody getRigidbodyLayerOrAdd(this Rigidbody rb)
    { 
        int instanceID = rb.GetInstanceID();
        if (!mLayeredRBDics.ContainsKey(rb.GetInstanceID()))
        {
            LayeredRigidbody newLayer = new LayeredRigidbody();
            mLayeredRBDics.Add(instanceID, newLayer);
            return newLayer;
        }
        return mLayeredRBDics[instanceID];
    }
}
