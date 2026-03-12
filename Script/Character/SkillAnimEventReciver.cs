using System.Collections.Generic;
using UnityEngine;

public class SkillAnimEventReciver : MonoBehaviour
{
    private Dictionary<eActorType, SkillActor.SkillAnimEventCallback> mOnSkillEventDic;

    public void SkillEventInvoking(AnimationEvent animEventParameter)
    {
        animEventParameter.GetSkillEventData(out eActorType actorType, out eSkillEventMarkerType markerType);

        Debug.Assert(mOnSkillEventDic.ContainsKey(actorType), $"actor Type is not contained you cannot invoke skill event [{gameObject.name}], [{actorType}]");
        mOnSkillEventDic[actorType].Invoke(markerType, animEventParameter.animatorClipInfo.clip.name);
    }

    public void EnrollEventInvoking(eActorType actor, SkillActor.SkillAnimEventCallback onSkillEvent)
    {
        if(mOnSkillEventDic == null)
        {
            mOnSkillEventDic = new Dictionary<eActorType, SkillActor.SkillAnimEventCallback>();
        }

        Debug.Assert(actor.IsAttackType(), $"Actor [{actor}]");
        mOnSkillEventDic.Add(actor, onSkillEvent);
    }
}

public enum eSkillEventMarkerType
{
    InvokeSkillFX,
    InvokeAttackBox,
    InvokeProjectile,
    ChangeProgressState,
    InvokeSound,
    InvokeTransition,
    SetComboInputWait,
    InvokeCameraActing,
    PowerOverwalrming,
    InvokePointSpecifyAttack,
    ChanellingAttack,
    Count,
}
