using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackRange2D : MonoBehaviour
{
    public static AttackRange2D Instance;

    [Header("Hit Box")]
    public BoxCollider2D hitBox;

    [Header("Attack")]
    public float activeTime = 0.18f;

    private HashSet<RhythmTarget> targetsInRange = new HashSet<RhythmTarget>();
    private Coroutine attackRoutine;

    private void Awake()
    {
        Instance = this;

        if (hitBox == null)
        {
            hitBox = GetComponent<BoxCollider2D>();
        }

        if (hitBox == null)
        {
            Debug.LogError("AttackRange2D 没有找到 BoxCollider2D");
            return;
        }

        hitBox.isTrigger = true;
        hitBox.enabled = false;
    }

    public void BeginAttack()
    {
        if (hitBox == null) return;

        if (attackRoutine != null)
        {
            StopCoroutine(attackRoutine);
        }

        targetsInRange.Clear();

        hitBox.enabled = true;

        CheckOverlapNow();

        attackRoutine = StartCoroutine(AttackRoutine());
    }

    private IEnumerator AttackRoutine()
    {
        yield return new WaitForSeconds(activeTime);

        hitBox.enabled = false;
        targetsInRange.Clear();

        attackRoutine = null;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        TryAddTarget(other);
    }

    private void CheckOverlapNow()
    {
        Collider2D[] hits = Physics2D.OverlapBoxAll(
            hitBox.bounds.center,
            hitBox.bounds.size,
            0f
        );

        foreach (Collider2D hit in hits)
        {
            TryAddTarget(hit);
        }
    }

    private void TryAddTarget(Collider2D other)
    {
        RhythmTarget target = other.GetComponentInParent<RhythmTarget>();

        if (target == null)
        {
            return;
        }

        targetsInRange.Add(target);
        Debug.Log("进入攻击范围: " + target.name);
    }

    public bool Contains(RhythmTarget target)
    {
        if (target == null) return false;

        return targetsInRange.Contains(target);
    }
}