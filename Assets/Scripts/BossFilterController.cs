using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;

public class BossFilterController : MonoBehaviour
{
    public static BossFilterController Instance;

    [Header("Volume")]
    public Volume bossVolume;

    [Header("Fade")]
    public float fadeInTime = 0.6f;
    public float fadeOutTime = 0.5f;

    private Coroutine currentRoutine;

    private void Awake()
    {
        Instance = this;

        if (bossVolume != null)
        {
            bossVolume.weight = 0f;
        }
    }

    public void EnterBossFilter()
    {
        FadeTo(1f, fadeInTime);
    }

    public void ExitBossFilter()
    {
        FadeTo(0f, fadeOutTime);
    }

    private void FadeTo(float targetWeight, float duration)
    {
        if (bossVolume == null)
        {
            Debug.LogWarning("BossFilterController 没有绑定 Boss Volume");
            return;
        }

        if (currentRoutine != null)
        {
            StopCoroutine(currentRoutine);
        }

        currentRoutine = StartCoroutine(FadeRoutine(targetWeight, duration));
    }

    private IEnumerator FadeRoutine(float targetWeight, float duration)
    {
        float startWeight = bossVolume.weight;
        float timer = 0f;

        while (timer < duration)
        {
            timer += Time.deltaTime;
            float t = timer / duration;

            bossVolume.weight = Mathf.Lerp(startWeight, targetWeight, t);

            yield return null;
        }

        bossVolume.weight = targetWeight;
        currentRoutine = null;
    }
}