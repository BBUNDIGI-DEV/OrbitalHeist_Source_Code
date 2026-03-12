using FMODUnity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossAnimSoundEffect : MonoBehaviour
{
    public void PlayBossAttackSoundEffect()
    {
        RuntimeManager.PlayOneShot("event:/SFX/Monster/Boss/SFX_BossAttack");
    }

    public void PlayBossSpinSoundEffect()
    {
        RuntimeManager.PlayOneShot("event:/SFX/Monster/Boss/SFX_BossSpin");
    }

    public void PlayBossReadyRushSoundEffect()
    {
        RuntimeManager.PlayOneShot("event:/SFX/Monster/Boss/SFX_BossPreparingRush");
    }

    public void PlayBossRushSoundEffect()
    {
       RuntimeManager.PlayOneShot("event:/SFX/Monster/Boss/SFX_BossRushing");
    }

    public void PlayBossRushHitSoundEffect()
    {
        RuntimeManager.PlayOneShot("event:/SFX/Monster/Boss/SFX_BossRushStunned");
    }
}
