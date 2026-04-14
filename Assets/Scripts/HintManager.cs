using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// 按「从计时起点起的绝对秒数」列表触发提示；具体显示由 <see cref="HintVisualBase"/> 或 <see cref="onHintTriggered"/> 实现。
/// </summary>
public class HintManager : MonoBehaviour
{
    [Header("目标（可选，便于场景绑定）")]
    [Tooltip("仅作引用；特效逻辑请用 HintVisual 或 UnityEvent。")]
    [SerializeField] SpriteRenderer hintSprite;

    [Header("调度")]
    [Tooltip("从计时起点算起的时刻（秒），已排序后依次触发；同秒多次时保持列表中的先后顺序。")]
    [SerializeField] List<float> hintTimesSeconds = new List<float>();

    [SerializeField] bool playOnStart = true;

    [Tooltip("勾选后先等到 GameManager.Phase == Playing再开始计时。")]
    [SerializeField] bool startWhenGamePlaying;

    [Header("触发")]
    [SerializeField] HintVisualBase hintVisual;

    [SerializeField] UnityEvent<int> onHintTriggered = new UnityEvent<int>();

    Coroutine scheduleRoutine;

    void Start()
    {
        if (playOnStart)
            StartSchedule();
    }

    void OnDisable()
    {
        StopSchedule();
    }

    /// <summary>开始或重新开始整段提示时间表。</summary>
    public void StartSchedule()
    {
        StopSchedule();
        scheduleRoutine = StartCoroutine(HintScheduleRoutine());
    }

    /// <summary>停止调度（场景关闭时也会自动调用）。</summary>
    public void StopSchedule()
    {
        if (scheduleRoutine == null) return;
        StopCoroutine(scheduleRoutine);
        scheduleRoutine = null;
    }

    IEnumerator HintScheduleRoutine()
    {
        if (startWhenGamePlaying)
        {
            yield return new WaitUntil(() =>
                GameManager.Instance != null && GameManager.Instance.Phase == GamePhase.Playing);
        }

        if (hintTimesSeconds == null || hintTimesSeconds.Count == 0)
            yield break;

        var items = new List<(float t, int origOrder)>();
        for (int i = 0; i < hintTimesSeconds.Count; i++)
        {
            float t = hintTimesSeconds[i];
            if (float.IsNaN(t) || t < 0f)
                continue;
            items.Add((t, i));
        }

        if (items.Count == 0)
            yield break;

        items.Sort((a, b) =>
        {
            int c = a.t.CompareTo(b.t);
            return c != 0 ? c : a.origOrder.CompareTo(b.origOrder);
        });

        float prev = 0f;
        for (int seq = 0; seq < items.Count; seq++)
        {
            float t = items[seq].t;
            float wait = t - prev;
            if (wait > 0f)
                yield return new WaitForSeconds(wait);
            prev = t;

            onHintTriggered.Invoke(seq);
            if (hintVisual != null)
                hintVisual.OnHintTriggered(seq);
        }
    }
}
