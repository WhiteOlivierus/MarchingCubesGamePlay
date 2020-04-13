using UnityEngine;

public class MoveBall : MonoBehaviour
{
    public MeshGenerator meshGenerator;

    private void FixedUpdate()
    {
        meshGenerator.offset = GetMouseWorldspace();
        meshGenerator.RequestMeshUpdate();
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
