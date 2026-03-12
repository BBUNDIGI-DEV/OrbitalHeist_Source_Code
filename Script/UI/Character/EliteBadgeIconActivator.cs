using UnityEngine.EventSystems;

namespace UnityEngine.UI
{
    public class EliteBadgeIconActivator : MonoBehaviour
    {
        private void Start()
        {
            MonsterBase monsterBase = GetComponentInParent<MonsterBase>();
            if (monsterBase == null)
            {
                gameObject.SetActive(false);
                return;
            }

            if(monsterBase.MonsterConfig.MonsterName.ToString().Contains("Elite"))
            {
                return;
            }

            gameObject.SetActive(false);
        }

    }
}