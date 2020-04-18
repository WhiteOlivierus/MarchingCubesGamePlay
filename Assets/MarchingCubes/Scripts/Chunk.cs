using UnityEngine;

public class Chunk : MonoBehaviour
{
    public Vector3Int coord;
    private MeshFilter meshFilter;
    private MeshRenderer meshRenderer;
    private MeshCollider meshCollider;
    public Rigidbody meshRigidbody;

    public Mesh Mesh { get; private set; }

    public void DestroyOrDisable()
    {
        if (Application.isPlaying)
        {
            Mesh.Clear();
            gameObject.SetActive(false);
            return;
        }

        DestroyImmediate(gameObject, false);
    }

    // Add components/get references in case lost (references can be lost when working in the editor)
    public void SetUp(Material mat, PhysicMaterial physicsMaterial, bool generateCollider)
    {
        meshFilter = CreateComponent<MeshFilter>();
        meshRenderer = CreateComponent<MeshRenderer>();
        meshCollider = CreateComponent<MeshCollider>();
        meshRigidbody = CreateComponent<Rigidbody>();

        if (meshRigidbody != null)
            meshRigidbody.isKinematic = true;

        if (meshCollider != null && !generateCollider)
            DestroyImmediate(meshCollider);

        Mesh = meshFilter.sharedMesh;

        if (Mesh == null)
            meshFilter.sharedMesh = new Mesh { indexFormat = UnityEngine.Rendering.IndexFormat.UInt32 };

        CreatCollider(generateCollider);

        meshCollider.material = physicsMaterial;
        meshRenderer.material = mat;
    }

    private void CreatCollider(bool generateCollider)
    {
        if (!generateCollider)
            return;

        meshCollider.cookingOptions = MeshColliderCookingOptions.EnableMeshCleaning;

        if (meshCollider.sharedMesh == null)
            meshCollider.sharedMesh = Mesh;

        meshCollider.convex = true;

        meshCollider.enabled = false;
        meshCollider.enabled = true;
    }

    private T CreateComponent<T>() where T : Component
    {
        T parameter = gameObject.GetComponent<T>();
        if (parameter == null)
            return gameObject.AddComponent<T>();
        return parameter;
    }
}
