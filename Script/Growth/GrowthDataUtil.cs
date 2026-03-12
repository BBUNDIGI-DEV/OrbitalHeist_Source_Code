using System.Collections.Generic;
using UnityEngine;

public static class GrowthDataUtil
{
    public static GrowthAbilityData[] AllDatas;
    public static Sprite[] Icons;

    private static AbilityRankChacneData mDefaultTearChanceWeight;
    private static Dictionary<eAbilGrade, List<GrowthAbilityData>> mRandomPicker;

    public static void LoadDatas()
    {
        mRandomPicker = new Dictionary<eAbilGrade, List<GrowthAbilityData>>();

        for (eAbilGrade i = 0; i < eAbilGrade.Count; i++)
        {
            mRandomPicker.Add(i, new List<GrowthAbilityData>());
        }
        AllDatas = Resources.LoadAll<GrowthAbilityData>("GrowthData");
        Icons = Resources.LoadAll<Sprite>("GrowthData/Icons");
        mDefaultTearChanceWeight = Resources.Load<AbilityRankChacneData>("GrowthData/RankChacneData/DefaultChance");
    }

    public static GrowthAbilityData[] GetRandomDataList(int count, List<GrowthAbilityData> ownedAbilList, AbilityRankChacneData chanceOrNull = null)
    {
        #region Obsolute
        foreach (KeyValuePair<eAbilGrade, List<GrowthAbilityData>> gradeListPair in mRandomPicker)
        {
            gradeListPair.Value.Clear();
        }

        for (int i = 0; i < AllDatas.Length; i++)
        {
            if (!CheckMoreOwnable(AllDatas[i], ownedAbilList))
            {
                continue;
            }

            mRandomPicker[AllDatas[i].AbilGradeEnum].Add(AllDatas[i]);
        }

        GrowthAbilityData[] pickableData = new GrowthAbilityData[AllDatas.Length - ownedAbilList.Count];
        int pickableDataIndex = 0;
        for (int i = 0; i < AllDatas.Length; i++)
        {
            if(CheckMoreOwnable(AllDatas[i], ownedAbilList))
            {
                pickableData[pickableDataIndex] = AllDatas[i];
                pickableDataIndex++;
            }
        }
        #endregion

        return pickableData;
    }

    public static bool CheckMoreOwnable(GrowthAbilityData data, List<GrowthAbilityData> ownedAbilList)
    {
        int count = 0;
        for (int i = 0; i < ownedAbilList.Count; i++)
        {
            if (ownedAbilList[i] == data)
            {
                count++;
            }
        }

        return count < data.OwnLimit;
    }
}
