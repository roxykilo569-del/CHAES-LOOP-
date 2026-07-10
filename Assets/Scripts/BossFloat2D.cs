using UnityEngine;

public class BossFloat2D : MonoBehaviour
{
    [Header("Float")]
    public float floatHeight = 0.25f;   // 上下漂浮幅度
    public float floatSpeed = 1.5f;     // 上下漂浮速度

    [Header("Side Drift")]
    public float sideAmount = 0.08f;    // 左右轻微漂移
    public float sideSpeed = 1.0f;

    [Header("Rotation")]
    public float rotateAmount = 2f;     // 轻微旋转角度
    public float rotateSpeed = 1.2f;

    private Vector3 startPosition;

    private void Start()
    {
        startPosition = transform.position;
    }

    private void Update()
    {
        float y = Mathf.Sin(Time.time * floatSpeed) * floatHeight;
        float x = Mathf.Sin(Time.time * sideSpeed) * sideAmount;
        float zRot = Mathf.Sin(Time.time * rotateSpeed) * rotateAmount;

        transform.position = startPosition + new Vector3(x, y, 0f);
        transform.rotation = Quaternion.Euler(0f, 0f, zRot);
    }
}