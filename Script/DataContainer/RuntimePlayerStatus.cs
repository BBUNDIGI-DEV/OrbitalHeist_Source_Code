using UnityEngine;
using Sirenix.OdinInspector;

public class RuntimePlayerStatus : CharacterStatus
{
    public ObservedData<Gauge> UltimateGuage;

    public float UltimateGuageAutoIncreasement;
    public float UltimateGuageIncreasementPerDamage;
    public float UltimateGuageIncreasementOnHurt;

    public InteractableTypeObservedData DetectedInteractableObject;
    [System.NonSerialized] public eInteractableType ProcessedInteractableObject;

    public RuntimePlayerStatus(PlayerConfig config) : base()
    {
        UltimateGuage.Value = new Gauge(0.0f, config.MaxUltimageGuage);

        MaxHP.Value = config.InitialMaxHP;
        CurrentHP.Value = MaxHP;
        Power = config.BaseDamage;
        Defense = config.BaseDefense;
        UltimateGuageAutoIncreasement = config.UltimateGuageAutoIncreasement;
        UltimateGuageIncreasementPerDamage = config.UltimateGuageIncreasementPerDamage;
    }

    public void IncreaseUltimateGuage(float increaseAmount)
    {
        UltimateGuage.Value = UltimateGuage.Value.AddGuage(increaseAmount);
    }

    public void SetUltimateGuageZero()
    {
        UltimateGuage.Value = new Gauge(0.0f, UltimateGuage);
    }

    public void DecreaseHP(float dealAmount, out bool isForceShiledDecreased)
    {
        isForceShiledDecreased = false;
        dealAmount = Mathf.Clamp(dealAmount, 2.0f, dealAmount);
        dealAmount = Mathf.Max(dealAmount, 2.0f);

        if (PlayerManager.Instance.GlobalPlayerStatus.ForceShieldAmount > 0)
        {
            PlayerManager.Instance.GlobalPlayerStatus.ForceShieldAmount.Value--;
            isForceShiledDecreased = true;
        }
        else
        {
            dealAmount = Mathf.Clamp(dealAmount, 0.0f, CurrentHP);
            CurrentHP.Value -= dealAmount;
        }
    }
}

