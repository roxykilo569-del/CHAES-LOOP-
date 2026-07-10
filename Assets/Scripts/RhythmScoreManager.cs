using System.Collections;
using UnityEngine;
using TMPro;

public class RhythmScoreManager : MonoBehaviour
{
    public static RhythmScoreManager Instance;

    [Header("UI")]
    public TextMeshProUGUI scoreText;

    [Header("Score Settings")]
    public int totalNotes = 50;
    public int maxScore = 100000;
    public float goodRate = 0.6f;

    [Header("Runtime")]
    public int currentScore = 0;
    public int judgedNotes = 0;
    public int perfectCount = 0;
    public int goodCount = 0;
    public int missCount = 0;

    [Header("Score Effect")]
    public float scorePunchScale = 1.15f;
    public float scorePunchTime = 0.1f;

    private Coroutine scoreRoutine;
    private Vector3 scoreOriginalScale;

    private void Awake()
    {
        Instance = this;

        if (scoreText != null)
        {
            scoreOriginalScale = scoreText.transform.localScale;
        }

        UpdateScoreText();

    }

    public void AddJudgement(string result)
    {
        judgedNotes++;

        int perfectScore = GetPerfectScore();
        int goodScore = GetGoodScore();

        if (result == "PERFECT")
        {
            currentScore += perfectScore;
            perfectCount++;
        }
        else if (result == "GOOD")
        {
            currentScore += goodScore;
            goodCount++;
        }
        else if (result == "MISS")
        {
            missCount++;
        }

        if (currentScore > maxScore)
        {
            currentScore = maxScore;
        }

        UpdateScoreText();
        PlayScorePunch();

        Debug.Log(
            "Score: " + currentScore +
            " / Notes: " + judgedNotes + "/" + totalNotes +
            " / PERFECT: " + perfectCount +
            " / GOOD: " + goodCount +
            " / MISS: " + missCount
        );
    }

    private int GetPerfectScore()
    {
        if (totalNotes <= 0) return 0;
        return maxScore / totalNotes;
    }

    private int GetGoodScore()
    {
        return Mathf.RoundToInt(GetPerfectScore() * goodRate);
    }

    private void UpdateScoreText()
    {
        if (scoreText != null)
        {
            scoreText.text = currentScore.ToString("D6");
        }
    }
    private void PlayScorePunch()
    {
        if (scoreText == null) return;

        if (scoreRoutine != null)
        {
            StopCoroutine(scoreRoutine);
        }

        scoreRoutine = StartCoroutine(ScorePunchRoutine());
    }

    private IEnumerator ScorePunchRoutine()
    {
        float timer = 0f;

        while (timer < scorePunchTime)
        {
            timer += Time.deltaTime;
            float t = timer / scorePunchTime;

            float scale;

            if (t < 0.5f)
            {
                scale = Mathf.Lerp(1f, scorePunchScale, t * 2f);
            }
            else
            {
                scale = Mathf.Lerp(scorePunchScale, 1f, (t - 0.5f) * 2f);
            }

            scoreText.transform.localScale = scoreOriginalScale * scale;

            yield return null;
        }

        scoreText.transform.localScale = scoreOriginalScale;
        scoreRoutine = null;
    }
}
