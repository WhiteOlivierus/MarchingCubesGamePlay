using UnityEditor;
using UnityEngine;

public partial class MeshGenerator : MonoBehaviour
{
    private const int THREADGROUPSIZE = 8;

    [Header("General Settings")]
    public DensityGenerator densityGenerator;

    public ComputeShader shader;

    public Vector3 center;
    public Vector3 offset;
    public float boundsSize;
    public Vector3 bounds;

    [Range(2, 100)]
    private int numPointsPerAxis = 10;

    private ComputeBuffer triangleBuffer;
    private ComputeBuffer pointsBuffer;
    private ComputeBuffer triCountBuffer;
    public Vector3 startPosition;

    private void Awake() => startPosition = transform.position;

    public void RequestMeshUpdate(Chunk chunk)
    {
        CreateBuffers();

        UpdateChunkMesh(chunk);

        ReleaseBuffers();
    }

    public void UpdateChunkMesh(Chunk chunk)
    {
        if (pointsBuffer == null)
            return;

        int numVoxelsPerAxis = numPointsPerAxis - 1;
        //float pointSpacing = chunk.boundSize / (numPointsPerAxis - 1);
        float pointSpacing = boundsSize / (numPointsPerAxis - 1);

        Vector3 movementOffset = startPosition - transform.position;

        //densityGenerator.Generate(pointsBuffer,
        //                          numPointsPerAxis,
        //                          chunk.boundSize,
        //                          chunk.boundSize.ToVector(),
        //                          chunk.position,
        //                          -chunk.offset,
        //                          pointSpacing);

        densityGenerator.Generate(pointsBuffer,
                          numPointsPerAxis,
                          boundsSize,
                          Vector3.zero,
                          center - movementOffset,
                          (-transform.parent.localPosition - (chunk.offset * boundsSize)) + movementOffset,
                          pointSpacing);

        triangleBuffer.SetCounterValue(0);
        shader.SetBuffer(0, "points", pointsBuffer);
        shader.SetBuffer(0, "triangles", triangleBuffer);
        shader.SetInt("numPointsPerAxis", numPointsPerAxis);
        shader.SetFloat("isoLevel", 1);

        int numThreadsPerAxis = Mathf.CeilToInt(numVoxelsPerAxis / (float)THREADGROUPSIZE);
        shader.Dispatch(0, numThreadsPerAxis, numThreadsPerAxis, numThreadsPerAxis);

        ComputeBuffer.CopyCount(triangleBuffer, triCountBuffer, 0);
        int[] triCountArray = { 0 };
        triCountBuffer.GetData(triCountArray);
        int numTris = triCountArray[0];

        Triangle[] tris = new Triangle[numTris];
        triangleBuffer.GetData(tris, 0, 0, numTris);

        UpdateMesh(chunk, numTris, tris);
    }

    private static void UpdateMesh(Chunk chunk, int numTris, Triangle[] tris)
    {
        Mesh mesh = chunk.meshFilter.mesh;

        if (mesh == null)
            return;

        mesh.Clear();

        Vector3[] vertices = new Vector3[numTris * 3];
        int[] meshTriangles = new int[numTris * 3];

        for (int i = 0; i < numTris; i++)
            for (int k = 0; k < 3; k++)
            {
                meshTriangles[i * 3 + k] = i * 3 + k;
                vertices[i * 3 + k] = tris[i][k];
            }

        mesh.vertices = vertices;
        mesh.triangles = meshTriangles;

        if (vertices.Length == 0)
            return;

        //NormalSolver.RecalculateNormals(mesh, 90);

        //Unwrapping.GenerateSecondaryUVSet(mesh);
        //mesh.uv = mesh.uv2;

        chunk.meshFilter.mesh = mesh;
    }

    private void OnDestroy() =>
        ReleaseBuffers();

    private void CreateBuffers()
    {
        int numPoints = numPointsPerAxis * numPointsPerAxis * numPointsPerAxis;
        int numVoxelsPerAxis = numPointsPerAxis - 1;
        int numVoxels = numVoxelsPerAxis * numVoxelsPerAxis * numVoxelsPerAxis;
        int maxTriangleCount = numVoxels * 5;

        ReleaseBuffers();

        triangleBuffer = new ComputeBuffer(maxTriangleCount, sizeof(float) * 3 * 3, ComputeBufferType.Append);
        pointsBuffer = new ComputeBuffer(numPoints, sizeof(float) * 4);
        triCountBuffer = new ComputeBuffer(1, sizeof(int), ComputeBufferType.Raw);
    }

    private void ReleaseBuffers()
    {
        if (triangleBuffer == null)
            return;

        triangleBuffer.Release();
        pointsBuffer.Release();
        triCountBuffer.Release();
    }
}
