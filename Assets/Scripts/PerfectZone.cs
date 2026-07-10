using UnityEngine;

public class PerfectZone : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        RhythmTarget target = other.GetComponentInParent<RhythmTarget>();

        if (target != null && RhythmJudgeManager.Instance != null)
        {
            RhythmJudgeManager.Instance.RegisterPerfect(target);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        RhythmTarget target = other.GetComponentInParent<RhythmTarget>();

        if (target != null && RhythmJudgeManager.Instance != null)
        {
            RhythmJudgeManager.Instance.UnregisterPerfect(target);
        }
    }
}