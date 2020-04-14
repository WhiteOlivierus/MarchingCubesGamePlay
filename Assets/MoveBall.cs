using UnityEngine;

public class MoveBall : MonoBehaviour
{
    public MeshGenerator meshGenerator;

    private bool once = false;

    private void Update()
    {
        if (Input.GetMouseButtonDown(0) && !once)
        {
            once = true;
            meshGenerator.CreateChunk(Vector3Int.zero);
        }

        if (Input.GetMouseButton(0))
        {
            meshGenerator.offset = GetMouseWorldspace();
            meshGenerator.RequestMeshUpdate();
        }
    }

    private Vector3 GetMouseWorldspace()
    {
        Vector3 v3 = Input.mousePosition;
        v3.z = 10f;
        v3 = Camera.main.ScreenToWorldPoint(v3);
        v3.z = 0f;
        return v3 * -1;
    }
}
