using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public partial class MeshGenerator : MonoBehaviour
{
    const int THREADGROUPSIZE = 8;

    [Header("General Settings")]
    public DensityGenerator densityGenerator;

    public bool fixedMapSize;
    [ConditionalHide(nameof(fixedMapSize), true)]
    public Vector3Int numChunks = Vector3Int.one;
    [ConditionalHide(nameof(fixedMapSize), false)]
    public Transform viewer;
    [ConditionalHide(nameof(fixedMapSize), false)]
    public float viewDistance = 30;

    [Space()]
    public bool autoUpdateInEditor = true;
    public bool autoUpdateInGame = true;

    public ComputeShader shader;
    public Material mat;
    public PhysicMaterial physicsMat;
    public bool generateColliders;

    [Header("Voxel Settings")]
    public float isoLevel;
    public float boundsSize = 1;
    public Vector3 offset = Vector3.zero;

    [Range(2, 100)]
    public int numPointsPerAxis = 30;

    [Header("Gizmos")]
    public bool showBoundsGizmo = true;
    public Color boundsGizmoCol = Color.white;

    GameObject chunkHolder;
    const string CHUNKHOLDERNAME = "Chunks Holder";
    List<Chunk> chunks;

    // Buffers
    ComputeBuffer triangleBuffer;
    ComputeBuffer pointsBuffer;
    ComputeBuffer triCountBuffer;

    private void Awake()
    {
        InitVariableChunkStructures();
        CreateChunkHolder();

    }

    public void Run()
    {
        CreateBuffers();

        InitChunks();
        UpdateAllChunks();

        if (Application.isPlaying)
            return;

        // Release buffers immediately in editor
        ReleaseBuffers();
    }

    public void RequestMeshUpdate()
    {
        if ((Application.isPlaying && autoUpdateInGame) ||
            (!Application.isPlaying && autoUpdateInEditor))
            Run();
    }

    private void InitVariableChunkStructures()
    {
        chunks = new List<Chunk>();
        //existingChunks = new Dictionary<Vector3Int, Chunk>();
    }

    public void UpdateChunkMesh(Chunk chunk)
    {
        int numVoxelsPerAxis = numPointsPerAxis - 1;
        int numThreadsPerAxis = Mathf.CeilToInt(numVoxelsPerAxis / (float)THREADGROUPSIZE);
        float pointSpacing = boundsSize / (numPointsPerAxis - 1);

        Vector3 centre = CentreFromCoord(chunk.coord);

        Vector3 worldBounds = new Vector3(numChunks.x, numChunks.y, numChunks.z) * boundsSize;

        if (pointsBuffer == null)
            return;

        densityGenerator.Generate(pointsBuffer, numPointsPerAxis, boundsSize, worldBounds, centre, offset, pointSpacing);

        triangleBuffer.SetCounterValue(0);
        shader.SetBuffer(0, "points", pointsBuffer);
        shader.SetBuffer(0, "triangles", triangleBuffer);
        shader.SetInt("numPointsPerAxis", numPointsPerAxis);
        shader.SetFloat("isoLevel", isoLevel);

        shader.Dispatch(0, numThreadsPerAxis, numThreadsPerAxis, numThreadsPerAxis);

        // Get number of triangles in the triangle buffer
        ComputeBuffer.CopyCount(triangleBuffer, triCountBuffer, 0);
        int[] triCountArray = { 0 };
        triCountBuffer.GetData(triCountArray);
        int numTris = triCountArray[0];

        // Get triangle data from shader
        Triangle[] tris = new Triangle[numTris];
        triangleBuffer.GetData(tris, 0, 0, numTris);

        UpdateMesh(chunk, numTris, tris);
    }

    private static void UpdateMesh(Chunk chunk, int numTris, Triangle[] tris)
    {
        Mesh mesh = chunk.Mesh;

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

        NormalSolver.RecalculateNormals(mesh, 90);

        //Unwrapping.GenerateSecondaryUVSet(mesh);
        //mesh.uv = mesh.uv2;
    }

    public void UpdateAllChunks()
    {
        // Create mesh for each chunk
        foreach (Chunk chunk in chunks)
            UpdateChunkMesh(chunk);
    }

    private void OnDestroy()
    {
        if (Application.isPlaying)
            ReleaseBuffers();
    }

    private void CreateBuffers()
    {
        int numPoints = numPointsPerAxis * numPointsPerAxis * numPointsPerAxis;
        int numVoxelsPerAxis = numPointsPerAxis - 1;
        int numVoxels = numVoxelsPerAxis * numVoxelsPerAxis * numVoxelsPerAxis;
        int maxTriangleCount = numVoxels * 5;

        // Always create buffers in editor (since buffers are released immediately to prevent memory leak)
        // Otherwise, only create if null or if size has changed
        if (Application.isPlaying && pointsBuffer != null && numPoints == pointsBuffer.count)
            return;

        if (Application.isPlaying)
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

    private Vector3 CentreFromCoord(Vector3Int coord)
    {
        // Centre entire map at origin
        if (!fixedMapSize)
            return new Vector3(coord.x, coord.y, coord.z) * boundsSize;

        Vector3 totalBounds = (Vector3)numChunks * boundsSize;
        return -totalBounds / 2 + (Vector3)coord * boundsSize + Vector3.one * boundsSize / 2;
    }

    private void CreateChunkHolder()
    {
        // Create/find mesh holder object for organizing chunks under in the hierarchy
        if (chunkHolder != null)
            return;

        chunkHolder = GameObject.Find(CHUNKHOLDERNAME);

        if (chunkHolder != null)
            return;

        chunkHolder = new GameObject(CHUNKHOLDERNAME);
    }

    // Create/get references to all chunks
    private void InitChunks()
    {
        List<Chunk> oldChunks = new List<Chunk>(FindObjectsOfType<Chunk>());

        chunks = new List<Chunk>();

        // Go through all coords and create a chunk there if one doesn't already exist
        for (int x = 0; x < numChunks.x; x++)
            for (int y = 0; y < numChunks.y; y++)
                for (int z = 0; z < numChunks.z; z++)
                    ValidateChunks(oldChunks, new Vector3Int(x, y, z));

        // Delete all unused chunks
        for (int i = 0; i < oldChunks.Count; i++)
            oldChunks[i].DestroyOrDisable();
    }

    private void ValidateChunks(List<Chunk> oldChunks, Vector3Int coord)
    {
        // If chunk already exists, add it to the chunks list, and remove from the old list.
        for (int i = 0; i < oldChunks.Count; i++)
        {
            if (oldChunks[i].coord != coord)
                continue;

            chunks.Add(oldChunks[i]);
            oldChunks.RemoveAt(i);
            break;
        }

        if (chunks.Count != 0)
            chunks[chunks.Count - 1].SetUp(mat, physicsMat, generateColliders);
    }

    private Chunk InnitChunk(Vector3Int coord)
    {
        GameObject chunk = new GameObject($"Chunk ({coord.x}, {coord.y}, {coord.z})");
        chunk.transform.parent = chunkHolder.transform;
        Chunk newChunk = chunk.AddComponent<Chunk>();
        newChunk.coord = coord;
        return newChunk;
    }

    public Chunk CreateChunk(Vector3Int coord)
    {
        Chunk chunk = InnitChunk(coord);
        chunk.coord = coord;
        chunk.SetUp(mat, physicsMat, generateColliders);
        chunks.Add(chunk);
        UpdateChunkMesh(chunk);
        return chunk;
    }

    private void OnDrawGizmos()
    {
        if (!showBoundsGizmo)
            return;

        Gizmos.color = boundsGizmoCol;

        List<Chunk> chunks = this.chunks ?? new List<Chunk>(FindObjectsOfType<Chunk>());

        foreach (var chunk in chunks)
        {
            Gizmos.color = boundsGizmoCol;
            Gizmos.DrawWireCube(CentreFromCoord(chunk.coord), Vector3.one * boundsSize);
        }
    }
}
