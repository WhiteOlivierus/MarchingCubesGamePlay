﻿using UnityEngine;

public class Chunk : MonoBehaviour
{
    public Material material;
    public PhysicMaterial physicsMaterial;

    [HideInInspector] public Vector3 position;
    [HideInInspector] public float boundSize;
    [HideInInspector] public Vector3 offset;

    [HideInInspector] public MeshFilter meshFilter;
    [HideInInspector] public Rigidbody meshRigidbody;
    [HideInInspector] public MeshRenderer meshRenderer;
    [HideInInspector] public MeshCollider meshCollider;

    private GameObject unityObject;

    private void SetUpObject(GameObject unityObject)
    {
        meshFilter = unityObject.CreateComponent<MeshFilter>();
        meshRenderer = unityObject.CreateComponent<MeshRenderer>();
        meshCollider = unityObject.CreateComponent<MeshCollider>();
        meshRigidbody = unityObject.CreateComponent<Rigidbody>();

        meshRigidbody.isKinematic = true;

        meshFilter.mesh = new Mesh { indexFormat = UnityEngine.Rendering.IndexFormat.UInt32 };

        meshCollider.material = physicsMaterial;
        meshRenderer.material = material;
    }

    public void CreateObject()
    {
        //unityObject = Instantiate(gameObject, transform);
        unityObject = new GameObject("object");
        //unityObject.transform.parent = transform;
        unityObject.transform.position = transform.position;
        Destroy(unityObject.GetComponent<Chunk>());
        SetUpObject(unityObject);
    }

    public void ReleaseObject()
    {
        CreateCollider(unityObject);
    }

    private void CreateCollider(GameObject unityObject)
    {
        meshCollider.cookingOptions = MeshColliderCookingOptions.EnableMeshCleaning;

        if (meshCollider.sharedMesh == null)
            meshCollider.sharedMesh = meshFilter.mesh;

        meshCollider.convex = true;

        meshCollider.enabled = false;
        meshCollider.enabled = true;
    }
}
