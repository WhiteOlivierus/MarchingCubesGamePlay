using UnityEngine;

public class Chunk : MonoBehaviour
{
    public Vector3Int coord;

    [HideInInspector]
    public Mesh mesh;

    MeshFilter meshFilter;
    MeshRenderer meshRenderer;
    MeshCollider meshCollider;

    public void DestroyOrDisable()
    {
        if (Application.isPlaying)
        {
            mesh.Clear();
            gameObject.SetActive(false);
        }
        else
            DestroyImmediate(gameObject, false);
    }

    // Add components/get references in case lost (references can be lost when working in the editor)
    public void SetUp(Material mat, bool generateCollider)
    {
        meshFilter = GetComponent<MeshFilter>();
        meshRenderer = GetComponent<MeshRenderer>();
        meshCollider = GetComponent<MeshCollider>();

        if (meshFilter == null)
            meshFilter = gameObject.AddComponent<MeshFilter>();

        if (meshRenderer == null)
            meshRenderer = gameObject.AddComponent<MeshRenderer>();

        if (meshCollider == null && generateCollider)
            meshCollider = gameObject.AddComponent<MeshCollider>();

        if (meshCollider != null && !generateCollider)
            DestroyImmediate(meshCollider);

        mesh = meshFilter.sharedMesh;

        if (mesh == null)
        {
            mesh = new Mesh();
            mesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;
            meshFilter.sharedMesh = mesh;
        }

        meshRenderer.material = mat;

        if (!generateCollider)
            return;

        if (meshCollider.sharedMesh == null)
            meshCollider.sharedMesh = mesh;

        // force update
        meshCollider.enabled = false;
        meshCollider.enabled = true;
    }
}
