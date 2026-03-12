using UnityEngine;

public static class AnimationEventExtenstion
{
    public static bool IsSkillEvent(this AnimationEvent animEvent)
    {
        bool checkStringParamIsActorType = System.Enum.IsDefined(typeof(eActorType), animEvent.stringParameter);

        if (checkStringParamIsActorType == false)
        {
            return false;
        }

        eActorType actorType = System.Enum.Parse<eActorType>(animEvent.stringParameter);
        if (actorType.IsAttackType() == false)
        {
            return false;
        }

        if (animEvent.objectReferenceParameter == null)
        {
            return false;
        }
        if (animEvent.objectReferenceParameter is not SkillAnimEventParameter)
        {
            return false;
        }
        return true;
    }
    public static bool CheckValidForSkillEvent(this AnimationEvent animEvent)
    {
        bool checkStringParamIsActorType = System.Enum.IsDefined(typeof(eActorType), animEvent.stringParameter);

        if(checkStringParamIsActorType == false)
        {
            Debug.Log($"String Parameter is not actorType [{animEvent.stringParameter}]");
            return false;
        }

        eActorType actorType = System.Enum.Parse<eActorType>(animEvent.stringParameter);
        if(actorType.IsAttackType() == false)
        {
            Debug.LogError($"Actor Type Is Not Skill Actor [{actorType}]");
            return false;
        }

        if(animEvent.objectReferenceParameter == null)
        {
            Debug.LogError($"objectReferenceParameter Is Null");
            return false;
        }
        if(animEvent.objectReferenceParameter is not SkillAnimEventParameter)
        {
            Debug.LogError($"objectReferenceParameter Is Not SkillAnimEventParamter [{animEvent.objectReferenceParameter.name}]");
            return false;
        }

        return true;
    }

    public static void GetSkillEventData(this AnimationEvent animEvent, out eActorType skillActorType, out eSkillEventMarkerType markerType)
    {
        Debug.Assert(animEvent.CheckValidForSkillEvent(), $"You Can't Get Skill Event Data From [{animEvent.functionName}]");
        markerType = (animEvent.objectReferenceParameter as SkillAnimEventParameter).SkillEventType;
        skillActorType = System.Enum.Parse<eActorType>(animEvent.stringParameter);
    }
}

