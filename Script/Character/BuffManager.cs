using System.Collections.Generic;

public class BuffManager
{
    private readonly CharacterBase mOwnerBase;

    public List<BuffElement> OwnedBuffList
    {
        get; private set;
    }

    public BuffManager(CharacterBase character)
    {
        mOwnerBase = character;
        OwnedBuffList = new List<BuffElement>();
    }

    public void UpdateBuff(float deltaTime)
    {
        for (int i = 0; i < OwnedBuffList.Count; i++)
        {
            OwnedBuffList[i].UpdateBuff(deltaTime);
        }

        for (int i = 0; i < OwnedBuffList.Count; i++)
        {
            if(!OwnedBuffList[i].IsActivated)
            {
                OwnedBuffList.RemoveAt(i);
                i--;
            }
        }
    }

    public bool CheckBuffActivated(eBuffNameID buffNameID)
    {
        for (int i = 0; i < OwnedBuffList.Count; i++)
        {
            if(OwnedBuffList[i].BuffDataConfig.BuffNameIDEnum == buffNameID)
            {
                return true;
            }
        }

        return false;
    }

    public void AddBuff(eBuffNameID data, float duration, params float[] powers)
    {
        AddBuff(RuntimeDataLoader.AllBuffData[(int)data], false, duration, powers);
    }

    public void AddBuffWithSkipFX(eBuffNameID data, float duration, params float[] powers)
    {
        AddBuff(RuntimeDataLoader.AllBuffData[(int)data], true, duration, powers);
    }

    public void AddBuff(BuffData data, float duration, int count, params float[] powers)
    {
        for (int i = 0; i < count; i++)
        {
            AddBuff(data, false, duration, powers);
        }
    }

    public void AddBuff(BuffData data, bool skipFX, float duration, int count, params float[] powers)
    {
        for (int i = 0; i < count; i++)
        {
            AddBuff(data, skipFX, duration, powers);
        }
    }

    public void AddBuff(BuffData data, bool skipFX, float duration,  params float[] powers)
    {
        BuffElement buffAreadyIn = checkBuffAreadyIn(data.BuffNameIDEnum);
        if (buffAreadyIn != null)
        {
            buffAreadyIn.UpgradeStack(duration, skipFX);
        }
        else
        {
            BuffElement buffElement = new BuffElement(data, mOwnerBase, skipFX, duration, powers);
            OwnedBuffList.Add(buffElement);
        }
    }

    public void ClearAllBuff()
    {
        for (int i = 0; i < OwnedBuffList.Count; i++)
        {
            OwnedBuffList[i].RemoveBuffEffect();
        }
    }

    private BuffElement checkBuffAreadyIn(eBuffNameID buffNameID)
    {
        for (int i = 0; i < OwnedBuffList.Count; i++)
        {
            if(OwnedBuffList[i].BuffDataConfig.BuffNameIDEnum == buffNameID)
            {
                return OwnedBuffList[i];
            }
        }
        return null;
    }
}

public class BuffElement
{
    public readonly BuffData BuffDataConfig;

    public bool IsActivated
    {
        get; private set;
    }
    public int StackCount
    {
        get; private set;
    }

    private readonly CharacterBase mOwnerCharacter;
    private float mEndDuration;
    private float mProcessingDuration;
    private int mTickCount;
    private float mPower1;
    private float mPower2;
    private float mPower3;
    private ParticleBinder mSpawnedAuraFX;

    public BuffElement(BuffData buff, CharacterBase owner, bool skipFX, float endDuration, params float[] powers)
    {
        BuffDataConfig = buff;
        mOwnerCharacter = owner;
        mEndDuration = endDuration;
        mProcessingDuration = 0.0f;
        mTickCount = 0;
        StackCount = 0;
        IsActivated = true;
        for (int i = 0; i < powers.Length; i++)
        {
            switch (i)
            {
                case 0:
                    mPower1 = powers[i];
                    break;
                case 1:
                    mPower2 = powers[i];
                    break;
                case 2:
                    mPower3 = powers[i];
                    break;
            }
        }
        addBuffEffect(skipFX);
    }

