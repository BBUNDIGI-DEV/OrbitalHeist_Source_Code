using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(ParticleSystem))]
public class ParticleDeactivator : MonoBehaviour
{
    private ParticleSystem C_PS;

    private void Awake()
    {
        C_PS = GetComponent<ParticleSystem>();

    }

    private void Update()
    {

    }
}
