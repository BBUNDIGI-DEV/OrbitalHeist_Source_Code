using UnityEngine;

[RequireComponent(typeof(Animator))]
public class CharacterAnimator : MonoBehaviour
{
    public readonly float RUN_ANIM_INVOKE_THRESHOLD = 0.05f;

    public bool IsAttackAnimPlayed
    {
        get
        {
            AnimatorStateInfo stateInfo = Animator.GetCurrentAnimatorStateInfo((int)eAnimatorLayer.AttackLayer);

            return !stateInfo.IsName("AttackIdle");
        }
    }

    public Animator Animator
    {
        get
        {
            if(mAnimator == null)
            {
                mAnimator = GetComponentInChildren<Animator>();
            }
            return mAnimator;
        }
    }

    public Animator MatAnimator
    {
        get
        {
            if (mMatAnimator == null)
            {
                mMatAnimator = transform.parent.GetComponent<Animator>();

            }
            return mMatAnimator;
        }
    }

    public float NormalizeDurationCurrentAnim
    {
        get
        {
            AnimatorStateInfo stateInfo = Animator.GetCurrentAnimatorStateInfo((int)eAnimatorLayer.AttackLayer);
            // current normalized progress (0 ¡æ 1, looping if >1)
            float normalizedTime = stateInfo.normalizedTime;
            return normalizedTime;
        }
    }

    private Animator mAnimator;
    private Animator mMatAnimator;



    public void UpdateMovementAnim(Vector3 velocity, bool isFixedUpdate = true)
    {
        float deltaTime = isFixedUpdate ? Time.fixedDeltaTime : Time.deltaTime;
        float runAnimPlayThreashold = RUN_ANIM_INVOKE_THRESHOLD * RUN_ANIM_INVOKE_THRESHOLD;
        Vector3 horiMoveVelocity = new Vector3(velocity.x, 0.0f, velocity.z);

        if (horiMoveVelocity.sqrMagnitude >= runAnimPlayThreashold)
        {
            Animator.SetFloat(eAnimatorParams.MovementBlendFactor.ToString(), 1.0f, 0.1f, deltaTime);
            Animator.SetBool(eAnimatorParams.IsRun.ToString(), true);
        }
        else
        {
            Animator.SetFloat(eAnimatorParams.MovementBlendFactor.ToString(), 0.0f, 0.1f, deltaTime);
            Animator.SetBool(eAnimatorParams.IsRun.ToString(), false);
        }
    }

    public void SetMovementBlendFactor(float value)
    {
        Animator.SetFloat(eAnimatorParams.MovementBlendFactor.ToString(), value);
    }

    public void TriggerDashAnim(float speed)
    {
        Animator.PlayOrRewind("Dash");
        Animator.SetFloat("DashSpeedMultiplier", speed);
    }

    public void TriggerAttackAnim(SkillConfig config)
    {
        if(config.IsComboAttack)
        {
            Animator.SetInteger("ComboCount", config.ComboIndex);
            AnimatorStateInfo curInfo = Animator.GetCurrentAnimatorStateInfo(0);
            if(Animator.IsInTransition(0) || !curInfo.IsName(config.name))
            {
                Animator.PlayOrRewind($"{config.BaseConfig.ActorType}_Combo{config.ComboIndex + 1}" );
            }
        }
        else if(config.IsChargingAttack)
        {
            Animator.PlayOrRewind($"{config.BaseConfig.ActorType}_Charging");
        }
        else if(config.IsChannellingSkill)
        {
            Animator.PlayOrRewind($"{config.BaseConfig.ActorType}_ChannellingReady");
        }
        else
        {
            Animator.PlayOrRewind($"{config.BaseConfig.ActorType}_Combo1");
        }
        Animator.SetBool(eAnimatorParams.IsOnAttack.ToString(),true);
    }

    public void TriggerChargingEnd(SkillConfig config)
    {
        Debug.Assert(config.IsChargingAttack, "You Cannot call charging end animation with not charging skill config");
            Animator.SetTrigger(eAnimatorParams.ChargingEnd.ToString());
    }

    public void TriggerChargingPaused(SkillConfig config)
    {
        Debug.Assert(config.IsChargingAttack, "You Cannot call charging end animation with not charging skill config");
        Animator.SetTrigger(eAnimatorParams.ChargingPaused.ToString());
    }

    public void PlayStunAnim()
    {
        Animator.SetBool(eAnimatorParams.IsStuned.ToString(), true);
        Animator.PlayOrRewind(eGotoStateName.Stuned.ToString());
    }

    public void PlayStunAnim(float speed)
    {
        PlayStunAnim();
        Animator.SetFloat(eAnimatorParams.StunSpeedMultiplier.ToString(), speed);
    }

