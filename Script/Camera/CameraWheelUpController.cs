using UnityEngine;
using static UnityEngine.InputSystem.InputAction;
using Cinemachine;

public class CameraWheelUpController : MonoBehaviour
{
    [SerializeField] private const float sfTriggerThreshold = 0.1f;
    [SerializeField] private float sfMaxDistance;
    [SerializeField] private float sfMinDistance;
    [SerializeField] private float sfZoomSpeed;
    [SerializeField] private float sfWheelUpPower = 1.0f;

    private CinemachineFramingTransposer mTransposer;
    private float mInitialDistance;
    private float mCurrentDistance;
    private float mDestDistance;

    private void Awake()
    {
        mTransposer = GetComponent<CinemachineVirtualCamera>().GetCinemachineComponent<CinemachineFramingTransposer>();
        mInitialDistance = mTransposer.m_CameraDistance;
        mCurrentDistance = mInitialDistance;
        mDestDistance = mCurrentDistance;
    }

    public void Start()
    {
        InputManager.Instance.AddInputCallback(eInputSections.BattleGamePlay, eBattleInputName.ZoomInZoomUp.ToString(), onWheal, UnityEngine.InputSystem.InputActionPhase.Performed);
    }

    public void Update()
    {
        if(Mathf.Abs(mCurrentDistance - mDestDistance) > sfTriggerThreshold)
        {
            mCurrentDistance = Mathf.Lerp(mCurrentDistance, mDestDistance, sfZoomSpeed * Time.deltaTime);
            mTransposer.m_CameraDistance = mCurrentDistance;
        }
    }

    public void ReturnToInitialDistance()
    {
        mDestDistance = mInitialDistance;
    }

    private void onWheal(CallbackContext context)
    {
        Vector2 deltaPos = context.ReadValue<Vector2>();

        if(deltaPos.y > 0)
        {
            mDestDistance -= sfWheelUpPower;
        }
        else
        {
            mDestDistance += sfWheelUpPower;
        }
        mDestDistance = Mathf.Clamp(mDestDistance, sfMinDistance, sfMaxDistance);

    }
}
