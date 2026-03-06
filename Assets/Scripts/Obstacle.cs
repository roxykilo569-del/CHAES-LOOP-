using UnityEngine;

public class Obstacle : MonoBehaviour
{
    [Min(0f)]
    public float moveSpeed = 5f;

    [Tooltip("当障碍物的 x 坐标小于该值时自动销毁。")]
    public float destroyX = -20f;

    void Update()
    {
        transform.Translate(Vector3.left * (moveSpeed * Time.deltaTime), Space.World);

        if (transform.position.x < destroyX)
        {
            Destroy(gameObject);
        }
    }
}

