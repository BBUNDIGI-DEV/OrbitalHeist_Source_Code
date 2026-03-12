using System.Collections;

namespace UnityEngine.UI
{
    public class MissionUITrigger : MonoBehaviour
    {
        [SerializeField] private eCharacterName sfSpeaker;
        [SerializeField] private string sfContents;
        [SerializeField] private float sfDuration;


        private void OnTriggerEnter(Collider other)
        {
            GetComponent<Collider>().enabled = false;
            ManualTrigger();
        }

        public void ManualTrigger()
        {
            UIManager.Instance.SFMissionMessageUI.MissionMessageShowUP(sfSpeaker);
            UIManager.Instance.SFMissionMessageUI.SetText(sfContents);
            if (sfDuration != 0.0f)
            {
                StartCoroutine(hideRoutine(sfDuration));
            }
        }

        public void HideInfoMessage()
        {
            UIManager.Instance.SFMissionMessageUI.MissionMessageCloseDown();
        }

        private IEnumerator hideRoutine(float duration)
        {
            yield return new WaitForSeconds(duration);
            UIManager.Instance.SFMissionMessageUI.MissionMessageCloseDown();
        }
    }

}
