using System.Collections.Generic;
using UnityEngine;

public class RhythmJudgeManager : MonoBehaviour
{
    public static RhythmJudgeManager Instance;

    [Header("Judge Point")]
    public Transform judgePoint;

    private List<RhythmTarget> perfectTargets = new List<RhythmTarget>();
    private List<RhythmTarget> goodTargets = new List<RhythmTarget>();
    private bool HasAnyTargetInJudgeZone()
    {
        for (int i = perfectTargets.Count - 1; i >= 0; i--)
        {
            RhythmTarget target = perfectTargets[i];

            if (target == null || target.judged)
            {
                perfectTargets.RemoveAt(i);
                continue;
            }

            return true;
        }

        for (int i = goodTargets.Count - 1; i >= 0; i--)
        {
            RhythmTarget target = goodTargets[i];

            if (target == null || target.judged)
            {
                goodTargets.RemoveAt(i);
                continue;
            }

            return true;
        }

        return false;
    }

    private void Awake()
    {
        Instance = this;
    }

    public void RegisterPerfect(RhythmTarget target)
    {
        if (target == null) return;

        if (!perfectTargets.Contains(target))
        {
            perfectTargets.Add(target);
        }
    }

    public void UnregisterPerfect(RhythmTarget target)
    {
        if (target == null) return;

        perfectTargets.Remove(target);
    }

    public void RegisterGood(RhythmTarget target)
    {
        if (target == null) return;

        if (!goodTargets.Contains(target))
        {
            goodTargets.Add(target);
        }
    }

    public void UnregisterGood(RhythmTarget target)
    {
        if (target == null) return;

        goodTargets.Remove(target);
    }

    public void TryJudge(int pressedKey)
    {
        RhythmTarget target = GetClosestTarget(perfectTargets, pressedKey);

        if (target != null)
        {
            if (RhythmScoreManager.Instance != null)
            {
                RhythmScoreManager.Instance.AddJudgement("PERFECT");
            }
            if (RhythmFeedbackUI.Instance != null)
            {
                RhythmFeedbackUI.Instance.ShowJudgement("PERFECT");
            }

            target.OnJudged("PERFECT");
            RemoveTarget(target);
            return;
        }

        target = GetClosestTarget(goodTargets, pressedKey);

        if (target != null)
        {
            if (RhythmScoreManager.Instance != null)
            {
                RhythmScoreManager.Instance.AddJudgement("GOOD");
            }

            if (RhythmFeedbackUI.Instance != null)
            {
                RhythmFeedbackUI.Instance.ShowJudgement("GOOD");
            }

            target.OnJudged("GOOD");
            RemoveTarget(target);
            return;
        }

        if (HasAnyTargetInJudgeZone())
        {
            if (RhythmScoreManager.Instance != null)
            {
                RhythmScoreManager.Instance.AddJudgement("MISS");
            }
            if (RhythmFeedbackUI.Instance != null)
            {
                RhythmFeedbackUI.Instance.ShowJudgement("MISS");
            }

            Debug.Log("MISS - 判定区内有目标，但按键不匹配 / Pressed Key: " + RhythmTarget.GetKeyName(pressedKey));
        }
        else
        {
            if (RhythmFeedbackUI.Instance != null)
            {
                RhythmFeedbackUI.Instance.ShowEmptyInput();
            }

            Debug.Log("X - 没有障碍物时按键 / Pressed Key: " + RhythmTarget.GetKeyName(pressedKey));

        }
    }


    public void ForceMiss(RhythmTarget target)
    {
        if (target == null) return;
        if (target.judged) return;

        target.judged = true;

        if (RhythmFeedbackUI.Instance != null)
        {
            RhythmFeedbackUI.Instance.ShowJudgement("MISS");
        }

        Debug.Log(
            "MISS - 错过障碍物: " + target.name +
            " / Required Key: " + target.GetRequiredKeyName()
        );

        RemoveTarget(target);

        // 如果你不想 Miss 后消失，就保留注释
        // Destroy(target.gameObject);
    }

    private RhythmTarget GetClosestTarget(List<RhythmTarget> list, int pressedKey)
    {
        RhythmTarget closest = null;
        float closestDistance = Mathf.Infinity;

        for (int i = list.Count - 1; i >= 0; i--)
        {
            RhythmTarget target = list[i];

            if (target == null || target.judged)
            {
                list.RemoveAt(i);
                continue;
            }

            if (target.requiredKey != pressedKey)
            {
                continue;
            }


            float distance = Mathf.Abs(target.transform.position.x - judgePoint.position.x);

            if (distance < closestDistance)
            {
                closestDistance = distance;
                closest = target;
            }
        }

        return closest;
    }
    private void RemoveTarget(RhythmTarget target)
    {
        perfectTargets.Remove(target);
        goodTargets.Remove(target);
    }
}