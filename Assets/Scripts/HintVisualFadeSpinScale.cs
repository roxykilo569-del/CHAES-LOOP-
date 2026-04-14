using System.Collections;
using UnityEngine;

/// <summary>
/// 示例：<see cref="HintVisualBase"/> 的子类：弹性入场 → 停留 → 淡出消失（可再次触发重新播放）。
/// 挂在带 <see cref="SpriteRenderer"/> 的物体上，并把该组件拖到 <see cref="HintManager.hintVisual"/>。
/// </summary>
public class HintVisualFadeSpinScale : HintVisualBase
{
    [SerializeField] SpriteRenderer targetRenderer;

    [Min(0.01f)]
    [SerializeField] float duration = 0.6f;

    [SerializeField] float startScaleMultiplier = 0.25f;
    [SerializeField] float endScaleMultiplier = 1f;

    [Tooltip("在 duration 内绕 Z 轴累计旋转的角度（度）。")]
    [SerializeField] float spinDegrees = 360f;

    [Header("Juicy 缩放")]
    [Tooltip("越大缩放越「弹」：会先冲过目标尺寸再回弹。约 1.7为经典值。")]
    [Min(0.01f)]
    [SerializeField] float scaleOvershoot = 1.75f;

    [Tooltip("透明度进度略快于缩放，让轮廓先亮起来再完成弹性落地。")]
    [Range(0.35f, 1f)]
    [SerializeField] float alphaLead = 0.72f;

    [Header("消失")]
    [Tooltip("入场结束后保持完全可见的秒数，再开始淡出。")]
    [Min(0f)]
    [SerializeField] float visibleHoldSeconds = 1.25f;

    [Tooltip("淡出时长（alpha 1→0）。设为 0 则立刻变透明。")]
    [Min(0f)]
    [SerializeField] float fadeOutDuration = 0.4f;

    [Tooltip("淡出结束后关闭 SpriteRenderer，节省绘制；下次提示触发时会自动再打开。")]
    [SerializeField] bool disableRendererWhenHidden = true;

    Vector3 initialScale;
    float initialEulerZ;
    Coroutine routine;

    void Awake()
    {
        if (targetRenderer == null)
            targetRenderer = GetComponent<SpriteRenderer>();

        initialScale = transform.localScale;
        if (initialScale.sqrMagnitude < 1e-6f)
            initialScale = Vector3.one;

        initialEulerZ = transform.eulerAngles.z;
    }

    public override void OnHintTriggered(int hintIndex)
    {
        if (targetRenderer == null) return;

        if (routine != null)
            StopCoroutine(routine);

        routine = StartCoroutine(FadeSpinScaleRoutine());
    }

    IEnumerator FadeSpinScaleRoutine()
    {
        targetRenderer.enabled = true;

        Vector3 fromScale = initialScale * startScaleMultiplier;
        Vector3 toScale = initialScale * endScaleMultiplier;

        Color c = targetRenderer.color;
        c.a = 0f;
        targetRenderer.color = c;
        transform.localScale = fromScale;
        transform.rotation = Quaternion.Euler(0f, 0f, initialEulerZ);

        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / duration);

            float scaleT = EaseOutBack(t, scaleOvershoot);
            float spinT = EaseOutCubic(t);
            float alphaT = Mathf.Clamp01(Mathf.InverseLerp(0f, alphaLead, t));
            alphaT = Mathf.SmoothStep(0f, 1f, alphaT);

            c.a = Mathf.Lerp(0f, 1f, alphaT);
            targetRenderer.color = c;
            transform.localScale = Vector3.LerpUnclamped(fromScale, toScale, scaleT);
            transform.rotation = Quaternion.Euler(0f, 0f, initialEulerZ + spinDegrees * spinT);

            yield return null;
        }

        c.a = 1f;
        targetRenderer.color = c;
        transform.localScale = toScale;
        transform.rotation = Quaternion.Euler(0f, 0f, initialEulerZ + spinDegrees);

        if (visibleHoldSeconds > 0f)
            yield return new WaitForSeconds(visibleHoldSeconds);

        if (fadeOutDuration <= 0f)
        {
            c.a = 0f;
            targetRenderer.color = c;
        }
        else
        {
            elapsed = 0f;
            while (elapsed < fadeOutDuration)
            {
                elapsed += Time.deltaTime;
                float t = Mathf.Clamp01(elapsed / fadeOutDuration);
                float u = Mathf.SmoothStep(0f, 1f, t);
                c.a = Mathf.Lerp(1f, 0f, u);
                targetRenderer.color = c;
                yield return null;
            }

            c.a = 0f;
            targetRenderer.color = c;
        }

        if (disableRendererWhenHidden)
            targetRenderer.enabled = false;

        routine = null;
    }

    /// <summary>Ease-out cubic：旋转略「拖」在后半段，和弹性缩放错开节奏。</summary>
    static float EaseOutCubic(float t)
    {
        if (t >= 1f) return 1f;
        return 1f - Mathf.Pow(1f - t, 3f);
    }

    /// <summary>Ease-out-back：缩放冲过 1 再收回，更有弹性。</summary>
    static float EaseOutBack(float t, float overshoot)
    {
        if (t <= 0f) return 0f;
        if (t >= 1f) return 1f;
        float s = Mathf.Max(0.01f, overshoot);
        t -= 1f;
        return t * t * ((s + 1f) * t + s) + 1f;
    }
}
