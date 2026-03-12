using UnityEngine;

public static class AnimationExtenstion
{
    public static float GetSynchronizedSpeed(this AnimationClip animClip, float duration)
    {
        float clipTime = animClip.length;
        return clipTime / duration;
    }

    public static void Play(this Animation anim, string key, float normalizedTime)
    {
        anim.Play(key);
        anim[key].normalizedTime = Mathf.Clamp01(normalizedTime);
        anim.Sample();
        anim.Stop();
    }
}

