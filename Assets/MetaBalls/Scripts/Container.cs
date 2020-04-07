using UnityEngine;

public class Container : MonoBehaviour
{
    public float safeZone;
    public float resolution;
    public float threshold;
    public ComputeShader computeShader;
    public bool calculateNormals;

    private CubeGrid grid;

    public void Start() => grid = new CubeGrid(this, computeShader);

    public void Update()
    {
        grid.evaluateAll(GetComponentsInChildren<MetaBall>());

        Mesh mesh = GetComponent<MeshFilter>().mesh;
        mesh.Clear();
        mesh.vertices = grid.vertices.ToArray();
        mesh.triangles = grid.getTriangles();

        if (calculateNormals)
        {
            mesh.RecalculateNormals();
        }
    }
}