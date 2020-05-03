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
            BeginInput();
        else if (Input.GetMouseButton(0))
            HoldInput();
        else if (Input.GetMouseButtonUp(0) && ballSelected)
            EndInput();
    }

    private void BeginInput()
    {
        chunk.CreateObject();

        ballSelected = true;
    }

    private void HoldInput()
    {
        chunk.offset = cursorPosition;
        chunk.boundSize = boundsScale;

        meshGenerator.RequestMeshUpdate(chunk);
    }

    private void EndInput()
    {
        chunk.ReleaseObject();

        chunk.meshRigidbody.isKinematic = false;
        chunk.meshRigidbody.AddForce(Camera.main.transform.forward * FORCE);

        ballSelected = false;
    }
}