    public void PlayOnHitMaterailAnim()
    {
        MatAnimator.SetTrigger(eAnimatorParams.OnHitMaterialAnim.ToString());
    }

    public void PauseStunAnim()
    {
        Animator.SetBool(eAnimatorParams.IsStuned.ToString(), false);
    }

    public void SetAttackSpeed(float speedMultiplier)
    {
        Animator.SetFloat(eAnimatorParams.AttackSpeedMultiplier.ToString(), speedMultiplier);
    }

    public void EndAttackAnim()
    {
        Animator.SetBool(eAnimatorParams.IsOnAttack.ToString(), false);
    }

    public void PlayDamagedAnim(float speed)
    {
        Animator.PlayOrRewind(eGotoStateName.Damaged.ToString());
        Animator.SetFloat(eAnimatorParams.DamagedSpeedMultiplier.ToString(), speed);
        MatAnimator.SetTrigger(eAnimatorParams.OnHitMaterialAnim.ToString());
        Animator.PlayOrRewind(eGotoStateName.AttackIdle.ToString(), 1);
    }

    public void PlayAppearanceAnim(float speed)
    {
        Animator.PlayOrRewind(eGotoStateName.Appearance.ToString());
        Animator.SetFloat(eAnimatorParams.ApperanceSpeedMultiplier.ToString(), speed);
    }

    public void PlayDeadAnim(float speed)
    {
        Animator.PlayOrRewind(eGotoStateName.Dead.ToString());
        Animator.SetFloat(eAnimatorParams.DeadSpeedMultiplier.ToString(), speed);
        Animator.PlayOrRewind(eGotoStateName.AttackIdle.ToString(), 1);
    }   

    public void PlayDeadDissolve()
    {
        MatAnimator.SetTrigger(eAnimatorParams.OnDeadDissolveMaterialAnim.ToString());
    }

    public void SetBurningFactor(float factor)
    {
        factor = Mathf.Clamp01(factor);
        MatAnimator.SetFloat(eAnimatorParams.BurningFactor.ToString(), factor);
    }

    public void PauseAnim()
    {
        Animator.speed = 0.05f;
    }

    public void ReusemeAnim()
    {
        Animator.speed = 1.0f;
    }

    public void WarpToSpecifiedSkillEvent(eSkillEventMarkerType markerType)
    {
        int layer = 1;
        if (Animator.IsInTransition(layer))
        {
            return;
        }

        AnimatorStateInfo currentState = Animator.GetCurrentAnimatorStateInfo(layer);

        AnimatorClipInfo[] clips = Animator.GetCurrentAnimatorClipInfo(layer);
        if(clips.Length == 0)
        {
            return;
        }

        AnimationClip currentClip = clips[0].clip;

        AnimationEvent[] animEvent = currentClip.events;

        for (int i = 0; i < animEvent.Length; i++)
        {
            if (!animEvent[i].IsSkillEvent())
            {
                continue;
            }

            animEvent[i].GetSkillEventData(out eActorType actor, out eSkillEventMarkerType outMarkerType);

            if(outMarkerType == markerType)
            {
                float normalizedTime = animEvent[i].time / currentClip.length;
                Debug.Log(normalizedTime);
                Animator.Play(currentState.fullPathHash, layer, normalizedTime);
                Animator.Update(0f); //Force Update Animator for changing motion in this frame
                return;
            }
        }
    }

    public void SetChannelingEnd()
    {
        Animator.SetTrigger(eAnimatorParams.ChannelingEndTrigger.ToString());
    }

}

//Non Order Sensitive
public enum eAnimatorParams
{
    None = -1,
    IsOnAttack,
    IsRun,
    IsStuned,
    MovementBlendFactor,
    AttackTrigger,
    BasicAttackComboCount,
    DamagedSpeedMultiplier,
    DashSpeedMultiplier,
    AttackSpeedMultiplier,
    ApperanceSpeedMultiplier,
    StunSpeedMultiplier,
    DeadSpeedMultiplier,
    ToxicGuage,
    ChargingEnd,
    ChargingPaused,
    OnHitMaterialAnim,
    OnDeadDissolveMaterialAnim,
    OnApperanceDissolveMaterialAnim,
    BurningFactor,
    ChannelingEndTrigger,
}

//Non Order Sensitive
public enum eGotoStateName
{
    Damaged,
    Dash,
    Stuned,
    Appearance,
    Dead,
    AttackIdle
}

//Order Sensitive
public enum eAnimatorType
{
    PlayerBase,
    RoboBoss,
}

public enum eAnimatorLayer
{
    MovementLayer,
    AttackLayer,
    MovementOverlay,
    MaterialOverlay,
    SecondMaterialOverlay,
}

