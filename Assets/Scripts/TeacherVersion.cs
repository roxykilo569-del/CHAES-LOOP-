using UnityEngine;

public class TeacherVersion : MonoBehaviour
{
    [Header("滑铲")]
    public float slideDuration = 0.5f;
    private float slideTimer;
    private bool isSliding;

    [Header("跳跃/重力(非Rigidbody2D)")]
    public float jumpForce = 9f;
    public float gravity = -28f;
    public float maxFallSpeed = -20f;
    private float verticalVelocity;

    [Header("地面检测")]
    public Transform groundCheck;
    public float groundCheckRadius = 0.15f;
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

        bool currentlyGrounded = IsGrounded();

        // 刚落地 → 强制把角色贴到地面高度，并切回跑步
        if (currentlyGrounded && !wasGrounded)
        {
            SnapToGroundTop();
            verticalVelocity = 0f;

            if (anim != null)
            {
                anim.Play("Run");
            }
        }

        // 滑铲冷却
        if (isSliding)
        {
            slideTimer -= Time.deltaTime;
            if (!Input.GetKey(KeyCode.X) || slideTimer <= 0)
            {
                isSliding = false;
                if (anim != null && !isSliding && currentlyGrounded)
                {
                    anim.Play("Run");
                }
            }
        }

        // 跳跃（Z键）
        if (Input.GetKeyDown(KeyCode.Z))
        {
            if (currentlyGrounded && !isSliding)
            {
                verticalVelocity = jumpForce;
                currentlyGrounded = false;
                if (anim != null)
                    anim.Play("Jump");
            }
        }

        // 滑铲（X键）
        if (Input.GetKeyDown(KeyCode.X))
        {
            if (currentlyGrounded && !isSliding)
            {
                isSliding = true;
                slideTimer = slideDuration;
                if (anim != null) anim.Play("Slide");
            }
        }
        // 只要离地就持续下落（包含从高台边缘走出）。

        // 计算速度
        if (!currentlyGrounded)
        {
            verticalVelocity += gravity * Time.deltaTime;
            if (verticalVelocity < maxFallSpeed)
                verticalVelocity = maxFallSpeed;
        }
        else if (verticalVelocity < 0f)
        {
            verticalVelocity = 0f;
        }

        // 用算出来的速度更新位置
        if (!Mathf.Approximately(verticalVelocity, 0f))
        {
            Vector3 pos = transform.position;
            pos.y += verticalVelocity * Time.deltaTime;
            transform.position = pos;
        }

        // 再做一次地面检测
        bool groundedAfterMove = IsGrounded();
        if (groundedAfterMove && verticalVelocity <= 0f)
        {
            SnapToGroundTop();
            verticalVelocity = 0f;
            if (!isSliding && anim != null)
            {
                anim.Play("Run");
            }
        }

        // 确定was grounded的值
        wasGrounded = groundedAfterMove;
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
        isSliding = false;
        wasGrounded = false;
        verticalVelocity = 0f;

        if (anim != null && !isSliding)
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

    bool IsGrounded()
    {
        if (groundCheck == null) return false;
        return Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);
    }

    void SnapToGroundTop()
    {
        if (groundCheck == null) return;

        Collider2D groundColl = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);
        if (groundColl == null) return;

        float groundTopY = groundColl.bounds.max.y;
        Vector3 finalPos = transform.position;
        finalPos.y = groundTopY;
        transform.position = finalPos;
    }
}