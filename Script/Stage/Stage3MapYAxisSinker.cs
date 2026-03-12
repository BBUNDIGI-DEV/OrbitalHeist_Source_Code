using UnityEngine;

public class Stage3MapYAxisSinker : MonoBehaviour
{
    [SerializeField] private float sfYAxis;

    public void SinkToGround()
    {
        transform.position.Set(transform.position.x, 0.0f, transform.position.z);
    }

    public void SinkToBattle()
    {
        Vector3 position = transform.position;
        transform.position = new Vector3 (position.x, sfYAxis, position.z);
    }
}
