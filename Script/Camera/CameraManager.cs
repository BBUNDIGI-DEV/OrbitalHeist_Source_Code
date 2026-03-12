using UnityEngine;
using Cinemachine;

[RequireComponent(typeof(CinemachineBrain))]
public class CameraManager : SingletonClass<CameraManager>
{
    public CameraActingHelper Actor
    {
        get
        {
            if(mActor == null)
            {
                mActor = GetComponent<CameraActingHelper>();
            }
            return mActor;
        }
    }

    public Camera MainCamera
    {
        get; private set;
    }

    public Camera UICamera
    {
        get
        {
            return sfUICamera;
        }
    }

    [SerializeField] private Camera sfUICamera;
    private CameraActingHelper mActor;

    protected override void Awake()
    {
        base.Awake();
        MainCamera = Camera.main;
    }

    private void Start()
    {
    }

    public bool ScreenPointToPlaneHitPoint(Vector3 mousePoint, Plane plane, out Vector3 hitPoint)
    {
        Ray worldMouseRay = MainCamera.ScreenPointToRay(mousePoint);
        float distance;
        hitPoint = Vector3.zero;
        if (plane.Raycast(worldMouseRay, out distance))
        {
            hitPoint = worldMouseRay.GetPoint(distance);
            return true;
        }
        return false;
    }

    public void EnableUICamera()
    {
        sfUICamera.cullingMask = LayerMask.GetMask("UI");
    }

    public void DisableUICamera()
    {
        sfUICamera.cullingMask = 0;
    }
}

