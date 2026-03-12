using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class MobileInputManager : InputHandlerBase
{
    private MobileInputUI mMobileInputUIOrNull
    {
        get
        {
            if(!mMobileInputUIDic.ContainsKey(mCurrentActivateMap))
            {
                return null;
            }

            return mMobileInputUIDic[mCurrentActivateMap];
        }
    }

    private Dictionary<eInputSections, MobileInputUI> mMobileInputUIDic;

    protected override void Awake()
    {
        base.Awake();
        mMobileInputUIDic = new Dictionary<eInputSections, MobileInputUI>();
        MobileInputUI[] inputHelpingUIs = GetComponentsInChildren<MobileInputUI>();
        for (int i = 0; i < inputHelpingUIs.Length; i++)
        {
            mMobileInputUIDic.Add(inputHelpingUIs[i].InputSection, inputHelpingUIs[i]);
        }
    }


    public override void AddInputCallback(eInputSections map, string action, Action<InputAction.CallbackContext> callback, InputActionPhase phase)
    {
        mMobileInputUIDic[map].ActionDics[action].AddInputCallback(callback);
    }

    public override void RemoveInputCallback(eInputSections map, string action, Action<InputAction.CallbackContext> callback, InputActionPhase phase)
    {
        mMobileInputUIDic[map].ActionDics[action].RemoveInputCallback(callback);
    }

    public override bool CheckPlatform(RuntimePlatform platform)
    {
        return checkPlatform(platform, RuntimePlatform.WindowsEditor, RuntimePlatform.Android);     
    }

    public override void DisableHandler()
    {
        gameObject.SetActive(false);
    }

    public override Vector3 GetAttackAim(Transform playerTrans)
    {
        return GetMoveDir();
    }

    public override Vector3 GetMoveDir()
    {
        Vector2 stickPos = mMobileInputUIDic[eInputSections.BattleGamePlay].JoystickPos;
        return new Vector3(stickPos.x, 0.0f, stickPos.y).normalized;
    }

    public override void SwitchActionMap(eInputSections newMap)
    {
        mMobileInputUIOrNull?.gameObject.SetActive(false);
        mCurrentActivateMap = newMap;
        mMobileInputUIOrNull?.gameObject.SetActive(true);
    }

    public override void SetEnabled(bool enabled)
    {
        mMobileInputUIOrNull.gameObject.SetActive(enabled);
    }


}