    public void UpdateBuff(float deltaTime)
    {
        mProcessingDuration += deltaTime;
        if (mProcessingDuration >= mEndDuration)
        {
            RemoveBuffEffect();
            IsActivated = false;
        }

        switch (BuffDataConfig.BuffNameIDEnum)
        {
            case eBuffNameID.Fire:
                int tickCount = (int)(mProcessingDuration / BuffDataConfig.TickGap);
                if (mTickCount != tickCount)
                {
                    float damage = mPower1 + (BuffDataConfig.AdditionalPowerPerStack * mPower1 * StackCount);
                    damage *= BuffDataConfig.TickGap;
                    mOwnerCharacter.CharacterStatus.DecreaseHPWithLeastHP(damage, 2);
                    if(mOwnerCharacter.CharacterStatus.CurrentHP > 2)
                    {
                        mOwnerCharacter.CharacterStatus.LastDamageInfo.Value = new DamageInfo(damage, HitEffectData.GetDebuffDamageHitEffectData(), eBuffNameID.Fire);
                    }
                    mTickCount = tickCount;
                }
                break;

            case eBuffNameID.HealingUp:
                tickCount = (int)(mProcessingDuration / BuffDataConfig.TickGap);
                if (mTickCount != tickCount)
                {
                    float healing = mPower1 + BuffDataConfig.AdditionalPowerPerStack * StackCount;
                    healing *= BuffDataConfig.TickGap;
                    mOwnerCharacter.CharacterStatus.AddHP(healing);
                    mTickCount = tickCount;
                }
                break;
        }
    }

    public void UpgradeStack(float newDuration, bool skipFX)
    {
        mEndDuration = newDuration;
        if (StackCount + 1 >= BuffDataConfig.MaxStack)
        {
            if (BuffDataConfig.BuffTypeEnum == eBuffType.Buff)
            {
                mEndDuration += (mEndDuration - mProcessingDuration) * 0.5f;
            }

            mProcessingDuration = 0.0f;
            return;
        }
        StackCount++;
        mEndDuration += mEndDuration * StackCount * BuffDataConfig.AdditionalDurationPerStack;
        mProcessingDuration = 0.0f;
        mTickCount = 0;
        addBuffEffect(skipFX);
    }

    public void RemoveBuffEffect()
    {
        float commonDecreaseAmount = mPower1 + StackCount * BuffDataConfig.AdditionalPowerPerStack * mPower1;
        switch (BuffDataConfig.BuffNameIDEnum)
        {
            case eBuffNameID.SlowDown:
                mOwnerCharacter.CharacterStatus.ExtraSpeed += commonDecreaseAmount;
                break;
            case eBuffNameID.PowerUp:
                mOwnerCharacter.CharacterStatus.Defense -= commonDecreaseAmount;
                break;
            case eBuffNameID.DefenseUp:
                mOwnerCharacter.CharacterStatus.Defense -= commonDecreaseAmount;
                break;
            case eBuffNameID.PowerOverwalming:
                mOwnerCharacter.CharacterStatus.IsPowerOverwalming.DisEnrollValue(CharacterStatus.ePowerOverwalmingSetter.SetByBuff);
                break;
            case eBuffNameID.Fire:
                mOwnerCharacter.CharacterStatus.Defense += mPower2;
                mOwnerCharacter.Anim.SetBurningFactor(0);
                break;
            case eBuffNameID.AttackSpeedUp:
                mOwnerCharacter.CharacterStatus.AttackSpeedMultiplier -= commonDecreaseAmount;
                break;
        }

        if (mSpawnedAuraFX != null)
        {
            mSpawnedAuraFX.StopFXImmediate();
        }
    }


