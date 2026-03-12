using FMODUnity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorAnimSoundEffect : MonoBehaviour
{
    public void PlayDoorOpenSoundEffect()
    {
        RuntimeManager.PlayOneShot("event:/SFX/Stage/SFX_CommonGate");
    }
}
