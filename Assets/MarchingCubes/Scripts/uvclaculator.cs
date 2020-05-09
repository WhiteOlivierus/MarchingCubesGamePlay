using UnityEngine;

public class UvCalculator
{
    private enum Facing { Up, Forward, Right };

    public static Vector2[] CalculateUVs(Vector3[] vertices, float scale = 1f)
    {
        Vector2[] uvs = new Vector2[vertices.Length];

        for (int i = 0; i < uvs.Length; i += 3)
        {
            Vector3 direction = Vector3.Cross(vertices[i + 1] - vertices[i], vertices[i + 2] - vertices[i]);
            Facing facing = FacingDirection(direction);

            switch (facing)
            {
                case Facing.Forward:
                    uvs[i] = ScaledUV(vertices[i].x, vertices[i].y, scale);
                    uvs[i + 1] = ScaledUV(vertices[i + 1].x, vertices[i + 1].y, scale);
                    uvs[i + 2] = ScaledUV(vertices[i + 2].x, vertices[i + 2].y, scale);
                    break;
                case Facing.Up:
                    uvs[i] = ScaledUV(vertices[i].x, vertices[i].z, scale);
                    uvs[i + 1] = ScaledUV(vertices[i + 1].x, vertices[i + 1].z, scale);
                    uvs[i + 2] = ScaledUV(vertices[i + 2].x, vertices[i + 2].z, scale);
                    break;
                case Facing.Right:
                    uvs[i] = ScaledUV(vertices[i].y, vertices[i].z, scale);
                    uvs[i + 1] = ScaledUV(vertices[i + 1].y, vertices[i + 1].z, scale);
                    uvs[i + 2] = ScaledUV(vertices[i + 2].y, vertices[i + 2].z, scale);
                    break;
            }
        }
        return uvs;
    }

    private static bool FacesThisWay(Vector3 v, Vector3 dir, Facing p, ref float maxDot, ref Facing ret)
    {
        float t = Vector3.Dot(v, dir);
        if (t > maxDot)
        {
            ret = p;
            maxDot = t;
            return true;
        }
        return false;
    }

    private static Facing FacingDirection(Vector3 v)
    {
        Facing ret = Facing.Up;
        float maxDot = Mathf.NegativeInfinity;

        if (!FacesThisWay(v, Vector3.right, Facing.Right, ref maxDot, ref ret))
            FacesThisWay(v, Vector3.left, Facing.Right, ref maxDot, ref ret);

        if (!FacesThisWay(v, Vector3.forward, Facing.Forward, ref maxDot, ref ret))
            FacesThisWay(v, Vector3.back, Facing.Forward, ref maxDot, ref ret);

        if (!FacesThisWay(v, Vector3.up, Facing.Up, ref maxDot, ref ret))
            FacesThisWay(v, Vector3.down, Facing.Up, ref maxDot, ref ret);

        return ret;
    }

    private static Vector2 ScaledUV(float uv1, float uv2, float scale) =>
        new Vector2(uv1 / scale, uv2 / scale);
}
