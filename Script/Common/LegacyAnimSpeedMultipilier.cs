using UnityEngine;

[RequireComponent(typeof(Animation))]
public class LegacyAnimSpeedMultipilier : MonoBehaviour
{
    public void SetSpeed(float speed)
    {
        Animation anim = GetComponent<Animation>();

        foreach (AnimationState item in anim)
        {
            item.speed = speed;
        }
    }
}
