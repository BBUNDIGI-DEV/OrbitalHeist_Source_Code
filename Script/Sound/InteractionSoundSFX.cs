using FMODUnity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractionSoundSFX : MonoBehaviour
{
    private const float M_COOL_DOWN = 5.0f;
    private float mCurrentCoolDown = 0.0f;

    private void Update()
    {
        if(mCurrentCoolDown >= 0.0f)
        {
            mCurrentCoolDown -= Time.deltaTime;
        }
    }

    public void PlayGeneratorSFX()
    {
        if(mCurrentCoolDown < 0.0f)
        {
            mCurrentCoolDown = M_COOL_DOWN;
            RuntimeManager.PlayOneShot("event:/SFX/Stage/SFX_Generator");
        }
    }

    public void PlayElectricSFX()
    {
        if (mCurrentCoolDown < 0.0f)
        {
            mCurrentCoolDown = M_COOL_DOWN;
            RuntimeManager.PlayOneShot("event:/SFX/Stage/SFX_CommonElectric");
        }
    }
}
