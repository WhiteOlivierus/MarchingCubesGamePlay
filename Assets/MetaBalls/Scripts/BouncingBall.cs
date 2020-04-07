using UnityEngine;

namespace MetaBall
{
    public class BouncingBall : MetaBall
    {
        public float speed;

        private Container container;
        private Vector3 direction;

        private Vector3 containerPosition;
        private Vector3 containerScale;

        public override void Start()
        {
            base.Start();

            direction = Random.onUnitSphere;
            container = GetComponentInParent<Container>();
        }

        public void Update()
        {
            containerPosition = container.transform.position;
            containerScale = container.transform.localScale;

            transform.position = UpdateVector(transform.position, Time.deltaTime);
        }

        private Vector3 UpdateVector(Vector3 position, float step)
        {
            float x = UpdateValue(position.x, Vector3.left, containerPosition.x, containerScale.x);
            float y = UpdateValue(position.y, Vector3.down, containerPosition.y, containerScale.y);
            float z = UpdateValue(position.z, Vector3.back, containerPosition.z, containerScale.z);

            return new Vector3(x, y, z) + direction * speed * step;
        }

        private float UpdateValue(float value, Vector3 vector, float position, float scale)
        {
            if (value + radius + container.safeZone > position + (scale / 2))
            {
                value -= 0.01f;
                direction = Vector3.Reflect(direction, vector);
            }
            else if (value - radius - container.safeZone < position - (scale / 2))
            {
                value += 0.01f;
                direction = Vector3.Reflect(direction, vector * -1);
            }

            return value;
        }
    }
}