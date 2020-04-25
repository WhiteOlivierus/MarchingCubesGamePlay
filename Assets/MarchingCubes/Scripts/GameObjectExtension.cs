using UnityEngine;

public static class GameObjectExtension
{
    public static T CreateComponent<T>(this GameObject unityObject) where T : Component
    {
        T parameter = unityObject.GetComponent<T>();
        if (parameter == null)
            return unityObject.AddComponent<T>();
        return parameter;
    }
}
