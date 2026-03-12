using UnityEngine;

public static class SOExtensions
{
    public static T InstantiateWithNotNameChanging<T>(this T original) where T : ScriptableObject
    {
        T instance = Object.Instantiate(original);
        instance.name = original.name;
        return instance;
    }
}