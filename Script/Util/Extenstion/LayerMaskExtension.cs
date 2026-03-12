using UnityEngine;

public static class LayerMaskExtension
{
    public static bool Contains(this LayerMask mask, int layer)
    {
        return mask == (mask | (1 << layer));
    }

    public static void Add(ref this LayerMask mask, string layer)
    {
        mask |= (1 << LayerMask.NameToLayer(layer));
    }

    public static void Remove(ref this LayerMask mask, string layer)
    {
        mask &= ~(1 << LayerMask.NameToLayer(layer));
    }
}
