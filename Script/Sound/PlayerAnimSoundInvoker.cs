using UnityEngine;

public class PlayerAnimSoundInvoker : MonoBehaviour
{
    private ActorStateMachine mPlayerSM;

    private void Start()
    {
        PlayerCharacterController controller = GetComponentInParent<PlayerCharacterController>();
        if(controller == null)
        {
            return;
        }
        mPlayerSM = controller.SM;
    }

    public void PlayPlayerFootstepSoundEffect()
    {
        if(mPlayerSM == null)
        {
            SFXManager.PlayPlayerFootstepSound();
            return;
        }

        if (mPlayerSM.CurrentActorType.IsAttackType())
        {
            return;
        }

        if (!mPlayerSM.Anim.Animator.GetBool("IsRun"))
        {
            return;
        }
        SFXManager.PlayPlayerFootstepSound();
    }

    public void PlayPlayerAttack1or2SoundEffect()
    {
        //RuntimeManager.PlayOneShot("event:/SFX/PC/Attack/SFX_NormalAttackSwing1&2");
    }

    public void PlayPlayerFinalAttackSoundEffect()
    {
        //RuntimeManager.PlayOneShot("event:/SFX/PC/Attack/SFX_NormalAttackSwing3");
    }

    public void PlayPlayerFinalAttackCrashSoundEffect()
    {
        //RuntimeManager.PlayOneShot("event:/SFX/PC/Attack/SFX_NormalAttac3Impact");
    }

    public void PlayPlayerSpecialAttackSoundEffect()
    {
        //RuntimeManager.PlayOneShot("event:/SFX/PC/Attack/SFX_QSkillSwing");
    }

    public void PlayPlayerSpecialAttackCrashSoundEffect()
    {
       // RuntimeManager.PlayOneShot("event:/SFX/PC/Attack/SFX_QSkillImpact");
    }
}
