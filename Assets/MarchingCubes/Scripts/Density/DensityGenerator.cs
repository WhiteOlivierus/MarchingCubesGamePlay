using System.Collections.Generic;
using UnityEngine;

public abstract class DensityGenerator : MonoBehaviour
{
    public ComputeShader densityShader;

    private const int THREADGROUPSIZE = 8;

    protected List<ComputeBuffer> buffersToRelease;

    public virtual ComputeBuffer Generate(ComputeBuffer pointsBuffer, int numPointsPerAxis, float boundsSize, Vector3 worldBounds, Vector3 centre, Vector3 offset, float spacing)
    {
        int numThreadsPerAxis = Mathf.CeilToInt(numPointsPerAxis / (float)THREADGROUPSIZE);

        // Points buffer is populated inside shader with pos (xyz) + density (w).
        // Set paramaters
        densityShader.SetBuffer(0, "points", pointsBuffer);
        densityShader.SetInt("numPointsPerAxis", numPointsPerAxis);
        densityShader.SetFloat("boundsSize", boundsSize);
        densityShader.SetVector("centre", new Vector4(centre.x, centre.y, centre.z));
        densityShader.SetVector("offset", new Vector4(offset.x, offset.y, offset.z));
        densityShader.SetFloat("spacing", spacing);
        densityShader.SetVector("worldSize", worldBounds);

        // Dispatch shader
        densityShader.Dispatch(0, numThreadsPerAxis, numThreadsPerAxis, numThreadsPerAxis);

        // Return voxel data buffer so it can be used to generate mesh
        if (buffersToRelease == null)
            return pointsBuffer;

        foreach (var b in buffersToRelease)
            b.Release();

        // Return voxel data buffer so it can be used to generate mesh
        return pointsBuffer;
    }

    private void OnValidate()
    {
        MeshGenerator meshGenerator = FindObjectOfType<MeshGenerator>();

        if (meshGenerator == null)
            return;

        meshGenerator.RequestMeshUpdate();
    }
}
