using UnityEngine;

public class PlayerInteractiveManager : MonoBehaviour
{
    private InteractableBase mDectedObjectOrNull;
    private GlobalPlayerStatus mPlyerStatus;

    private void Awake()
    {
        mPlyerStatus = GetComponentInParent<PlayerManager>().GlobalPlayerStatus;
    }

    public void EnableInputCallback()
    {
        InputManager.Instance.AddInputCallback(eInputSections.BattleGamePlay, eBattleInputName.Interactive.ToString(), tryInteractive);
    }

    public void DisableInputCallback()
    {
        InputManager.Instance.RemoveInputCallback(eInputSections.BattleGamePlay, eBattleInputName.Interactive.ToString(), tryInteractive);
    }

    public void SetDetectedObject(InteractableBase baseObjectOrNull)
    {
        if(baseObjectOrNull == null)
        {
            mDectedObjectOrNull = null;
            mPlyerStatus.DetectedInteractableObject.Value = eInteractableType.None;
            return;
        }

        mDectedObjectOrNull = baseObjectOrNull;
        mPlyerStatus.DetectedInteractableObject.Value = baseObjectOrNull.ObjectType;
    }

    private void tryInteractive(UnityEngine.InputSystem.InputAction.CallbackContext context)
    {
        if (mPlyerStatus.DetectedInteractableObject.Value == eInteractableType.None)
        {
            return;
        }
        mPlyerStatus.DetectedInteractableObject.Value = eInteractableType.None;
        mDectedObjectOrNull.InvokeInteraction();
        mDectedObjectOrNull = null;
    }
}
