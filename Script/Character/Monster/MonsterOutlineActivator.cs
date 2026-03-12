using UnityEngine;


[RequireComponent(typeof(OutlineController))]
public class MonsterOutlineActivator : MonoBehaviour
{
    private OutlineController mOutlienController;
    private void Awake()
    {
        mOutlienController = GetComponent<OutlineController>();
    }

    private void Start()
    {
        MonsterStatus monsterStatus = GetComponentInParent<MonsterBase>(true).MonsterStatus;
        monsterStatus.ShieldAmount.AddListener(UpdateOutline, true);
    }

    public void UpdateOutline(Gauge shieldAmount)
    {
        if(shieldAmount.Current > 0.0f)
        {
            mOutlienController.outlineWidth = RuntimeDataLoader.OutlineConfig.SheildOutlineSnapshot.OutlineWidth;
            mOutlienController.outlineColor = RuntimeDataLoader.OutlineConfig.SheildOutlineSnapshot.OutlineColor;
        }
        else
        {
            mOutlienController.outlineWidth = 0.0f;
        }
    }
}
