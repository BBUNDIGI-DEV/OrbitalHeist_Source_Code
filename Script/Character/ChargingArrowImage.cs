using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChargingArrowImage : MonoBehaviour
{
    [SerializeField] private eActorType sfBindingActor;
    private void Awake()
    {



    }

    private void Start()
    {
        PlayerCharacterController charController = GetComponentInParent<PlayerCharacterController>();

        if (charController == null)
        {
            gameObject.SetActive(false);
            return;
        }
        ChargeSkillActor chargeSkillActor = charController.SM.GetActor<ChargeSkillActor>(sfBindingActor);
        chargeSkillActor.NormalizedChargingDuration.AddListener(resizingArrow);
    }

    private void resizingArrow(float normalizeChargingAmount)
    {
        transform.localScale = new Vector3(normalizeChargingAmount, 1.0f, 1.0f);
    }
}
