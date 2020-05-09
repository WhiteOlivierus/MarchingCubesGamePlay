using UnityEngine;

public class MeshGenerator : MonoBehaviour
{
    private const int THREADGROUPSIZE = 8;
    private const int NUMBER_OF_POINTS_PER_AXIS = 10;
    private const int TRI_SIZE = 3; //Never toutch, you ever seen a trianlge with 2 or 4 points
    [Header("General Settings")]
    public DensityGenerator densityGenerator;
    public ComputeShader shader;

    [Header("Positions")]
    public Vector3 center;
    public Vector3 offset;
    public float boundsSize;
    public Vector3 bounds;

    [Header("Offsets")]
    public Vector3 startPosition;
    public Vector3 lastRotation;

    private ComputeBuffer triangleBuffer;
    private ComputeBuffer pointsBuffer;
    private ComputeBuffer triCountBuffer;

    private void Awake() => startPosition = transform.position;

    private void Update() => lastRotation = transform.root.rotation.eulerAngles;

    private void OnDestroy() => ReleaseBuffers();

    public void RequestMeshUpdate(Chunk chunk)
    {
        CreateBuffers();

        UpdateChunkMesh(chunk);

        ReleaseBuffers();
    }

    private void UpdateChunkMesh(Chunk chunk)
    {
        if (pointsBuffer == null)
            return;

        GenerateVoxels(chunk);

        Triangle[] tris = GenerateMesh(NUMBER_OF_POINTS_PER_AXIS - 1);

        UpdateMesh(chunk, tris);
    }

    private void GenerateVoxels(Chunk chunk)
    {
        float pointSpacing = boundsSize / (NUMBER_OF_POINTS_PER_AXIS - 1);

        Vector3 rotation = lastRotation - transform.root.rotation.eulerAngles;
        center = center.RotatePointAroundPivot(Vector3.zero, rotation);

        Vector3 movementOffset = startPosition - transform.position;
        offset = (-transform.parent.localPosition - (chunk.offset * chunk.boundSize)) + movementOffset;

        densityGenerator.Generate(pointsBuffer,
                                  NUMBER_OF_POINTS_PER_AXIS,
                                  boundsSize,
                                  Vector3.zero,
                                  center - movementOffset,
                                  offset,
                                  pointSpacing);
    }

    private Triangle[] GenerateMesh(int numVoxelsPerAxis)
    {
        triangleBuffer.SetCounterValue(0);

        shader.SetBuffer(0, "points", pointsBuffer);
        shader.SetBuffer(0, "triangles", triangleBuffer);
        shader.SetInt("numPointsPerAxis", NUMBER_OF_POINTS_PER_AXIS);
        shader.SetFloat("isoLevel", 1);

        int numThreadsPerAxis = Mathf.CeilToInt(numVoxelsPerAxis / (float)THREADGROUPSIZE);

        shader.Dispatch(0, numThreadsPerAxis, numThreadsPerAxis, numThreadsPerAxis);

        ComputeBuffer.CopyCount(triangleBuffer, triCountBuffer, 0);

        int[] triCountArray = { 0 };
        triCountBuffer.GetData(triCountArray);
        Triangle[] tris = new Triangle[(triCountArray[0])];
        triangleBuffer.GetData(tris, 0, 0, triCountArray[0]);

        return tris;
    }

    private void UpdateMesh(Chunk chunk, Triangle[] tris)
    {
        int amountOfTris = tris.Length;
        if (chunk.meshFilter.mesh == null || amountOfTris == 0)
            return;

        chunk.meshFilter.mesh.Clear();

        int size = amountOfTris * TRI_SIZE;
        Vector3[] vertices = new Vector3[size];
        int[] meshTriangles = new int[size];

        for (int i = 0; i < amountOfTris; i++)
            for (int k = 0; k < TRI_SIZE; k++)
            {
                meshTriangles[i * TRI_SIZE + k] = i * TRI_SIZE + k;
                vertices[i * TRI_SIZE + k] = tris[i][k];
            }

        chunk.meshFilter.mesh.vertices = vertices;
        chunk.meshFilter.mesh.triangles = meshTriangles;

        chunk.meshFilter.mesh = chunk.meshFilter.mesh;
    }

    private void CreateBuffers()
    {
        int numPoints = NUMBER_OF_POINTS_PER_AXIS * NUMBER_OF_POINTS_PER_AXIS * NUMBER_OF_POINTS_PER_AXIS;
        int numVoxelsPerAxis = NUMBER_OF_POINTS_PER_AXIS - 1;
        int numVoxels = numVoxelsPerAxis * numVoxelsPerAxis * numVoxelsPerAxis;
        int maxTriangleCount = numVoxels * 5;

        ReleaseBuffers();

        triangleBuffer = new ComputeBuffer(maxTriangleCount, sizeof(float) * TRI_SIZE * TRI_SIZE, ComputeBufferType.Append);
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
