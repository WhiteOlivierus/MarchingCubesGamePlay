using UnityEngine;

public class MoveBall : MousePositionInMesh
{
    private const int FORCE = 1000;

    public MeshGenerator meshGenerator;
    public Chunk chunk;

    private bool ballSelected = false;

    private void Awake() => chunk.CreateObject();

    private void Update()
    {
        if (Input.GetMouseButtonDown(0) && !ballSelected)
        {
            chunk.CreateObject();
            ballSelected = true;
        }

        if (Input.GetMouseButton(0))
        {
            chunk.offset = Vector3.zero;
            chunk.offset = cursorPosition;
            chunk.boundSize = boundsScale;

            meshGenerator.RequestMeshUpdate(chunk);
        }

        if (Input.GetMouseButtonUp(0) && ballSelected)
        {
            ballSelected = false;

            Vector3 direction = Camera.main.transform.forward;

            chunk.ReleaseObject();

            chunk.meshRigidbody.isKinematic = false;
            chunk.meshRigidbody.AddForce(direction * FORCE);
        }
    }
}

