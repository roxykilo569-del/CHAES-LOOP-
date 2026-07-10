using UnityEngine;

public class GoodZone : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        RhythmTarget target = other.GetComponentInParent<RhythmTarget>();

        if (target != null && RhythmJudgeManager.Instance != null)
        {
            RhythmJudgeManager.Instance.RegisterGood(target);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        RhythmTarget target = other.GetComponentInParent<RhythmTarget>();

        if (target != null && RhythmJudgeManager.Instance != null)
        {
            RhythmJudgeManager.Instance.UnregisterGood(target);
        }
    }
}