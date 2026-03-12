using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "DataContainer/CameraConfig")]
public class CameraConfig : ScriptableObject
{
    public float CameraMoveSensitivity = 0.14f;
    public float DampingSpeedX = 0.005f;
    public float DampingSpeedY = 0.005f;
    public float FadeOutSpeed = 2.5f;
    public float FadeInSpeed = 3f;
}
