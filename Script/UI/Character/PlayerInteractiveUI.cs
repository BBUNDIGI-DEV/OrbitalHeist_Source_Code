using TMPro;

namespace UnityEngine.UI
{
    public class PlayerInteractiveUI : MonoBehaviour
    {
        private const string SHOW_ANIM_KEY = "ANI_interaction_info_show_up";
        private const string CLOSE_ANIM_KEY = "ANI_interaction_info_close";
        [SerializeField] private Animation sfAnim;
        [SerializeField] private TMP_Text sfInfoText;
        [SerializeField] private TMP_Text sfInputInfoText;

        private void Start()
        {
            PlayerManager.Instance.GlobalPlayerStatus.DetectedInteractableObject.AddListener(updateInteractableObjectType);
        }

        private void OnDestroy()
        {
            if (PlayerManager.IsExist)
            {
                PlayerManager.Instance.GlobalPlayerStatus.DetectedInteractableObject.RemoveListener(updateInteractableObjectType);
            }
        }

        private void updateInteractableObjectType(eInteractableType newInteractableObjectType)
        {
            if (newInteractableObjectType == eInteractableType.None)
            {
                sfAnim.PlayQueued(CLOSE_ANIM_KEY);
            }
            else
            {
                sfAnim.PlayQueued(SHOW_ANIM_KEY);
                switch (newInteractableObjectType)
                {
                    case eInteractableType.None:
                        break;
                    case eInteractableType.DialogueNPC:
                        sfInfoText.text = "대화하기";
                        break;
                    case eInteractableType.CommonObject:
                        sfInfoText.text = "상호작용";
                        break;
                    case eInteractableType.EndingObject:
                        sfInfoText.text = "살펴보기";
                        break;
                    case eInteractableType.GrowthItem:
                        sfInfoText.text = "강화하기";
                        break;
                    case eInteractableType.Stage1Door:
                        sfInfoText.text = "파괴하기";
                        break;
                    case eInteractableType.Stage2Door:
                        sfInfoText.text = "들어가기";
                        break;
                    case eInteractableType.Stage3BossBattle:
                        sfInfoText.text = "입장하기";
                        break;
                    default:
                        Debug.LogError(newInteractableObjectType);
                        break;
                }

            }
        }
    }
}
