using UnityEngine;
using UnityEngine.Events;

public class StageManager : SingletonClass<StageManager>
{
    [SerializeField] private UnityEvent sfOnStageStart;
    private SectionElement[] mSections;
    private Transform mPlayerEntrance;

    protected override void Awake()
    {
        base.Awake();
        mSections = GetComponentsInChildren<SectionElement>();
        mPlayerEntrance = GameObject.FindGameObjectWithTag("PlayerEntrance").transform;
    }

    private void Start()
    {
        if(mPlayerEntrance != null)
        {
            PlayerManager.Instance.InitializeOnNewStage(mPlayerEntrance.position);
        }
        PlayerManager.Instance.ReviveAllCharacter(1000);
        InputManager.Instance.SwitchInputSection(eInputSections.BattleGamePlay);
        sfOnStageStart?.Invoke();
    }


    private void Update()
    {
        bool isAllCleared = true;
        for (int i = 0; i < mSections.Length; i++)
        {
            if (!mSections[i].IsCleared)
            {
                isAllCleared = false;
                break;
            }
        }


        if(isAllCleared)
        {
        }
    }
}
