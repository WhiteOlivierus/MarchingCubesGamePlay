using UnityEngine;

public static class Vector3Extension
{
    public static Vector3 Clamp(this Vector3 vector, Vector3 min, Vector3 max)
    {
        vector.x = Mathf.Clamp(vector.x, min.x, max.x);
        vector.y = Mathf.Clamp(vector.y, min.y, max.y);
        vector.z = Mathf.Clamp(vector.z, min.z, max.z);
        return vector;
    }

    public static Vector3 Multiply(this Vector3 vector, Vector3 by)
    {
        vector.x *= by.x;
        vector.y *= by.y;
        vector.z *= by.z;
        return vector;
    }

    public static Vector3 CreateBackTopRightCorner(this Vector3 vector3, Vector3 bounds) =>
        new Vector3(vector3.x + (bounds.x / 2),
                    vector3.y + (bounds.y / 2),
                    vector3.z + (bounds.z / 2));

    public static Vector3 CreateFrontBottomLeftCorner(this Vector3 vector3, Vector3 bounds) =>
        new Vector3(vector3.x - (bounds.x / 2),
                    vector3.y - (bounds.y / 2),
                    vector3.z - (bounds.z / 2));
}
