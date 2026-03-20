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
        if(anim == null)
            anim = this.GetComponentInChildren<Animator>();
        startPos = transform.position;

        // 一开始就播放跑步动画
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

        // 刚落地 → 切回跑步
        if (currentlyGrounded && !wasGrounded)
        {
            // check ground height and then set up my height
            this.transform.position = groundCheck.position;
            if (anim != null)
            {
                anim.Play("Run");
            }
        }

        wasGrounded = currentlyGrounded;

        // 滑铲冷却
        if (slideTimer > 0)
            slideTimer -= Time.deltaTime;

        // 跳跃
        if (Input.GetKeyDown(KeyCode.Z))
        {
            if (currentlyGrounded && slideTimer <= 0)
            {
                //rb.velocity = new Vector2(rb.velocity.x, jumpForce);
                if (anim != null) anim.Play("Jump");
            }
        }

        // 滑铲
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
        //rb.velocity = Vector2.zero;
        isDead = false;
        slideTimer = 0;
        wasGrounded = false;

        // 复活直接跑步
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