using UnityEngine;
using System.Collections.Generic;
using TMPro;

public static class RuntimeDataLoader
{
    public static RuntimePlayData RuntimePlayData
    {
        get; private set;
    }

    public static RuntimeStageData RuntimeStageData
    {
        get; private set;
    }

    public static RuntimeUIData RuntimeUIData
    {
        get; private set;
    }

    public static GlobalMonsterConfig GlobalMonsterConfig
    {
        get; private set;
    }

    public static CameraConfig CameraConfig
    {
        get; private set;
    }

    public static Dictionary<string, SkillConfig[]> ComboSkillConfigs
    {
        get; private set;
    }

    public static Dictionary<eCharacterName, SkillConfig> AdvancedFirstAttackSkillConfigs
    {
        get; private set;
    }

    public static Dictionary<eCharacterName, SkillConfig> AdvancedSecondAttackSkillConfigs
    {
        get; private set;
    }

    public static Dictionary<eCharacterName, SkillConfig> AdvancedLastAttackSkillConfigs
    {
        get; private set;
    }

    public static Dictionary<eCharacterName, SkillConfig> AdvancedSwitchingAttackSkillConfigs
    {
        get; private set;
    }

    public static Dictionary<string, MonsterConfig> MonsterConfigDic
    {
        get; private set;
    }

    public static Dictionary<eDialogueTag, DialogueConfig[]> DialogueConfigDic
    {
        get; private set;
    }

    public static TMP_ColorGradient[] DamagedGradientPreset
    {
        get; private set;
    }

    public static BuffData[] AllBuffData
    {
        get; private set;
    }

    public static GlobalSettingConfig GlobalSetting
    {
        get; private set;
    }

    public static OutlineSnapshotConfig OutlineConfig
    {
        get; private set;
    }


    private static bool mIsLoaded = false;

    static RuntimeDataLoader()
    {
        LoadData();
    }

    public static void LoadData()
    {
        if(mIsLoaded)
        {
            return;
        }

        RuntimePlayData = Resources.Load<RuntimePlayData>("RuntimePlayData");
        RuntimeStageData = Resources.Load<RuntimeStageData>("RuntimeStageData");
        RuntimeUIData = Resources.Load<RuntimeUIData>("RuntimeUIData");
        CameraConfig = Resources.Load<CameraConfig>("Camera/CameraConfig");
        AllBuffData = Resources.LoadAll<BuffData>("BuffData");
        GlobalMonsterConfig = Resources.Load<GlobalMonsterConfig>("Monsters/Config_GlobalMonsterConfig");
        GlobalSetting = Resources.Load<GlobalSettingConfig>("GlobalSetting/GlobalSetting");
        OutlineConfig = Resources.Load<OutlineSnapshotConfig>("OutlineColorPreset/Setting_OutlineConfig");
        Debug.Assert(CameraConfig != null, "Cannot Found Camera Config");


        loadDialogueConfig();
        loadSkillAndBindingComboAttack("Player");
        loadSkillAndBindingComboAttack("Monsters");
        loadAdvacnedSkillData();
        DamagedGradientPreset = Resources.LoadAll<TMP_ColorGradient>("DamagedTextGradientPreset");
        MonsterConfigDic = LoadDatas<MonsterConfig>("Monsters");
        mIsLoaded = true;
    }

    public static Dictionary<string, T> LoadDatas<T>(string dataPath) where T : DataConfigBase
    {
        var dataDics = new Dictionary<string, T>();

        DataConfigBase[] datas = Resources.LoadAll<T>(dataPath);

        for (int i = 0; i < datas.Length; i++)
        {
            dataDics.Add(datas[i].name, datas[i] as T);
        }

        return dataDics;
    }

