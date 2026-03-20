using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("滑铲")]
    public float slideDuration = 0.5f;
    private float slideTimer;

    [Header("地面检测")]
    public Transform groundCheck;
    public float groundCheckRadius = 0.1f;
    public LayerMask groundLayer;

    [SerializeField]
    private Animator anim;
    [SerializeField]
    private bool wasGrounded;

    private bool isDead;
    private Vector3 startPos;

    void Start()
    {
        groundLayer = LayerMask.GetMask("Ground");
        if (anim == null)
            anim = this.GetComponentInChildren<Animator>();
        startPos = transform.position;

        // 一开始播放跑步动画
        if (anim != null)
        {
            anim.Play("Run");
        }
    }

    void Update()
    {
        if (isDead) return;

        bool currentlyGrounded = Physics2D.OverlapCircle(
            groundCheck.position, groundCheckRadius, groundLayer);

        // 刚落地 → 强制把角色贴到地面高度，并切回跑步
        if (currentlyGrounded && !wasGrounded)
        {
            // 获取当前碰到的所有地面碰撞体
            Collider2D groundColl = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);

            if (groundColl != null)
            {
                // 直接取地面碰撞盒的**最顶部高度**
                float groundTopY = groundColl.bounds.max.y;

                Vector3 finalPos = transform.position;
                finalPos.y = groundTopY;
                transform.position = finalPos;
            }

            if (anim != null)
            {
                anim.Play("Run");
            }
        }

        wasGrounded = currentlyGrounded;

        // 滑铲冷却
        if (slideTimer > 0)
            slideTimer -= Time.deltaTime;

        // 跳跃（Z键）
        if (Input.GetKeyDown(KeyCode.Z))
        {
            if (currentlyGrounded && slideTimer <= 0)
            {
                if (anim != null) anim.Play("Jump");
            }
        }

        // 滑铲（X键）
        if (Input.GetKeyDown(KeyCode.X))
        {
            if (currentlyGrounded && slideTimer <= 0)
            {
                slideTimer = slideDuration;
                if (anim != null) anim.Play("Slide");
            }
        }
    }

    public void Die()
    {
        if (isDead) return;
        isDead = true;
        Respawn();
    }

    void Respawn()
    {
        transform.position = startPos;
        isDead = false;
        slideTimer = 0;
        wasGrounded = false;

        if (anim != null)
        {
            anim.Rebind();
            anim.Play("Run");
        }
    }

    void OnDrawGizmosSelected()
    {
        if (groundCheck != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
        }
    }
}