using UnityEngine;
using UnityEngine.InputSystem;

[DefaultExecutionOrder(-55)]
public class InputManager : SingletonClass<InputManager>
{
    public Vector3 MoveDir
    { get; private set; }
    public eInputSections CurrentInputSection
    { get; private set; }
    public InputHandlerBase LoadedInputHandler
    {
        get; private set;
    }

    public bool IsInputEnabled
    {
        get
        {
            return mIsInputEnabled;
        }
        set
        {
            mIsInputEnabled = value;
            LoadedInputHandler.SetEnabled(mIsInputEnabled);
        }
    }
    private bool mIsInputEnabled;


    protected override void Awake()
    {
        base.Awake();
        InputHandlerBase[] inputHandlers = GetComponentsInChildren<InputHandlerBase>(true);

        for (int i = 0; i < inputHandlers.Length; i++)
        {
            if(LoadedInputHandler == null && inputHandlers[i].CheckPlatform(Application.platform))
            {
                LoadedInputHandler = inputHandlers[i];
            }
            else
            {
                Destroy(inputHandlers[i].gameObject);
            }
        }
        Debug.Assert(LoadedInputHandler != null, "Input Handler not founded");
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        IsInputEnabled = true;
    }

    public void Update()
    {
        if(CurrentInputSection == eInputSections.BattleGamePlay)
        {
            MoveDir = LoadedInputHandler.GetMoveDir();
        }
        else
        {
            MoveDir = Vector3.zero;
        }
    }

    public void SwitchInputSection(eInputSections newMap)
    {
        if(CurrentInputSection == newMap)
        {
            return;
        }
        CurrentInputSection = newMap;
        LoadedInputHandler.SwitchActionMap(newMap);
    }

    public void AddInputCallback(eInputSections map, string action, System.Action<InputAction.CallbackContext> callback, InputActionPhase phase = InputActionPhase.Started)
    {
        LoadedInputHandler.AddInputCallback(map, action, callback, phase);
    }

    public void RemoveInputCallback(eInputSections map, string action, System.Action<InputAction.CallbackContext> callback, InputActionPhase phase = InputActionPhase.Started)
    {
        LoadedInputHandler.RemoveInputCallback(map, action, callback, phase);
    }

    public Vector3 GetAttackAim(Transform playerTrans)
    {
        return LoadedInputHandler.GetAttackAim(playerTrans);
    }
}

public enum eBattleInputName
{
    Move,
    Dash,
    NormalAttack,
    Interactive,
    PauseGame,
    SpeicalAttack,
    CheatDamage,
    SwitchPlayerTo1,
    SwitchPlayerTo2,
    SwitchPlayerTo3,
    SwitchPlayerToNext,
    UltimateSkill,
    TryCounter,
    Cheat1,
    Cheat2,
    Cheat3,
    Cheat4,
    ZoomInZoomUp,
    Cheat5,
    Cheat6,
    Cheat7,
    Cheat8,
    Cheat9,
}

public enum eCutSceneInputName
{
    CutSceneInteraction,
    Skip,
}

public enum eDialougeInputName
{
    DialogueInteraction,
    Skip
}

public enum eMenuInputName
{

}

public enum eUIInputName
{
    PauseGame,
}

public enum eInputDeviceType
{
    KeyboardAndMouse,
    GamePad,
    Mobile,
}

public enum eInputSections
{
    None = -1,
    BattleGamePlay,
    CutScene,
    Dialouge,
    Menu,
    UI,
    Count,
}