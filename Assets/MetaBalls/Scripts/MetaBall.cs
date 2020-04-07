using UnityEngine;

public class MetaBall : MonoBehaviour
{
    public float radius;
    public bool negativeBall;

    public float Factor { get; set; }

    public virtual void Start()
    {
        Factor = (negativeBall ? -1 : 1) * (radius * 2);
    }
}