    private static void loadSkillAndBindingComboAttack(string path)
    {
        SkillConfig[] datas = Resources.LoadAll<SkillConfig>(path);

        if(ComboSkillConfigs == null)
        {
            ComboSkillConfigs = new Dictionary<string, SkillConfig[]>();
        }

        Dictionary<string, int> comboCountChecker = new Dictionary<string, int>();
        for (int i = 0; i < datas.Length; i++)
        {
            SkillConfig skillConfig = datas[i];

            if(!skillConfig.IsComboAttack || skillConfig.name.Contains("Advanced"))
            {
                continue;
            }

            if(comboCountChecker.ContainsKey(skillConfig.ComboSkillName))
            {
                comboCountChecker[skillConfig.ComboSkillName]++;
            }
            else
            {
                comboCountChecker.Add(skillConfig.ComboSkillName, 1);
            }
        }


        foreach (var item in comboCountChecker)
        {
            ComboSkillConfigs.Add(item.Key, new SkillConfig[item.Value]);
        }

        for (int i = 0; i < datas.Length; i++)
        {
            SkillConfig skillConfig = datas[i];
            if (skillConfig.IsComboAttack && !skillConfig.name.Contains("Advanced"))
            {
                ComboSkillConfigs[skillConfig.ComboSkillName][skillConfig.ComboIndex] = skillConfig;
            }
        }

#if UNITY_EDITOR
        foreach (var item in ComboSkillConfigs)
        {
            for (int i = 0; i < item.Value.Length; i++)
            {
                if(item.Value[i] == null)
                {
                    Debug.LogError
                        ($"Validate Combo skill config fail null ComboSkill detected" +
                        $"Config Name [{item.Key}] , Index [{i}]");
                }
            }
        }
#endif
    }

    private static void loadAdvacnedSkillData()
    {
        AdvancedFirstAttackSkillConfigs = new Dictionary<eCharacterName, SkillConfig>();
        AdvancedSecondAttackSkillConfigs = new Dictionary<eCharacterName, SkillConfig>();
        AdvancedLastAttackSkillConfigs = new Dictionary<eCharacterName, SkillConfig>();
        AdvancedSwitchingAttackSkillConfigs = new Dictionary<eCharacterName, SkillConfig>();

        SkillConfig[] datas = Resources.LoadAll<SkillConfig>("Player");

        for (int i = 0; i < datas.Length; i++)
        {
            SkillConfig skillConfig = datas[i];
            if (skillConfig.name.Contains("Advanced"))
            {
                eCharacterName charName = eCharacterName.None;
                if(skillConfig.name.Contains("Glanda"))
                {
                    charName = eCharacterName.Glanda;
                }

                if (skillConfig.name.Contains("Hypo"))
                {
                    charName = eCharacterName.Hypo;
                }

                if (skillConfig.name.Contains("Shiv"))
                {
                    charName = eCharacterName.Shiv;
                }

                Debug.Assert(charName != eCharacterName.None, "Cannot founded char name");


                if(skillConfig.name.Contains("Switching"))
                {
                    AdvancedSwitchingAttackSkillConfigs.Add(charName, skillConfig.GetRuntimeSkillConfig());
                }
                else if(skillConfig.ComboIndex == 0)
                {
                    AdvancedFirstAttackSkillConfigs.Add(charName, skillConfig.GetRuntimeSkillConfig());
                }
                else if(skillConfig.ComboIndex == 1)
                {
                    AdvancedSecondAttackSkillConfigs.Add(charName, skillConfig.GetRuntimeSkillConfig());
                }
                else if(skillConfig.ComboIndex == 2 || skillConfig.ComboIndex == 3)
                {
                    AdvancedLastAttackSkillConfigs.Add(charName, skillConfig.GetRuntimeSkillConfig());
                }
            }
        }

#if UNITY_EDITOR
        foreach (var item in ComboSkillConfigs)
        {
            for (int i = 0; i < item.Value.Length; i++)
            {
                if (item.Value[i] == null)
                {
                    Debug.LogError
                        ($"Validate Combo skill config fail null ComboSkill detected" +
                        $"Config Name [{item.Key}] , Index [{i}]");
                }
            }
        }
#endif
    }

    private static void loadDialogueConfig()
    {
        DialogueConfigDic = new Dictionary<eDialogueTag, DialogueConfig[]>();
        for (eDialogueTag i = 0; i < eDialogueTag.Count; i++)
        {
            DialogueConfig[] datas = Resources.LoadAll<DialogueConfig>($"Dialogue/{i}");
            DialogueConfigDic.Add(i, datas);
        }
    }

}
