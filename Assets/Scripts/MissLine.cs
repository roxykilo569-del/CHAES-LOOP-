using UnityEngine;

public class MissLine : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        RhythmTarget target = other.GetComponentInParent<RhythmTarget>();

        if (target != null && RhythmJudgeManager.Instance != null)
        {
            RhythmJudgeManager.Instance.ForceMiss(target);
        }
    }
}