using UnityEngine;

public abstract class CharacterStatus
{
    public enum ePowerOverwalmingSetter
    {
        SetBySkill,
        SetByDash,
        SetByBuff,
    }

    public SerializableObesrvedData<float> MaxHP;
    public SerializableObesrvedData<float> CurrentHP;
    public SerializableObesrvedData<float> NormalizedHP;
    public SerializableObesrvedData<bool> IsDead;
    public ObservedData<DamageInfo> LastDamageInfo;
    public LayeredValue<ePowerOverwalmingSetter, bool> IsPowerOverwalming;

    public float Defense
    {
        get
        {
            return mDefense;
        }
        set
        {
            mDefense = value;
            mDefense = Mathf.Clamp(mDefense, 0, mDefense);
        }
    }

    public float Power
    {
        get
        {
            return mPower;
        }
        set
        {
            mPower = value;
            mPower = Mathf.Clamp(mPower, 0, mPower);
        }
    }

    public float ExtraSpeed
    {
        get
        {
            return mExtraSpeed;
        }
        set
        {
            mExtraSpeed = value;
        }
    }

    public float SpeedMultiplier;
    public float AttackSpeedMultiplier;

    private float mPower;
    private float mDefense;
    private float mExtraSpeed;

    public CharacterStatus()
    {
        MaxHP = new SerializableObesrvedData<float>();
        CurrentHP = new SerializableObesrvedData<float>();
        NormalizedHP = new SerializableObesrvedData<float>();
        IsDead = new SerializableObesrvedData<bool>();

        CurrentHP.AddListener(() => NormalizedHP.Value = CurrentHP / MaxHP);
        MaxHP.AddListener(updateMaxHP);
        IsPowerOverwalming = new LayeredValue<ePowerOverwalmingSetter, bool>(false);
    }

    public virtual void AddHP(float amount)
    {
        float newHP = CurrentHP + amount;
        newHP = Mathf.Clamp(newHP, 0, MaxHP);
        CurrentHP.Value = newHP;
    }

    public virtual void DecreaseHPWithLeastHP(float amount, float leastHP)
    {
        if(CurrentHP < leastHP)
        {
            AddHP(0);
        }
        else
        {
            float newHP = CurrentHP - amount;
            newHP = Mathf.Clamp(newHP, leastHP, MaxHP);

            float deltaDamage = CurrentHP - newHP;
            AddHP(-deltaDamage);
        }
    }

    public virtual void DecreaseMaxHPWithLeastHP(float amount, float leastHP)
    {
        if (MaxHP < leastHP)
        {
            return;
        }
        else
        {
            float newMaxHP = MaxHP - amount;
            newMaxHP = Mathf.Clamp(newMaxHP, leastHP, MaxHP);

            float deltaDamage = MaxHP - newMaxHP;
            AddHP(-deltaDamage);
        }
    }

    private void updateMaxHP(float prevMaxHP, float newMaxHP)
    {
        if(newMaxHP > prevMaxHP)
        {
            float deltaMaxHp = newMaxHP - prevMaxHP;
            AddHP(deltaMaxHp);
        }
        NormalizedHP.Value = CurrentHP / newMaxHP;
    }

}
