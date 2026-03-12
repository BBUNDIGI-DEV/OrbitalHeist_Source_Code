using UnityEngine;

public class SwitchingSkillActor : SkillActor
{
    public SwitchingSkillActor(ActorStateMachine ownerSM, SkillConfig config,AttackBoxElement.OnAttackBoxHit onAttackBoxHit ,System.Action onAttackEnd) 
        : base(ownerSM, config, onAttackBoxHit, onAttackEnd)
    {
    }

    public override void InovkeActing(object parameter1, object parameter2)
    {
        checkParamterValidate(typeof(float), typeof(bool), parameter1, parameter2);

        bool invokeSwitchingAttack = (bool)parameter2;
        if(invokeSwitchingAttack)
        {
            base.InovkeActing(parameter1, null);
        }
        else
        {
            PlayerManager.Instance.SwitchCharacterToNext(true);
        }
    }
}
