using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("滑铲")]
    public float slideDuration = 0.5f;
    private float slideTimer;
    private bool isSliding;

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

    [Header("Gravity")]
    public float gravity = -9.8f;
    public float verticleVelocity;
    public float jumpVelocity = 9; // jump force

    void Start()
    {
        //groundLayer = LayerMask.GetMask("Ground");
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

        // 检测是否在地上
        bool currentlyGrounded = CheckIfGrounded();

        // currentlyGrounded = true; wasGrounded = true
        // 刚落地 → 强制把角色贴到地面高度，并切回跑步        -> 落地
        if (currentlyGrounded && !wasGrounded)
        {
            SnapToGround();

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
                if (anim != null && !isSliding)
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
                currentlyGrounded = false;
                verticleVelocity = jumpVelocity;
                if (anim != null) anim.Play("Jump");
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

        // Calculate verticleVelocity value
        if (!currentlyGrounded)
        {
            // verticleVelocity 一定有速度
            verticleVelocity = verticleVelocity + gravity * Time.deltaTime;
        }
        else
        {
            verticleVelocity = 0;
        }

        // verticleVel value add to player's Postion
        Vector3 tempPos = transform.position;
        tempPos.y = tempPos.y + verticleVelocity * Time.deltaTime;
        transform.position = tempPos;

        // 👆 我们才确定好玩家的此刻的位置 因此再做一次地面检测

        // check again if we were ground
        bool groundedAfterAllMove = CheckIfGrounded();

        if (groundedAfterAllMove && verticleVelocity <= 0)
        {
            SnapToGround();
            verticleVelocity = 0;

            if (!isSliding && anim != null)
            {
                anim.Play("Run");
            }
        }

        wasGrounded = currentlyGrounded;
    }

    private bool CheckIfGrounded()
    {
        if (groundCheck == null) return false;

        return Physics2D.OverlapCircle(
                    groundCheck.position, groundCheckRadius, groundLayer);
    }

    private void SnapToGround()
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
    }

    public void Die()
    {
        if (isDead) return;
        isDead = true;
        Respawn();
    }

    /// <summary>由 GameManager 在复活点重生时调用。</summary>
    public void RespawnAt(Vector3 worldPosition)
    {
        transform.position = worldPosition;
        startPos = worldPosition;
        isDead = false;
        slideTimer = 0;
        isSliding = false;
        wasGrounded = false;
        verticleVelocity = 0f;

        if (anim != null && !isSliding)
        {
            anim.Rebind();
            anim.Play("Run");
        }
    }

    public void SetSpawnPoint(Vector3 worldPosition)
    {
        startPos = worldPosition;
    }

    public Vector3 GetSpawnPoint()
    {
        return startPos;
    }

    void Respawn()
    {
        RespawnAt(startPos);
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