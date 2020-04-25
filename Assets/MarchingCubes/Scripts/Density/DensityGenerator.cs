using UnityEngine;

public abstract class DensityGenerator : MonoBehaviour
{
    public ComputeShader densityShader;

    private const int THREADGROUPSIZE = 8;

    public virtual ComputeBuffer Generate(ComputeBuffer pointsBuffer, int numPointsPerAxis, float boundsSize, Vector3 worldBounds, Vector3 center, Vector3 offset, float spacing)
    {
        int numThreadsPerAxis = Mathf.CeilToInt(numPointsPerAxis / (float)THREADGROUPSIZE);

        // Points buffer is populated inside shader with pos (xyz) + density (w).
        // Set paramaters
        densityShader.SetBuffer(0, "points", pointsBuffer);
        densityShader.SetInt("numPointsPerAxis", numPointsPerAxis);
        densityShader.SetFloat("boundsSize", boundsSize);
        densityShader.SetVector("center", new Vector4(center.x, center.y, center.z));
        densityShader.SetVector("offset", new Vector4(offset.x, offset.y, offset.z));
        densityShader.SetFloat("spacing", spacing);
        densityShader.SetVector("worldSize", worldBounds);

        // Dispatch shader
        densityShader.Dispatch(0, numThreadsPerAxis, numThreadsPerAxis, numThreadsPerAxis);

        // Return voxel data buffer so it can be used to generate mesh
        return pointsBuffer;
    }
}
