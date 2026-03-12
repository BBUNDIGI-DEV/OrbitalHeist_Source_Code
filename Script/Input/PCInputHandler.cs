using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;

public class PCInputHandler : InputHandlerBase
{
    public class ActionMap
    {
        public InputAction this[string Tag]
        {
            get
            {
                return mActionDic[Tag];
            }
        }

        private Dictionary<string, InputAction> mActionDic;

        public void InitializeActionDic<T>(InputActionAsset actionAsset, eInputSections actionMapName) where T : System.Enum
        {
            mActionDic = new Dictionary<string, InputAction>();
            InputActionMap actionMap = actionAsset.FindActionMap(actionMapName.ToString());
            Debug.Assert(actionMap != null, $"Cannot found action map: [{actionMapName}]");
            string[] enumString = System.Enum.GetNames(typeof(T));
            for (int i = 0; i < enumString.Length; i++)
            {
                var action = actionMap.FindAction(enumString[i]);
                Debug.Assert(action != null, $"Cannot found action in action map" +
                    $": [Action Map {actionMapName}, Action {enumString[i]}]");
                mActionDic.Add(enumString[i], action);
            }
        }
    }

    [SerializeField] private InputActionAsset sfInputAction;
    private Dictionary<eInputSections, ActionMap> mActionMaps;

    protected override void Awake()
    {
        mActionMaps = new Dictionary<eInputSections, ActionMap>();
        InputSystem.onEvent += onAnyInput;
        for (int i = 0; i < (int)eInputSections.Count; i++)
        {
            eInputSections curMap = (eInputSections)i;
            ActionMap actionMap = new ActionMap();
            switch (curMap)
            {
                case eInputSections.BattleGamePlay:
                    actionMap.InitializeActionDic<eBattleInputName>(sfInputAction, curMap);
                    break;
                case eInputSections.CutScene:
                    actionMap.InitializeActionDic<eCutSceneInputName>(sfInputAction, curMap);
                    break;
                case eInputSections.Dialouge:
                    actionMap.InitializeActionDic<eDialougeInputName>(sfInputAction, curMap);
                    break;
                case eInputSections.Menu:
                    actionMap.InitializeActionDic<eMenuInputName>(sfInputAction, curMap);
                    break;
                case eInputSections.UI:
                    actionMap.InitializeActionDic<eUIInputName>(sfInputAction, curMap);
                    break;
                default:
                    Debug.LogError($"Cannot use this enum value [{curMap}]");
                    break;
            }
            sfInputAction.FindActionMap(curMap.ToString()).Disable();
            mActionMaps.Add(curMap, actionMap);
        }
    }

    private void OnDestroy()
    {
        InputSystem.onEvent -= onAnyInput;
    }


    public override Vector3 GetMoveDir()
    {
        if (mCurrentActivateMap != eInputSections.BattleGamePlay)
        {
            return Vector3.zero;
        }

        var moveAction = mActionMaps[eInputSections.BattleGamePlay][eBattleInputName.Move.ToString()];
        if(!moveAction.enabled)
        {
            return Vector3.zero;
        }

        Vector2 moveInputValue = moveAction.ReadValue<Vector2>();
        Vector3 moveDir = new Vector3(moveInputValue.x, 0.0f, moveInputValue.y);
        moveDir = moveDir.normalized;
        return moveDir;
    }

    public override Vector3 GetAttackAim(Transform playerTrans)
    {
        if (CurrentInputDevice == eInputDeviceType.GamePad)
        {
            Vector3 moveDir = GetMoveDir();
            if (moveDir != Vector3.zero)
            {
                return moveDir;
            }
        }
        else
        {
            Vector3 playerPos = playerTrans.position;
            Plane charPlane = new Plane(Vector3.up, playerPos);
            Vector3 planeHitPoint;
            Vector3 playerToMouse = Vector3.zero;
            Vector3 returnDir = Vector3.forward;
            Vector2 currentMousePos = Mouse.current.position.ReadValue();
            if (CameraManager.Instance.ScreenPointToPlaneHitPoint(currentMousePos, charPlane, out planeHitPoint))
            {
                playerToMouse = planeHitPoint - playerPos;
                playerToMouse.Normalize();
                returnDir = playerToMouse;
                return returnDir;
            }
        }
        return playerTrans.forward;
    }

