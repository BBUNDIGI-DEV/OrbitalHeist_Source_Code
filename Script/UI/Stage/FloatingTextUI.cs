
namespace UnityEngine.UI
{
    public class FloatingUI : MonoBehaviour
    {
        private static FloatingInfoMessageConfig[] smInfoMessageConfig;

        [SerializeField] private bool sfIsForPlayer;
        [SerializeField] private FloatingTextUIElement sfFloatingUIPrefab;
        private GameObjectPool mFloatingInfoMessageTool;

        private void Awake()
        {
            if(smInfoMessageConfig == null)
            {
                smInfoMessageConfig = Resources.LoadAll<FloatingInfoMessageConfig>("InfoMessageData");
            }

            mFloatingInfoMessageTool = GetComponent<GameObjectPool>();
        }

        private void Start()
        {
            if (sfIsForPlayer)
            {
                PlayerManager.Instance.FloatingInfomessageTrigger.AddListener(ShowInfoMessage);
            }
            else
            {
                MonsterBase monsterBase = GetComponentInParent<MonsterBase>();
                monsterBase.MonsterStatus.FloatingInfomessageTrigger.AddListener(ShowInfoMessage);
            }
        }


        public void ShowInfoMessage(eFloatingInfoMessageTag tag)
        {
            FloatingTextUIElement floatingUI = mFloatingInfoMessageTool.GetGameobject(sfFloatingUIPrefab);

            floatingUI.SetText(smInfoMessageConfig[(int)tag]);
        }
    }
}
