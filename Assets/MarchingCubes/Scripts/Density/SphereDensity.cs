using UnityEngine;

public class SphereDensity : DensityGenerator
{
    public float radius = 1;

    public override ComputeBuffer Generate(ComputeBuffer pointsBuffer, int numPointsPerAxis, float boundsSize, Vector3 worldBounds, Vector3 center, Vector3 offset, float spacing)
    {
        densityShader.SetFloat("radius", radius);
        return base.Generate(pointsBuffer, numPointsPerAxis, boundsSize - radius, worldBounds, center, offset, spacing);
    }
}
