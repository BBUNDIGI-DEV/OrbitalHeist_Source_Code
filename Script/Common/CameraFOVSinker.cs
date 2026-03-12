using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFOVSinker : MonoBehaviour
{
    public Camera MainCamera;
    private Camera mCurrentCamera;

    private void Awake()
    {
        MainCamera = Camera.main;
        mCurrentCamera = GetComponent<Camera>();
    }

    // Update is called once per frame
    void LateUpdate()
    {
        mCurrentCamera.fieldOfView = MainCamera.fieldOfView;
    }
}
