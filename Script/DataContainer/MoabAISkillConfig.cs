using Sirenix.OdinInspector;
using UnityEngine;

[CreateAssetMenu(menuName = "DataContainer/MoabAISkillConfig")]
public class MoabAISkillConfig : ActorConfigDataContainerBase
{
    public SkillConfig MeleeAttack;
    public SkillConfig RangeAttack;
    public SkillConfig RushAttack;
    public SkillConfig SideStep;


    public float RushableRange;
    public float MeleeAttackRange;
    public float ProjectileAttackRange;
    public float ProjectileMinAttackRange;

    public MoabComboAttackData[] MeleeCombo;
    public MoabComboAttackData[] RangeCombo;
    public MoabComboAttackData[] RushCombo;
    public MoabComboAttackData[] SideStepCombo;

    public float[] ComboTearThreshold;

    public SkillConfig[] RampageAttack;
    public float[] RampageAttackThreshold;

    protected override void initializeConfig()
    {
        BaseConfig.ActorType = eActorType.AIAttack;
        BaseConfig.IsUpdatedActor = true;
    }

    public eMoabAttackState[] PickComboAttack(int currentTear, eMoabAttackState attackStep)
    {
        Debug.Assert(attackStep != eMoabAttackState.NoneAttack, "You Cannot pick combo attack about none attack step");
        MoabComboAttackData[] comboList = null;

        switch (attackStep)
        {
            case eMoabAttackState.MeleeAttack:
                comboList = MeleeCombo;
                break;
            case eMoabAttackState.RushAttack:
                comboList = RushCombo;
                break;
            case eMoabAttackState.RangeAttack:
                comboList = RangeCombo;
                break;
            case eMoabAttackState.SideStep:
                comboList = SideStepCombo;
                break;
            default:
                break;
        }


        int totalWeight = 0;
        for (int i = 0; i < comboList.Length; i++)
        {
            totalWeight += comboList[i].GetWeight(currentTear);
        }

        int randomWeight = Random.Range(0, totalWeight);

        for (int i = 0; i < comboList.Length; i++)
        {
            randomWeight -= comboList[i].GetWeight(currentTear);
            if(randomWeight < 0)
            {
                return comboList[i].ComboInput;
            }
        }

        Debug.Assert(false);
        return null;
    }
}

[System.Serializable]
public struct MoabComboAttackData
{
    public eMoabAttackState[] ComboInput;
    public int Tear1Weight;
    public int Tear2Weight;
    public int Tear3Weight;


    public int GetWeight(int tear)
    {
        switch (tear)
        {
            case 0:
                return Tear1Weight;
            case 1:
                return Tear2Weight;
            case 2:
                return Tear3Weight;
            default:
                break;
        }
        Debug.Assert(false);
        return 0;
    }
}
