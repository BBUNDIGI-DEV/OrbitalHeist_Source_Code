using UnityEngine;

public class InteractableGrowthItem : InteractableBase
{
    [SerializeField] private AbilityRankChacneData sfAbilChance;

    public override void InvokeInteraction()
    {
        base.InvokeInteraction();
        GrowthManager.Instance.StartGrowthSelection(DisableInteractable); 
    }

    public override void DisableInteractable()
    {
        base.DisableInteractable();
        gameObject.SetActive(false);
    }
}

