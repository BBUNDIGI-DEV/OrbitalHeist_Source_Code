using UnityEngine;

public static class EnumExtension
{
    public static bool IsAttackType(this eActorType actorType)
    {
        return actorType.ToString().Contains("Attack");
    }

    public static bool CheckTarget(this eTargetTag tag, string targetTag)
    {
        switch (tag)
        {
            case eTargetTag.Player:
                return targetTag == "Player";
            case eTargetTag.Enemey:
                return targetTag == "Enemey";
            case eTargetTag.PlayerAndObject:
                return targetTag == "Player" || targetTag == "HitableObject";
            case eTargetTag.All:
                return targetTag == "Enemey" || targetTag == "Player" || targetTag == "HitableObject";
            default:
                Debug.LogError(tag);
                break;
        }
        return false;
    }

    public static bool CheckIsLevelScene(this eSceneName sceneName)
    {
        return sceneName.ToString().Contains("Stage");
    }

    public static LayerMask ConvertToLayerMask(this eLayerName layerEnum)
    {
        return LayerMask.NameToLayer(layerEnum.ToString());
    }

    public static System.Type GetInputNameEnumType(this eInputSections inputSections)
    {
        switch (inputSections)
        {
            case eInputSections.BattleGamePlay:
                return typeof(eBattleInputName);
            case eInputSections.CutScene:
                return typeof(eCutSceneInputName);
            case eInputSections.Dialouge:
                return typeof(eDialogueTag);
            case eInputSections.Menu:
                return typeof(eMenuInputName);
            case eInputSections.UI:
            default:
                Debug.LogError($"dfeault switch [{inputSections}]");
                break;
        }
        return null;
    }
}

