using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDamagable
{
    public void OnHit(DamageInfo damageInfo);

}

public struct DamageInfo  
{
    public float Damage;
    public HitEffectData HitData;
    public CharacterBase AttackerOrNull;
    public eBuffNameID DamagedByBuff;

    public DamageInfo(float damage, HitEffectData hitData, CharacterBase attackerOrNull)
    {
        Damage = damage;
        HitData = hitData;
        AttackerOrNull = attackerOrNull;
        DamagedByBuff = eBuffNameID.None;
    }

    public DamageInfo(float damage, HitEffectData hitData, eBuffNameID buff)
    {
        Damage = damage;
        HitData = hitData;
        AttackerOrNull = null;
        DamagedByBuff = buff;
    }
}