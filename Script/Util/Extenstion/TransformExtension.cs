using System;
using UnityEngine;

public static class TransformExtensions
{
    public static int GetActiveChildCount(this Transform self)
    {
        int count = 0;
        for (int i = 0; i < self.childCount; i++)
        {
            if(self.GetChild(i).gameObject.activeSelf)
            {
                count++;
            }
        }
        return count;
    }

    public static Transform FindRecursive(this Transform self, string exactName)
    {
        return self.findRecursive(child => child.name == exactName);
    }

    public static Transform FindRecursiveTag(this Transform self, string tag)
    {
        return self.findRecursive(child => child.CompareTag(tag));
    }

    public static GameObject FindRecursiveGameobjectWithTag(this Transform self, string tag)
    {
        Transform trans = self.findRecursive(child => child.CompareTag(tag));
        if(trans == null)
        {
            return null;
        }
        else
        {
            return trans.gameObject;
        }
    }

    public static Transform FindRecursiveUp(this Transform self, string exactName)
    {
        return self.findRecursive(child => child.name == exactName);
    }

    private static Transform findRecursive(this Transform self, Func<Transform, bool> selector)
    {
        foreach (Transform child in self)
        {
            if (selector(child))
            {
                return child;
            }

            var finding = child.findRecursive(selector);

            if (finding != null)
            {
                return finding;
            }
        }

        return null;
    }

    private static Transform findRecursiveUp(this Transform selfOrNull, Func<Transform, bool> selector)
    {
        if(selfOrNull == null)
        {
            return null;
        }

        Transform parent = selfOrNull.parent;
        if(parent == null)
        {
            return null;
        }
        else
        {
            foreach (Transform child in parent)
            {
                if (selector(child) == true)
                {
                    return child;
                }
            }
        }

        return findRecursiveUp(parent, selector);
    }
}
