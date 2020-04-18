using UnityEngine;

public class MoveBall : MonoBehaviour
{
    private const int FORCE = 1000;

    public MeshGenerator meshGenerator;

    private bool ballSelected = false;

    private Chunk chunk;

    private void Update()
    {
        //if (Input.GetMouseButtonDown(0) && !ballSelected)
        //{
        //    ballSelected = true;
        //    chunk = meshGenerator.CreateChunk(Vector3Int.zero);
        //}

        //if (Input.GetMouseButton(0))
        //{
        //    meshGenerator.offset = GetMouseWorldspace();
        //    meshGenerator.RequestMeshUpdate();
        //}

        //if (Input.GetMouseButtonUp(0) && ballSelected)
        //{
        //    ballSelected = false;

        //    Vector3 direction = Camera.main.transform.forward;

        //    chunk.meshRigidbody.isKinematic = false;
        //    chunk.meshRigidbody.AddForce(direction * FORCE);
        //}
    }

    //private Vector3 GetMouseWorldspace()
    //{
    //    Vector3 v3 = Input.mousePosition;
    //    v3.z = 10f;
    //    v3 = Camera.main.ScreenToWorldPoint(v3);
    //    v3.z = 0f;
    //    return v3 * -1;
    //}
}
