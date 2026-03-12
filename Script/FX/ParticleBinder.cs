using Sirenix.OdinInspector;
using UnityEngine;

[RequireComponent(typeof(ParticleSystem))]
public class ParticleBinder : PoolableMono
{
    [SerializeField] private bool sfDeactiveManually;
    [SerializeField] private bool sfIgnoreSetSpeed;

    private eFXTransformType mSpawnType;
    private Transform mOwnerTrans;
    private Vector3 mLocalOffset;
    private ParticleSystem mMainParticle;
    private ParticleSystem[] mParticles;
    private float mOriginalSpeed = -1.0f;
    private Vector3 mOriginalScale;
    private Vector3 mInitialPos;

    private void Awake()
    {
        mMainParticle = GetComponent<ParticleSystem>();
        mParticles = GetComponentsInChildren<ParticleSystem>();
    }

    private void Update()
    {
        if(!sfDeactiveManually && !isParticleActivated())
        {
            gameObject.SetActive(false);
        }


        switch (mSpawnType)
        {
            case eFXTransformType.World:
                break;
            case eFXTransformType.Local:
                transform.position = (mOwnerTrans.rotation * mLocalOffset) + mOwnerTrans.position;
                transform.rotation = mOwnerTrans.rotation;
                break;
            case eFXTransformType.PositionOnlyWorld:
                transform.position = (mOwnerTrans.rotation * mLocalOffset) + mOwnerTrans.position;
                break;
            case eFXTransformType.RotationOnlyWorld:
                transform.rotation = mOwnerTrans.rotation;
                transform.position = mInitialPos + (mOwnerTrans.rotation * mLocalOffset);
                break;
            case eFXTransformType.LocalActivatedCharacter:
                Transform activeTrans = PlayerManager.Instance.AcitvatedPlayerTrans;
                transform.position = (activeTrans.rotation * mLocalOffset) + activeTrans.position;
                break;
            default:
                break;
        }
    }

    private void OnDisable()
    {
        if (mOriginalScale != Vector3.zero)
        {
            transform.localScale = mOriginalScale;
        }
    }

    public void SetGlobalActivatedPlayerFX()
    {
        mSpawnType = eFXTransformType.LocalActivatedCharacter;
    }

    public void SetFXTransformType(Vector3 initialPos, Quaternion initialRot)
    {
        transform.position = initialPos;
        transform.rotation = initialRot;
        mSpawnType = eFXTransformType.World;
    }

    public void SetFXTransformType(eFXTransformType spawnType, Transform ownerTransform, Vector3 offset = default)
    {
        SetFXTransformType(spawnType, ownerTransform.position, ownerTransform.rotation, ownerTransform, offset);
    }

    public void SetFXTransformType(eFXTransformType spawnType, Quaternion initialRot, Transform ownerTransform, Vector3 offset = default)
    {
        SetFXTransformType(spawnType, ownerTransform.position, initialRot, ownerTransform, offset);
    }

    public void SetFXTransformType(eFXTransformType spawnType, Vector3 initialPos, Transform ownerTransform, Vector3 offset = default)
    {
        SetFXTransformType(spawnType, initialPos, ownerTransform.rotation, ownerTransform, offset);
    }

    public void SetFXTransformType(eFXTransformType spawnType, Vector3 initialTrans, Quaternion initialRot, Transform ownerTransformOrNull, Vector3 offset = default)
    {
        transform.position = initialTrans;
        transform.rotation = initialRot;
        mSpawnType = spawnType;
        mOwnerTrans = ownerTransformOrNull;
        mLocalOffset = offset;
        mInitialPos = initialTrans;
        if (spawnType == eFXTransformType.World)
        {
            transform.position += (initialRot * offset);
        }
    }

    public override void ActiveFromPool()
    {
        base.ActiveFromPool();
    }

    public override bool CanReusable()
    {
        return base.CanReusable();
    }

    public override bool CheckSame(Component comparer)
    {
        return base.CheckSame(comparer);
    }

    public void SetSimulationSpeed(float speed)
    {
        if(sfIgnoreSetSpeed)
        {
            return;
        }
        if(!isParticleActivated())
        {
            return;
        }
        if(mOriginalSpeed == -1)
        {
            mOriginalSpeed = mParticles[0].main.simulationSpeed;
        }

        for (int i = 0; i < mParticles.Length; i++)
        {
            ParticleSystem.MainModule main = mParticles[i].main;
            main.simulationSpeed = mOriginalSpeed * speed;
        }
    }

    public void ScaleFX(float sizeMultiplier)
    {
        mOriginalScale = transform.localScale;
        sizeMultiplier = 1 + sizeMultiplier;
        transform.localScale *= sizeMultiplier;
    }

    public void StopFX()
    {
        mMainParticle.Stop(true, ParticleSystemStopBehavior.StopEmitting);
    }

    public void StopFXImmediate()
    {
        mMainParticle.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
        gameObject.SetActive(false);
    }

    private bool isParticleActivated()
    {
        if( mMainParticle.IsAlive(true))
        {
            return true;
        }

        return false;
    }
}

public enum eFXTransformType
{
    World,
    Local,
    PositionOnlyWorld,
    RotationOnlyWorld,
    LocalActivatedCharacter,
}


public enum eFXLifetimeType
{
    OnParticleSystem,
    HandleByManual
}