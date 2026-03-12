using FMODUnity;
using FMOD.Studio;

public static class SFXManager
{
    public static void PlayMonsterHitSound()
    {
        RuntimeManager.PlayOneShot("event:/SFX/Enemy/Common/SFX_EnemyHit");
    }

    public static void PlayPlayerFootstepSound()
    {
        RuntimeManager.PlayOneShot("event:/SFX/PC/Common/SFX_PC_Public_FootStep");
    }

    public static void PlayPlayerPublicDashSound()
    {
        RuntimeManager.PlayOneShot("event:/SFX/PC/Common/SFX_PC_Public_Dash");
    }

    public static void PlayPlayerPublicHitSound()
    {
        RuntimeManager.PlayOneShot("event:/SFX/PC/Common/SFX_PC_Hit");
    }

    public static void PlayPlayerDeathSound()
    {
        RuntimeManager.PlayOneShot("event:/SFX/PC/Common/SFX_PC_Hit");
    }

    public static void PlayGameOverSOund()
    {
        RuntimeManager.PlayOneShot("event:/SFX/PC/Common/SFX_PC_Hit");
    }
}
