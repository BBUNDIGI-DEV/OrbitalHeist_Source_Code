using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;

[CreateAssetMenu(menuName = "DataContainer/AbilWeightByGradeData")]
public class AbilityRankChacneData : ExcelBasedSO
{
    public int CommonWeight;
    public int RareWeight;
    public int EpicWeight;
    public int UniqueWeight;

    public eAbilGrade GetRandomGrade()
    {
        int totalWeight = 0;
        foreach (int weight in GradeIterator())
        {
            totalWeight += weight;
        }
        string debuggingText = "";
        int randomWeight = UnityEngine.Random.Range(0, totalWeight);
        debuggingText += $"randomWeight set as {randomWeight}\n";
        eAbilGrade resultGrade = 0;
        foreach (int weight in GradeIterator())
        {
            randomWeight -= weight;
            debuggingText = $"randomWeight decrease as {weight} now {randomWeight}\n";
            if (randomWeight < 0)
            {
                debuggingText = $"randomWeight under 0 so result grade is set as as {resultGrade}\n";
                break;
            }
            resultGrade++;
        }

        if(resultGrade == eAbilGrade.Count)
        {
            Debug.Log(debuggingText);
        }

        return resultGrade;
    }

    public IEnumerable<int> GradeIterator()
    {
        yield return CommonWeight;
        yield return RareWeight;
        yield return EpicWeight;
        yield return UniqueWeight;
    }

    public override void AutoUpdate()
    {
    }
}