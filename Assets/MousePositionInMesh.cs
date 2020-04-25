using UnityEngine;

[ExecuteAlways]
public class MousePositionInMesh : MonoBehaviour
{
    private const float CURSOS_SIZE = .25f;
    private const float CORNER_SIZE = .10f;

    [SerializeField] public bool advancedBounds = false;

    [ConditionalHide("advancedBounds", false)]
    [SerializeField] public float boundsScale = 1;
    [ConditionalHide("advancedBounds", true)]
    [SerializeField] public Vector3 boundsDimensions = Vector3.one;

    [HideInInspector]
    public Vector3 BoundsSize =>
        advancedBounds ? boundsDimensions : boundsScale.ToVector();

    protected Vector3 cursorPosition;
    private Vector3 positiveCorner;
    private Vector3 negativeCorner;

    private CharacterController con;

    private void Awake() => con =
        transform.GetComponentInParent<CharacterController>();

    private void Update()
    {
        UpdateBoundsScale();

        // Update the bounds position
        transform.localPosition = UpdateBoundsPosition();
        Vector3 localScale = Vector3.one;

        // Create top-right outer bounds
        positiveCorner = transform.InverseTransformPoint(transform.position);
        positiveCorner = positiveCorner.CreateBackTopRightCorner(localScale);

        // Create bottom-left outer bounds
        negativeCorner = transform.InverseTransformPoint(transform.position);
        negativeCorner = negativeCorner.CreateFrontBottomLeftCorner(localScale);

        // Calculate the cursor position
        cursorPosition = GetMouseWorldspace(BoundsSize.z);
        cursorPosition = cursorPosition.Clamp(negativeCorner, positiveCorner);
    }

    private void UpdateBoundsScale()
    {
        transform.localScale = BoundsSize;

        //for (int i = 0; i < transform.childCount; i++)
        //    transform.GetChild(i).localScale = Vector3.one / 10;
    }

    private Vector3 UpdateBoundsPosition() =>
        new Vector3(0,
                    (BoundsSize.y / 2) - (con.height / 2),
                    (BoundsSize.z / 2) + con.radius);

    private Vector3 GetMouseWorldspace(float boundsSize)
    {
        Vector3 vector = Input.mousePosition;
        vector.z = boundsSize / 2;
        vector = Camera.main.ScreenToWorldPoint(vector);
        return transform.InverseTransformPoint(vector);
    }

    protected void OnDrawGizmos()
    {
        con = transform.GetComponentInParent<CharacterController>();

        Update();

        Gizmos.color = new Color(0, 0, 1f, 0.5f);
        Gizmos.DrawSphere(transform.TransformPoint(cursorPosition), CURSOS_SIZE);

        Gizmos.color = new Color(0, 1f, 0, 0.5f);
        Gizmos.DrawSphere(transform.TransformPoint(positiveCorner), CORNER_SIZE);

        Gizmos.color = new Color(0, 1f, 0, 0.5f);
        Gizmos.DrawSphere(transform.TransformPoint(negativeCorner), CORNER_SIZE);
    }
}
