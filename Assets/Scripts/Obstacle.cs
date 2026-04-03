using UnityEngine;

public class Obstacle : MonoBehaviour // class 类
{
    // goblin
    // 浮点数 整型数 布尔值 字符
    //变量
    // int healthpoint 100
    // float attackpoint 100

    // 执行攻击
    // f（x）
    // void ExecuteAttack(){

    //}
    // 执行逃跑
    // 执行追击

    // 
    // values 变量
    // function 函数
    [Min(0f)]
    public float moveSpeed = 5f;

    [Tooltip("当障碍物的 x 坐标小于该值时自动销毁。")]
    public float destroyX = -20f;
    private void Start()
    {
    }
    void Update()
    {
        transform.Translate(Vector3.left * (moveSpeed * Time.deltaTime), Space.World);

        if (transform.position.x < destroyX)
        {
            Destroy(gameObject);
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;
        if (GameManager.Instance != null)
            GameManager.Instance.OnPlayerHitObstacle();
    }
}