    public override void SwitchActionMap(eInputSections newMap)
    {
        if (mCurrentActivateMap == newMap)
        {
            return;
        }

        if (mCurrentActivateMap != eInputSections.None)
        {
            sfInputAction.FindActionMap(mCurrentActivateMap.ToString()).Disable();
        }

        sfInputAction.FindActionMap(newMap.ToString()).Enable();
        mCurrentActivateMap = newMap;
    }

    public override void AddInputCallback(eInputSections map, string action, System.Action<InputAction.CallbackContext> callback, InputActionPhase phase)
    {
        switch (phase)
        {
            case InputActionPhase.Started:
                mActionMaps[map][action].started += callback;
                break;
            case InputActionPhase.Performed:
                mActionMaps[map][action].performed += callback;
                break;
            case InputActionPhase.Canceled:
                mActionMaps[map][action].canceled += callback;
                break;
            default:
                Debug.LogError($"You Cannot use this statement as input pahse [{phase}]");
                break;
        }
    }

    public override void RemoveInputCallback(eInputSections map, string action, System.Action<InputAction.CallbackContext> callback, InputActionPhase phase )
    {
        switch (phase)
        {
            case InputActionPhase.Started:
                mActionMaps[map][action].started -= callback;
                break;
            case InputActionPhase.Performed:
                mActionMaps[map][action].performed -= callback;
                break;
            case InputActionPhase.Canceled:
                mActionMaps[map][action].canceled -= callback;
                break;
            default:
                Debug.LogError($"You Cannot use this statement as input pahse [{phase}]");
                break;
        }
    }

    public override void DisableHandler()
    {
        gameObject.SetActive(false);
    }

    public override bool CheckPlatform(RuntimePlatform platform)
    {
        return checkPlatform(platform, RuntimePlatform.WindowsEditor, RuntimePlatform.WindowsPlayer);
    }

    public override void SetEnabled(bool enabled)
    {
        if (enabled)
        {
            sfInputAction.FindActionMap(mCurrentActivateMap.ToString()).Enable();
        }
        else
        {
            sfInputAction.FindActionMap(mCurrentActivateMap.ToString()).Disable();
        }
    }

    private void onAnyInput(UnityEngine.InputSystem.LowLevel.InputEventPtr eventPtr, InputDevice device)
    {
        if (!eventPtr.IsA<UnityEngine.InputSystem.LowLevel.StateEvent>()
            && !eventPtr.IsA<UnityEngine.InputSystem.LowLevel.DeltaStateEvent>())
        {
            return;
        }

        bool isNoisyEvent = true;
        foreach (var control in eventPtr.EnumerateChangedControls())
        {
            if(control.device is not Gamepad)
            {
                isNoisyEvent = false;
                break;
            }

            if(control.shortDisplayName == null)
            {
                isNoisyEvent = false;
                return;
            }

            if(!control.shortDisplayName.Contains("LS") && !control.shortDisplayName.Contains("RS"))
            {
                isNoisyEvent = false;
                break;
            }

            float stickDelta = (float)control.ReadValueFromEventAsObject(eventPtr);
            if (stickDelta > 0.05f)
            {
                isNoisyEvent = false;
                break;
            }
        }

        if(isNoisyEvent)
        {
            return;
        }

        eInputDeviceType deviceType = deviceChecking(device);

        if (CurrentInputDevice != deviceType)
        {
            CurrentInputDevice.Value = deviceType;
            Debug.Log($"Received event for {device}");
        }
    }
    private eInputDeviceType deviceChecking(InputControl control)
    {
        eInputDeviceType device = eInputDeviceType.KeyboardAndMouse;

        if (control.device is Gamepad)
        {
            return eInputDeviceType.GamePad;
        }
        if (control.device is Keyboard || control.device is Mouse)
        {
            return eInputDeviceType.KeyboardAndMouse;
        }
        return device;
    }
}
