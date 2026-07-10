using UnityEngine;

public class RhythmTarget : MonoBehaviour
{
    [Header("Required Key")]
    [Tooltip("0 = Z, 1 = X, 2 = C")]
    public int requiredKey = 2;

    [Header("State")]
    public bool judged = false;

    public void OnJudged(string result)
    {
        if (judged) return;

        judged = true;

        Debug.Log(
            "Judgement: " + result +
            " / Required Key: " + GetKeyName(requiredKey) +
            " / " + gameObject.name
        );

        // 2 = C，也就是攻击障碍物
        if (requiredKey == 2)
        {
            if (BossHitEffect.Instance != null)
            {
                BossHitEffect.Instance.PlayHitEffect();
            }

            AttackBreakable breakable = GetComponent<AttackBreakable>();

            if (breakable != null)
            {
                breakable.HitByAttack();
            }
            else
            {
                Destroy(gameObject);
            }
        }
    }

    public string GetRequiredKeyName()
    {
        return GetKeyName(requiredKey);
    }

    public static string GetKeyName(int key)
    {
        switch (key)
        {
            case 0:
                return "Z";
            case 1:
                return "X";
            case 2:
                return "C";
            default:
                return "Unknown";
        }
    }
}