    private void addBuffEffect(bool skipFX)
    {
        float commonIncreaseAmount = mPower1;
        if (StackCount != 0)
        {
            commonIncreaseAmount = BuffDataConfig.AdditionalPowerPerStack * mPower1;
        }

        switch (BuffDataConfig.BuffNameIDEnum)
        {
            case eBuffNameID.SlowDown:
                mOwnerCharacter.CharacterStatus.ExtraSpeed -= commonIncreaseAmount;
                break;
            case eBuffNameID.PowerUp:
                mOwnerCharacter.CharacterStatus.Power += commonIncreaseAmount;
                break;
            case eBuffNameID.DefenseUp:
                mOwnerCharacter.CharacterStatus.Defense += commonIncreaseAmount;
                break;
            case eBuffNameID.HealingUp:
                break;
            case eBuffNameID.UltimateGuageUp:
                PlayerCharacterController pc = mOwnerCharacter as PlayerCharacterController;
                float increaseAmount = pc.PlayerStatus.UltimateGuage.Value.Max * mPower1;
                pc.PlayerStatus.UltimateGuage.Value =
                     pc.PlayerStatus.UltimateGuage.Value.AddGuage(increaseAmount);
                break;
            case eBuffNameID.Fire:
                mOwnerCharacter.CharacterStatus.Defense -= mPower2;
                mOwnerCharacter.Anim.SetBurningFactor((StackCount + 1) / (float)BuffDataConfig.MaxStack);
                break;
            case eBuffNameID.PowerOverwalming:
                mOwnerCharacter.CharacterStatus.IsPowerOverwalming.EnrollValue(CharacterStatus.ePowerOverwalmingSetter.SetByBuff, true);
                break;
            case eBuffNameID.AttackSpeedUp:
                mOwnerCharacter.CharacterStatus.AttackSpeedMultiplier += commonIncreaseAmount;
                break;
            case eBuffNameID.InstanceHealing:
                mOwnerCharacter.CharacterStatus.AddHP(mPower1);
                break;
        }

        bool isForPlayer;
        if (mOwnerCharacter is PlayerCharacterController)
        {
            isForPlayer = true;
        }
        else
        {
            isForPlayer = false;
        }


        if (BuffDataConfig.AuraFX != null)
        {
            GameObjectPool fxPool;

            if (isForPlayer)
            {
                fxPool = PlayerManager.Instance.GlobalFXPool;
            }
            else
            {
                fxPool = mOwnerCharacter.FXPool;
            }

            if (!fxPool.TryCheckActivatedObjectExist(BuffDataConfig.AuraFX))
            {
                mSpawnedAuraFX = fxPool.GetGameobject(BuffDataConfig.AuraFX);
                if(isForPlayer)
                {
                    mSpawnedAuraFX.SetGlobalActivatedPlayerFX();
                }
                else
                {
                    mSpawnedAuraFX.SetFXTransformType(eFXTransformType.PositionOnlyWorld, mOwnerCharacter.Translator.Trans);
                }
            }
        }

        if(skipFX)
        {
            return;
        }

        setFloatingInfoMessageTrigger(BuffDataConfig.BuffNameIDEnum);

        if (BuffDataConfig.InstanceFX != null)
        {
            ParticleBinder spawnedFX = PlayerManager.Instance.GlobalFXPool.GetGameobject(BuffDataConfig.InstanceFX);
            spawnedFX.SetGlobalActivatedPlayerFX();
        }
    }

    private void setFloatingInfoMessageTrigger(eBuffNameID nameID)
    {
        eFloatingInfoMessageTag tag = eFloatingInfoMessageTag.GetPowerUpBuff;
        bool isFounded = true;
        switch (nameID)
        {
            case eBuffNameID.PowerUp:
                tag = eFloatingInfoMessageTag.GetPowerUpBuff;
                break;
            case eBuffNameID.Fire:
                tag = eFloatingInfoMessageTag.GetShivFire;
                break;
            case eBuffNameID.PowerOverwalming:
                tag = eFloatingInfoMessageTag.GetPowerOverwalming;
                break;
            case eBuffNameID.AttackSpeedUp:
                tag = eFloatingInfoMessageTag.GetAttackSpeedBuff;
                break;
            default:
                isFounded = false;
                break;
        }

        if(!isFounded)
        {
            return;
        }

        if (mOwnerCharacter is MonsterBase monsterCharacter)
        {
            monsterCharacter.MonsterStatus.FloatingInfomessageTrigger.Value = tag;
        }
        else
        {
            PlayerManager.Instance.FloatingInfomessageTrigger.Value = tag;
        }
    }

}
