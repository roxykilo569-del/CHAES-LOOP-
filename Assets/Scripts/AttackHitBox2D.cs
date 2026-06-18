using System.Collections.Generic;
using UnityEngine;

public class AttackHitBox2D : MonoBehaviour
{
    private HashSet<AttackBreakable> hitObjects = new HashSet<AttackBreakable>();

    private void OnEnable()
    {
        hitObjects.Clear();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        AttackBreakable breakable = other.GetComponentInParent<AttackBreakable>();

        if (breakable == null) return;
        if (hitObjects.Contains(breakable)) return;

        hitObjects.Add(breakable);
        breakable.HitByAttack();
    }
}