using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MobileInputUI : MonoBehaviour
{
    public class MobileActionDic
    {
        public MobileButton this[string Tag]
        {
            get
            {
                return mActionDic[Tag];
            }
        }

        private Dictionary<string, MobileButton> mActionDic;

        public void InitializeActionDic<T>(MobileButton[] buttons) where T : Enum
        {
            mActionDic = new Dictionary<string, MobileButton>();
            for (int i = 0; i < buttons.Length; i++)
            {
                Debug.Assert(!mActionDic.ContainsKey(buttons[i].Usage),
                    "duplicated button usage detected" +
                    $"buttons name: [{buttons[i].gameObject.name}]");
                mActionDic.Add(buttons[i].Usage, buttons[i]);
            }
        }

        public void ClearActions()
        {
            foreach (var item in mActionDic)
            {
                item.Value.ClearCallback();
            }
        }
    }

    public eInputSections InputSection
    {
        get
        {
            return sfInputSection;
        }
    }

    public MobileActionDic ActionDics
    {
        get; private set;
    }

    public Vector2 JoystickPos
    {
        get
        {
            return sfMoveJoystick.DeltaStickPos;
        }
    }

    [SerializeField] private eInputSections sfInputSection;
    [SerializeField] private MobileJoystick sfMoveJoystick;


    public void Awake()
    {
        MobileButton[] buttons = GetComponentsInChildren<MobileButton>();
        ActionDics = new MobileActionDic();
        switch (sfInputSection)
        {
            case eInputSections.BattleGamePlay:
                ActionDics.InitializeActionDic<eBattleInputName>(buttons);
                break;
            case eInputSections.CutScene:
                ActionDics.InitializeActionDic<eCutSceneInputName>(buttons);
                break;
            case eInputSections.Dialouge:
                ActionDics.InitializeActionDic<eDialougeInputName>(buttons);
                break;
            case eInputSections.Menu:
                ActionDics.InitializeActionDic<eMenuInputName>(buttons);
                break;
            case eInputSections.UI:
                break;
            default:
                Debug.LogError($"default switch detected [{sfInputSection}]");
                break;
        }
        gameObject.SetActive(false);
    }
}
