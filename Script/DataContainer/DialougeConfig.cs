using UnityEngine;
using UnityEngine.UI;
using Sirenix.OdinInspector;

[CreateAssetMenu(menuName = "DataContainer/DialogueConfig")]
public class DialogueConfig : ExcelBasedSO
{
    public eDialougeDir Dir;
    public eDialogueCharacterType LeftCharacter;
    public eDialogueCharacterType RightCharacter;
    public string Contents;
    public string Name;
    public eGestureType GestureType;
    public eEmojiType EmojiType;
    public eEmotionType EmotionType;

    public override void AutoUpdate()
    {

    }
}

public enum eDialogueCharacterType
{
    None = -1,
    Glanda,
    Shiv,
    Hypo,
    Blin,
    Moab,
    MoabSilhouette,
}

public enum eDialougeDir
{
    None = -1,
    Left,
    Right,
}

public enum eDialogueTag
{
    Stage1_01_ShivEntrance,
    Stage1_02_CrachGate,
    Stage2_01_HypoEntrance,
    Stage2_02_Speaker,
    Stage2_03_BlinEntrance,
    Stage2_04_BlinRescue,
    Stage3_01_MoabEntrance,
    Stage3_02_MoabBoss,
    Count,
}

public enum eDefaultActing
{
    Show,
    Close,
    Disable,
    Enable,
    Swap,
}

public enum eEmotionType
{
    None,
    Angry,
    Flustred,
    Happy,
    Serious,
    Surprised,
    Pleased,
    Yawn,
}

public enum eEmojiType
{
    None,
    Bang,
    Question,
    Note,
    Surprised
}

public enum eGestureType
{
    None,
    Shock,
    ShakeVert,
}

public enum eDialogueCharacterAnimLayer
{
    None = -1,
    Default,
    Gesture,
    Emoji,
    EnabledOrDisabled
}


