
namespace UnityEngine.UI
{
    public class UIManager : SingletonClass<UIManager>
    {
        [field: SerializeField]
        public DialogueUIManager SFDialogueManager
        {
            get; private set;
        }

        [field: SerializeField]
        public GameObject SFGameOverUI
        {
            get; private set;
        }

        [field: SerializeField]
        public GameObject SFEndingCreditUI
        {
            get; private set;
        }

        [field: SerializeField]
        public GameObject SFTutorialIamge
        {
            get; private set;
        }

        [field: SerializeField]
        public GrowthUI SFGrowthUI
        {
            get; private set;
        }

        [field: SerializeField]
        public GameObject SFGameEnd
        {
            get; private set;
        }

        [field: SerializeField]
        public MissionMessageUI SFMissionMessageUI
        {
            get; private set;
        }

        public Camera UICamera
        {
            get
            {
                return CameraManager.Instance.UICamera;
            }
        }


        protected override void Awake()
        {
            base.Awake();
        }
    }
}