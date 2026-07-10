using System.Collections;
using UnityEngine;
using TMPro;

public class RhythmFeedbackUI : MonoBehaviour
{
    public static RhythmFeedbackUI Instance;

    [Header("UI")]
    public TextMeshProUGUI comboText;
    public TextMeshProUGUI judgementText;

    [Header("Judgement Time")]
    public float judgementShowTime = 0.35f;

    [Header("PERFECT Style")]
    public Color perfectColor = new Color(1f, 0.95f, 0.55f, 1f);
    public float perfectScale = 1.45f;
    public float perfectShake = 6f;

    [Header("GOOD Style")]
    public Color goodColor = new Color(0.55f, 0.9f, 1f, 1f);
    public float goodScale = 1.3f;
    public float goodShake = 4f;

    [Header("MISS Style")]
    public Color missColor = new Color(1f, 0.3f, 0.3f, 1f);
    public float missScale = 1.25f;
    public float missShake = 10f;

    [Header("Empty Input X Style")]
    public Color emptyColor = new Color(1f, 1f, 1f, 1f);
    public float emptyScale = 1.1f;
    public float emptyShake = 0f;
    public float emptyShowTime = 0.18f;

    [Header("Combo Effect")]
    public float comboPunchScale = 1.25f;
    public float comboPunchTime = 0.12f;

    private int combo = 0;

    private Coroutine judgementRoutine;
    private Coroutine comboRoutine;

    private Vector3 judgementOriginalScale;
    private Vector3 judgementOriginalPosition;
    private Vector3 comboOriginalScale;

    private struct JudgementStyle
    {
        public Color color;
        public float scale;
        public float shake;
        public float showTime;

        public JudgementStyle(Color color, float scale, float shake, float showTime)
        {
            this.color = color;
            this.scale = scale;
            this.shake = shake;
            this.showTime = showTime;
        }
    }

    private void Awake()
    {
        Instance = this;

        if (judgementText != null)
        {
            judgementOriginalScale = judgementText.transform.localScale;
            judgementOriginalPosition = judgementText.transform.localPosition;
            judgementText.text = "";
            judgementText.alpha = 1f;
        }

        if (comboText != null)
        {
            comboOriginalScale = comboText.transform.localScale;
        }

        UpdateComboText();
    }

    public void ShowJudgement(string result)
    {
        if (result == "PERFECT" || result == "GOOD")
        {
            combo++;
            PlayComboPunch();
        }
        else if (result == "MISS")
        {
            combo = 0;
            PlayComboPunch();
        }

        UpdateComboText();
        PlayJudgementText(result);
    }

    public void ShowEmptyInput()
    {
        // 空按，只显示 X，不影响 Combo，不计分
        PlayJudgementText("X");
    }

    public void ResetCombo()
    {
        combo = 0;
        UpdateComboText();
        PlayComboPunch();
    }

    private void UpdateComboText()
    {
        if (comboText != null)
        {
            // 只显示数字
            comboText.text = combo.ToString();
        }
    }

    private void PlayJudgementText(string text)
    {
        if (judgementText == null) return;

        if (judgementRoutine != null)
        {
            StopCoroutine(judgementRoutine);
        }

        JudgementStyle style = GetJudgementStyle(text);
        judgementRoutine = StartCoroutine(JudgementRoutine(text, style));
    }

    private JudgementStyle GetJudgementStyle(string text)
    {
        switch (text)
        {
            case "PERFECT":
                return new JudgementStyle(
                    perfectColor,
                    perfectScale,
                    perfectShake,
                    judgementShowTime
                );

            case "GOOD":
                return new JudgementStyle(
                    goodColor,
                    goodScale,
                    goodShake,
                    judgementShowTime
                );

            case "MISS":
                return new JudgementStyle(
                    missColor,
                    missScale,
                    missShake,
                    judgementShowTime
                );

            case "X":
                return new JudgementStyle(
                    emptyColor,
                    emptyScale,
                    emptyShake,
                    emptyShowTime
                );

            default:
                return new JudgementStyle(
                    Color.white,
                    1f,
                    0f,
                    judgementShowTime
                );
        }
    }

    private IEnumerator JudgementRoutine(string text, JudgementStyle style)
    {
        judgementText.text = text;
        judgementText.color = style.color;
        judgementText.alpha = 1f;

        judgementText.transform.localScale =
            judgementOriginalScale * style.scale;

        judgementText.transform.localPosition =
            judgementOriginalPosition;

        float timer = 0f;

        while (timer < style.showTime)
        {
            timer += Time.deltaTime;
            float t = timer / style.showTime;

            // 淡出
            judgementText.alpha = Mathf.Lerp(1f, 0f, t);

            // 从放大状态缩回原大小
            judgementText.transform.localScale =
                Vector3.Lerp(
                    judgementOriginalScale * style.scale,
                    judgementOriginalScale,
                    t
                );

            // 抖动逐渐减弱
            float shake = Mathf.Lerp(style.shake, 0f, t);

            Vector3 randomOffset = new Vector3(
                Random.Range(-shake, shake),
                Random.Range(-shake, shake),
                0f
            );

            judgementText.transform.localPosition =
                judgementOriginalPosition + randomOffset;

            yield return null;
        }

        judgementText.text = "";
        judgementText.alpha = 1f;
        judgementText.transform.localScale = judgementOriginalScale;
        judgementText.transform.localPosition = judgementOriginalPosition;

        judgementRoutine = null;
    }

    private void PlayComboPunch()
    {
        if (comboText == null) return;

        if (comboRoutine != null)
        {
            StopCoroutine(comboRoutine);
        }

        comboRoutine = StartCoroutine(ComboPunchRoutine());
    }

    private IEnumerator ComboPunchRoutine()
    {
        float timer = 0f;

        while (timer < comboPunchTime)
        {
            timer += Time.deltaTime;
            float t = timer / comboPunchTime;

            float scale;

            if (t < 0.5f)
            {
                scale = Mathf.Lerp(1f, comboPunchScale, t * 2f);
            }
            else
            {
                scale = Mathf.Lerp(comboPunchScale, 1f, (t - 0.5f) * 2f);
            }

            comboText.transform.localScale =
                comboOriginalScale * scale;

            yield return null;
        }

        comboText.transform.localScale = comboOriginalScale;
        comboRoutine = null;
    }
}