using UnityEngine;

public class BouncingBall : MetaBall
{
    public float speed;

    private Container container;
    private Vector3 direction;

    public override void Start()
    {
        base.Start();
        direction = Random.onUnitSphere;
        container = GetComponentInParent<Container>();
    }

    public void Update() => transform.position = UpdatePosition(Time.deltaTime);

    public Vector3 UpdatePosition(float step)
    {
        Vector3 containerPosition = container.transform.position;
        Vector3 containerScale = container.transform.localScale;

        Vector3 position = UpdateVector(transform.position, containerPosition, containerScale);

        return position + direction * speed * step;
    }

    private Vector3 UpdateVector(Vector3 position, Vector3 containerPosition, Vector3 containerScale)
    {
        Vector3 output = new Vector3();

        output.x = Reflect(position.x, containerPosition, containerScale);
        output.y = Reflect(position.y, containerPosition, containerScale);
        output.z = Reflect(position.z, containerPosition, containerScale);

        return output;
    }

    private float Reflect(float pos, Vector3 containerPosition, Vector3 containerScale)
    {
        if (pos + radius + container.safeZone > containerPosition.y + containerScale.y / 2)
        {
            pos -= 0.01f;
            direction = Vector3.Reflect(direction, Vector3.down);
        }
        else if (pos - radius - container.safeZone < containerPosition.y - containerScale.y / 2)
        {
            pos += 0.01f;
            direction = Vector3.Reflect(direction, Vector3.up);
        }

        return pos;
    }
}
