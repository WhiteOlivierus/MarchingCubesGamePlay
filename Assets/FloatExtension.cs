using UnityEngine;

public static class FloatExtension
{
    public static Vector3 ToVector(this float f) =>
        new Vector3(f, f, f);
}
