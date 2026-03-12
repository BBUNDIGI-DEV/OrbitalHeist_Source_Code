using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class SimpleTrigger : MonoBehaviour
{
    private System.Action mOnTriggerEnter;

    public void SetCallback(System.Action action)
    {
        mOnTriggerEnter += action;
    }

    private void OnTriggerEnter(Collider other)
    {
        mOnTriggerEnter?.Invoke();
    }

    public void SetEnable(bool toggleEnabled)
    {
        GetComponent<Collider>().enabled = toggleEnabled;
    }
}   
