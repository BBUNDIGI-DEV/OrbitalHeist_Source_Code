using UnityEngine;

[CreateAssetMenu(menuName = "DataContainer/GlobalMonsterConfig")]
public class GlobalMonsterConfig : ScriptableObject
{
    public MonsterBase[] FullMonsterArray;
    public eMonsterName[] NormalMonsters;
    public eMonsterName[] EliteMonsters;

    public MonsterBase GetMonsterByName(eMonsterName name)
    {
        for (int i = 0; i < FullMonsterArray.Length; i++)
        {
            if (FullMonsterArray[i].MonsterConfig.MonsterName == name)
            {
                return FullMonsterArray[i];
            }
        }

        Debug.LogError($"Monster not founded, {name}", this);
        return null;
    }
}

