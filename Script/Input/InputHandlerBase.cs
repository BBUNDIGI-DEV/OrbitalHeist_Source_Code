using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;

public abstract class InputHandlerBase : MonoBehaviour
{
    public ObservedData<eInputDeviceType> CurrentInputDevice;
    protected eInputSections mCurrentActivateMap;

    public abstract void SetEnabled(bool enabled);
    public abstract Vector3 GetMoveDir();
    public abstract Vector3 GetAttackAim(Transform playerTrans);
    public abstract void SwitchActionMap(eInputSections newMap);
    public abstract void AddInputCallback(eInputSections map, string action, System.Action<InputAction.CallbackContext> callback, InputActionPhase phase);
    public abstract void RemoveInputCallback(eInputSections map, string action, System.Action<InputAction.CallbackContext> callback, InputActionPhase phase);
    public abstract void DisableHandler();
    public abstract bool CheckPlatform(RuntimePlatform platform);

    protected bool checkPlatform(RuntimePlatform platform, params RuntimePlatform[] includedPlatforms)
    {
        int usingPlatform = 0;
        for (int i = 0; i < includedPlatforms.Length; i++)
        {
            usingPlatform |= 1 << (int)includedPlatforms[i];
        }
        return (usingPlatform & (1 << (int)platform)) == (1 << (int)platform);
    }
        
    protected virtual void Awake()
    {
        //if (SceneSwitchingManager.Instance.CurrentScene == eSceneName.Scene_StoryCutscene)
        //{
        //    SwitchActionMap(eInputSections.CutScene);
        //}
        //else
        //{
        //    SwitchActionMap(eInputSections.BattleGamePlay);
        //}
    }
}
