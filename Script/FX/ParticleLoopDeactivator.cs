using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleLoopDeactivator : MonoBehaviour
{
    public void LoopPause()
    {
        ParticleSystem[] particles = GetComponentsInChildren<ParticleSystem>();
        for (int i = 0; i < particles.Length; i++)
        {
            particles[i].Stop();
        }
    }
}
