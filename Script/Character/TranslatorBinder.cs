using UnityEngine;
using UnityEngine.AI;

public class TranslatorBinder
{
    public NavMeshAgent Agent
    {
        get
        {
            //if (_Trans.name.Contains("Monster"))
            //{
            //    Debug.Log(_Trans.position, _Trans.gameObject);
            //}
            return mAgent;
        }
    }
    public Rigidbody RB
    {
        get
        {
            //if (_Trans.name.Contains("Monster"))
            //{
            //    Debug.Log(_Trans.position, _Trans.gameObject);
            //}
            return mRB;
        }
    }

    public Transform Trans
    {
        get
        {
            //if(_Trans.name.Contains("Monster"))
            //{
            //    Debug.Log(_Trans.position, _Trans.gameObject);
            //}
            return _Trans;
        }
    }

    public bool SkipUpdateTransform;

    private const float M_SET_DEST_COOLDOWN = 0.15f;
    private readonly NavMeshAgent mAgent;
    private readonly Rigidbody mRB;
    private eTranslatorType mCurrentComponent;
    private Transform _Trans;
    private float mSetDestCooldown;
    public TranslatorBinder(Rigidbody rb, NavMeshAgent agent)
    {
        Debug.Assert(rb != null);
        mRB = rb;
        mRB.isKinematic = false;
        _Trans = rb.transform;
        if (agent != null)
        {
            mAgent = agent;
        }
    }

    public void UpdateTransform()
    {
        switch (mCurrentComponent)
        {
            case eTranslatorType.Rigidbody:
                mRB.UpdateVelocity();
                mRB.UpdateRotation();
                break;
            case eTranslatorType.NavAgent:
                mSetDestCooldown -= Time.deltaTime;
                break;
            default:
                break;
        }

    }

    public void SwitchComponent(eTranslatorType type)
    {
        if (mCurrentComponent == type)
        {
            return;
        }
        eTranslatorType prevType = mCurrentComponent;
        mCurrentComponent = type;
        switch (prevType)
        {
            case eTranslatorType.Rigidbody:
                mRB.isKinematic = true;
                break;
            case eTranslatorType.NavAgent:
                mAgent.isStopped = true;
                mAgent.updatePosition = false;
                mAgent.updateRotation = false;
                mAgent.enabled = false;
                break;
        }
        switch (mCurrentComponent)
        {
            case eTranslatorType.Rigidbody:
                if(prevType == eTranslatorType.NavAgent)
                {
                    mRB.position = mAgent.nextPosition;
                }
                mRB.isKinematic = false;
                break;
            case eTranslatorType.NavAgent:
                mAgent.enabled = true;
                mAgent.updateRotation = true;
                mAgent.updatePosition = true;
                mAgent.isStopped = false;
                if (prevType == eTranslatorType.Rigidbody)
                {
                    mAgent.Warp(mRB.position);
                }

                break;
        }
    }

    public void SetDestination(Vector3 dest)
    {
        if(mSetDestCooldown >= 0.0f)
        {
            return;
        }
        mSetDestCooldown = M_SET_DEST_COOLDOWN;
        Agent.SetDestination(dest);
    }

}

public enum eTranslatorType
{
    None,
    Rigidbody,
    NavAgent,
}