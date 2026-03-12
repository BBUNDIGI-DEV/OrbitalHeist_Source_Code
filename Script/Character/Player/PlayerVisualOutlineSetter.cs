using System.Collections;
using UnityEngine;


[RequireComponent(typeof(OutlineController))]
public class PlayerVisualOutlineSetter : MonoBehaviour
{
    private OutlineController mOutlineController;
    private PlayerCharacterController mPlayerController;

    private void Awake()
    {
        mOutlineController = GetComponent<OutlineController>();
        mPlayerController = GetComponentInParent<PlayerCharacterController>();
    }

    private void Update()
    {
        if(mPlayerController.BuffHandler.CheckBuffActivated(eBuffNameID.PowerOverwalming))
        {
            mOutlineController.outlineWidth = RuntimeDataLoader.OutlineConfig.PowerOverwalmingSnapshot.OutlineWidth;
            mOutlineController.outlineColor = RuntimeDataLoader.OutlineConfig.PowerOverwalmingSnapshot.OutlineColor;
            return;
        }

        if (PlayerManager.Instance.GlobalPlayerStatus.ForceShieldAmount != 0)
        {
            mOutlineController.outlineWidth = RuntimeDataLoader.OutlineConfig.ForceShieldOutlineSnapshot.OutlineWidth;
            mOutlineController.outlineColor = RuntimeDataLoader.OutlineConfig.ForceShieldOutlineSnapshot.OutlineColor;
            return;
        }

        mOutlineController.outlineWidth = 0.0f;
    }

}

