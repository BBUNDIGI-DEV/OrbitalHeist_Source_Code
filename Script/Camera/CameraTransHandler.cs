using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraTransHandler : MonoBehaviour
{
    private readonly Vector3 RENDER_SIZE = new Vector2(4.5f, 4.0f);
    [SerializeField] private Transform sfTrackTrans;
    [SerializeField] private Bounds sfBound;
    [SerializeField][Range(0.05f, 0.3f)] private float sfTrackSpeed;

    private Vector3 mInitialOffset;


    private void Awake()
    {
        mInitialOffset = transform.position - sfTrackTrans.position;
    }


    private void LateUpdate()
    {
        Vector2 trackPos = sfTrackTrans.position + mInitialOffset;
        Vector2 curPos = transform.position;
        Vector2 placeHolder = Vector2.zero;
        Vector2 newPos = Vector2.SmoothDamp(curPos, trackPos, ref placeHolder, sfTrackSpeed);
        Vector2 clampPos = ClampByBound(newPos);
        transform.position = new Vector3(clampPos.x, clampPos.y,mInitialOffset.z);
    }

    private Vector3 ClampByBound(Vector3 input)
    {
        Vector2 boundCenter = sfBound.center + transform.parent.position;
        Vector2 boundExtents = sfBound.extents;

        float minX = boundCenter.x - boundExtents.x + RENDER_SIZE.x;
        float maxX = boundCenter.x + boundExtents.x - RENDER_SIZE.x;
        float minY = boundCenter.y - boundExtents.y + RENDER_SIZE.y;
        float maxY = boundCenter.y + boundExtents.y - RENDER_SIZE.y;

        Vector3 result = new Vector3(Mathf.Clamp(input.x, minX, maxX), Mathf.Clamp(input.y, minY, maxY));
        return result;
    }


}
