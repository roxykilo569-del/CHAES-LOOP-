using UnityEngine;

public class AttackBreakable : MonoBehaviour
{
    public GameObject breakEffect;

    private bool isBroken = false;

    public void HitByAttack()
    {
        if (isBroken) return;

        isBroken = true;

        if (ScanlineEffectController.Instance != null)
        {
            ScanlineEffectController.Instance.PulseStrong(0.06f, 0.08f);
        }

        if (HitScreenEffect.Instance != null)
        {
            HitScreenEffect.Instance.PlayHitEffect();
        }
        if (breakEffect != null)
        {
            Instantiate(breakEffect, transform.position, Quaternion.identity);
        }

        Destroy(gameObject);
    }
}
