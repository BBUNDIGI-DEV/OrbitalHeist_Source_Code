using UnityEngine;
using TMPro;

public class VampireSurvivalModeUI : MonoBehaviour
{
    [SerializeField] private TMP_Text sfWave;
    [SerializeField] private TMP_Text sfREmainMonster;
    [SerializeField] private Transform sfMonsterParent;

    public void SetWaveText(int curWave, int maxWave)
    {
        sfWave.text = $"{curWave}/{maxWave}";
    }

    public void SetRemainMonsterText(int monsterCount)
    {
        sfREmainMonster.text = $"{monsterCount}";
    }


    public void Update()
    {
        SetRemainMonsterText(sfMonsterParent.childCount);
    }    
}


