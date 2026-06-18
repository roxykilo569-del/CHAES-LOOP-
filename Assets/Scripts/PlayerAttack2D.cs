using System.Collections;
using UnityEngine;

public class PlayerAttack2D : MonoBehaviour
{
    [Header("Attack Settings")]
    public GameObject attackHitBox;
    public float attackActiveTime = 0.12f;
    public float attackCooldown = 0.25f;

    [Header("Animation")]
    public Animator animator;

    private bool canAttack = true;

    void Start()
    {
        if (attackHitBox != null)
        {
            attackHitBox.SetActive(false);
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.C) && canAttack)
        {
            StartCoroutine(Attack());
        }
    }

    IEnumerator Attack()
    {
        canAttack = false;

        if (animator != null)
        {
            animator.SetTrigger("Attack");
        }

        attackHitBox.SetActive(true);

        yield return new WaitForSeconds(attackActiveTime);

        attackHitBox.SetActive(false);

        yield return new WaitForSeconds(attackCooldown);

        canAttack = true;
    }
}