using FMOD.Studio;
using FMODUnity;
using UnityEngine;

public class ChannelingSkillActor : SkillActor
{
    public bool IsOnChanneling
    {
        get; private set;
    }

    private float mCurrentTime;
    private ParticleBinder mSpawnedChannelingFX;
    private EventInstance mSFXEventInstance;
    public ChannelingSkillActor(ActorStateMachine onwerStateMachine, SkillConfig config, AttackBoxElement.OnAttackBoxHit onAttackBoxHit, System.Action onAttackEnd) 
        : base(onwerStateMachine, config, onAttackBoxHit, onAttackEnd)
    {
        Debug.Assert(SkillConfig.IsChannellingSkill,
            $"SkillConfig for ChargeSkillActor must be set as ChargeAttack");
        SetEnabledUpdating(false);
    }

    public override void InovkeActing(object parameter1, object parameter2)
    {
        base.InovkeActing(parameter1, parameter2);
        mCurrentTime = 0.0f;
        SetEnabledUpdating(true);
    }
      
    protected override void setSkillProgressState(eSkillProgressState state)
    {
        switch (state)
        {
            case eSkillProgressState.None:
                break;
            case eSkillProgressState.Preparing:
                break;
            case eSkillProgressState.OnAttack:
                SkillConfig.ChannelingAttackData.ColliderRemainTime = SkillConfig.ChannelingDuration;
                if(!SkillConfig.ChannelingAttackData.IsNull())
                {
                    handleAttackBox(SkillConfig.ChannelingAttackData);
                }

                if (!SkillConfig.ChannelingProjectileData.IsNull())
                {
                    handleProjectileAttackData(SkillConfig.ChannelingProjectileData);
                }

                if(!SkillConfig.ChannelingFX.IsNull())
                {
                    mSpawnedChannelingFX = spawnFX(SkillConfig.ChannelingFX);
                }
                if (!SkillConfig.ChannelingLoopSFX.IsNull)
                {
                    mSFXEventInstance = RuntimeManager.CreateInstance(SkillConfig.ChannelingLoopSFX);
                    mSFXEventInstance.start();
                }
                break;
            case eSkillProgressState.Cancelable:
                if(mSpawnedChannelingFX != null)
                {
                    mSpawnedChannelingFX.StopFX();
                    mSpawnedChannelingFX = null;
                }
                OWNER.Translator.RB.EnrollSetVelocity(Vector3.zero, BaseConfig.ActorType);
                if (!SkillConfig.ChannelingLoopSFX.IsNull)
                {
                    mSFXEventInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
                }
                break;
            default:
                Debug.LogError($"Default switch detected [{CurrentProgressState}]");
                break;
        }
        base.setSkillProgressState(state);
    }

    public override void UpdateActing(float deltaTime)
    {
        base.UpdateActing(deltaTime);
        if (CurrentProgressState == eSkillProgressState.Preparing)
        {
            return;
        }

        if (OWNER.IsPlayerActor)
        {
            OWNER.Translator.RB.EnrollLookRotation(InputManager.Instance.GetAttackAim(OWNER.Translator.Trans), BaseConfig.ActorType);
        }

        mCurrentTime += deltaTime;
        if(mCurrentTime > SkillConfig.ChannelingDuration)
        {
            Anim.SetChannelingEnd();
            SetEnabledUpdating(false);
        }
    }

    public override void StopActing()
    {
        base.StopActing();
        if (!SkillConfig.ChannelingLoopSFX.IsNull)
        {
            mSFXEventInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        }
    }

    public override void DestoryActor()
    {
        base.DestoryActor();
        if (!SkillConfig.ChannelingLoopSFX.IsNull)
        {
            mSFXEventInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        }
    }
}
