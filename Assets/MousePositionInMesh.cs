using UnityEngine;

public class MousePositionInMesh : MonoBehaviour
{
    public float boundsSize = 1;
    private Vector3 cursorPosition;
    private Vector3 positiveCorner;
    private Vector3 negativeCorner;
    private CharacterController con;

    private void Awake() => con = transform.parent.GetComponent<CharacterController>();

    private void Update()
    {
        UpdateBounds();

        cursorPosition = GetMouseWorldspace(boundsSize);

        positiveCorner = CreatePositiveCorner();
        negativeCorner = CreateNegativeCorner();

        cursorPosition = cursorPosition.Clamp(negativeCorner, positiveCorner);
    }

    private Vector3 CreatePositiveCorner() => new Vector3(transform.position.x + (transform.localScale.x / 2),
                                               transform.position.y + (transform.localScale.y / 2),
                                               transform.position.z + (transform.localScale.z / 2));

    private Vector3 CreateNegativeCorner() => new Vector3(transform.position.x - (transform.localScale.x / 2),
                                               transform.position.y - (transform.localScale.y / 2),
                                               transform.position.z - (transform.localScale.z / 2));

    private Vector3 GetMouseWorldspace(float boundsSize)
    {
        Vector3 v3 = Input.mousePosition;
        v3.z = boundsSize;
        v3 = Camera.main.ScreenToWorldPoint(v3);
        v3.z = boundsSize;
        return v3;
    }

    private void UpdateBounds()
    {
        transform.localScale = Vector3.one * boundsSize;
        transform.localPosition = new Vector3(0, (boundsSize / 2) - (con.height / 2), (boundsSize / 2) + con.radius);
    }

    private void OnDrawGizmos()
    {
        con = transform.parent.GetComponent<CharacterController>();

        UpdateBounds();

        positiveCorner = CreatePositiveCorner();
        negativeCorner = CreateNegativeCorner();

        Gizmos.color = new Color(1f, 0, 0, 0.5f);
        Gizmos.DrawCube(transform.position, Vector3.one * boundsSize);

        Gizmos.color = new Color(0, 0, 1f, 0.5f);
        Gizmos.DrawSphere(cursorPosition, .25f);

        Gizmos.color = new Color(0, 1f, 0, 0.5f);
        Gizmos.DrawSphere(positiveCorner, .10f);

        Gizmos.color = new Color(0, 1f, 0, 0.5f);
        Gizmos.DrawSphere(negativeCorner, .10f);
    }
}
