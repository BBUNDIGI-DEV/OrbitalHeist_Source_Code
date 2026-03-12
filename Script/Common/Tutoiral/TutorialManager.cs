using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class TutorialManager : MonoBehaviour
{
    public enum eTutorialState
    {
        None = -1,
        NormalAttack,
        SwapAttack,
        UltimateAttack,
    }

    [SerializeField] private InputActionAsset sfInputAsset;
    [SerializeField] private TutorialUI sfTutoiralUI;
    private eTutorialState mCurrentState;
    private List<eBattleInputName> mLockedInput;
    private int mBufferedStepCount;
    private bool mIsFocusedOn;
    private Coroutine mTutorialRoutine;

    private void Awake()
    {
        mLockedInput = new List<eBattleInputName>();
    }

    public void StartTutorial()
    {
        mLockedInput.Add(eBattleInputName.NormalAttack);
        mLockedInput.Add(eBattleInputName.SwitchPlayerToNext);
        mLockedInput.Add(eBattleInputName.UltimateSkill);
        mCurrentState = eTutorialState.None;
        MoveToNextTutorialStep();
    }

    public void MoveToNextTutorialStep()
    {
        if(mTutorialRoutine != null || mIsFocusedOn)
        {
            mBufferedStepCount += 1;
        }
        else
        {
            mTutorialRoutine = StartCoroutine(moveToNextTutorialStep());
        }
    }

    public void OnFocusedInputReceived(InputAction.CallbackContext context)
    {
        unFocuseInput(mCurrentState);
    }

    private IEnumerator moveToNextTutorialStep()
    {
        yield return new WaitForSeconds(0.75f);
        mCurrentState++;
        sfTutoiralUI.SetTutorialUI(mCurrentState);
        focusOnInput(mCurrentState);
        mTutorialRoutine = null;
    }

    private void focusOnInput(eTutorialState tutoiralState)
    {
        TimeScaleUtil.Instance.AddTimeScale("TutoiralFocusInput", new PriorityAndTimeScalePair(eTimeScaleTrigger.Tutoiral, 0.1f));
        if(tutoiralState == eTutorialState.UltimateAttack)
        {
            PlayerManager.Instance.CurrentPlayer.Value.PlayerStatus.UltimateGuage.Value =
                PlayerManager.Instance.CurrentPlayer.Value.PlayerStatus.UltimateGuage.Value.MaximizeGauge();
        }
        eBattleInputName waitInput = convertTutorialStateToBattleInput(tutoiralState);
        InputActionMap actionMap = sfInputAsset.FindActionMap(eInputSections.BattleGamePlay.ToString());
        string[] battleInputString = System.Enum.GetNames(typeof(eBattleInputName));
        for (int i = 0; i < battleInputString.Length; i++)
        {
            InputAction action = actionMap.FindAction(battleInputString[i]);
            if (battleInputString[i] == waitInput.ToString())
            {
                action.Enable();
                action.started += OnFocusedInputReceived;
                continue;
            }

            action.Disable();
        }
        mIsFocusedOn = true;
    }

    private void unFocuseInput(eTutorialState tutoiralState)
    {
        TimeScaleUtil.Instance.RemoveTimeScale("TutoiralFocusInput");
        InputActionMap actionMap = sfInputAsset.FindActionMap(eInputSections.BattleGamePlay.ToString());
        eBattleInputName waitInput = convertTutorialStateToBattleInput(tutoiralState);
        actionMap.FindAction(waitInput.ToString()).started -= OnFocusedInputReceived;
        mLockedInput.Remove(waitInput);
        updateTutorialInput();
        sfTutoiralUI.Disable();
        mIsFocusedOn = false;

        if(mBufferedStepCount != 0)
        {
            MoveToNextTutorialStep();
            mBufferedStepCount--;
        }
    }

    private void updateTutorialInput()
    {
        InputActionMap actionMap = sfInputAsset.FindActionMap(eInputSections.BattleGamePlay.ToString());
        string[] battleInputString = System.Enum.GetNames(typeof(eBattleInputName));
        for (int i = 0; i < battleInputString.Length; i++)
        {
            bool isFounded = false;
            InputAction action = actionMap.FindAction(battleInputString[i]);
            for (int j = 0; j < mLockedInput.Count; j++)
            {
                if (battleInputString[i] == mLockedInput[j].ToString())
                {
                    isFounded = true;
                    action.Disable();
                    break;
                }
            }

            if(!isFounded)
            {
                action.Enable();
            }
        }
    }

    private eBattleInputName convertTutorialStateToBattleInput(eTutorialState tutorialState)
    {
        eBattleInputName waitInput = default;

        switch (tutorialState)
        {
            case eTutorialState.NormalAttack:
                waitInput = eBattleInputName.NormalAttack;
                break;
            case eTutorialState.SwapAttack:
                waitInput = eBattleInputName.SwitchPlayerToNext;
                break;
            case eTutorialState.UltimateAttack:
                waitInput = eBattleInputName.UltimateSkill;
                break;
            default:
                break;
        }

        return waitInput;
    }
}


