using UnityEngine.EventSystems;

namespace UnityEngine.UI
{
    public class FireDebuffUIFXActivator : MonoBehaviour
    {
        [SerializeField] private GameObject sfFireFX;
        private MonsterBase mMonsterBase;
        private void Start()
        {
            mMonsterBase = GetComponentInParent<MonsterBase>();
        }

        private void Update()
        {
            if(mMonsterBase.BuffHandler.CheckBuffActivated(eBuffNameID.Fire))
            {
                if(!sfFireFX.activeInHierarchy)
                {
                    sfFireFX.SetActive(true);
                }
            }
            else
            {
                if(sfFireFX.activeInHierarchy)
                {
                    sfFireFX.SetActive(false);
                }
            }
        }
    }
}