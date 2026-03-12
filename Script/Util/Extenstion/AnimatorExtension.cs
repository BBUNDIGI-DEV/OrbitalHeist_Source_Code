using UnityEngine;

public static class AnimatorExtension
{
    public static void PlayOrRewind(this Animator anim, string name)
    {
        int layer;
        layer = 0;
        if (name.Contains("Attack"))
        {
            layer = 1;
        }
        PlayOrRewind(anim, name, layer);
    }

    public static void PlayOrRewind(this Animator anim, string name, int layer)
    {
        Debug.Assert(anim.HasState(layer, Animator.StringToHash(name)), $"Cannot Goto animation state [{name}]");
        anim.Play(name, layer, 0.0f);
    }
}

