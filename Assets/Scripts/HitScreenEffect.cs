using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class HitScreenEffect : MonoBehaviour
{
    public static HitScreenEffect Instance;

    public Volume globalVolume;

    public float normalIntensity = 0.05f;
    public float hitIntensity = 1.0f;
    public float recoverTime = 1.0f;

    private ChromaticAberration chromaticAberration;
    private Coroutine currentRoutine;

    private void Awake()
    {
        Instance = this;

        if (globalVolume == null)
        {
            Debug.LogError("Global Volume 没有拖进 HitScreenEffect");
            return;
        }

        if (!globalVolume.profile.TryGet(out chromaticAberration))
        {
            Debug.LogError("Global Volume 里没有 Chromatic Aberration");
            return;
        }

        Debug.Log("HitScreenEffect 初始化成功");

        chromaticAberration.intensity.value = normalIntensity;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.H))
        {
            Debug.Log("手动测试屏幕打击效果");
            PlayHitEffect();
        }
    }

    public void PlayHitEffect()
    {
        Debug.Log("PlayHitEffect 被调用");

        if (chromaticAberration == null)
        {
            Debug.LogError("Chromatic Aberration 没有获取到");
            return;
        }

        if (currentRoutine != null)
        {
            StopCoroutine(currentRoutine);
        }

        currentRoutine = StartCoroutine(HitEffectRoutine());
    }

    private IEnumerator HitEffectRoutine()
    {
        chromaticAberration.intensity.value = hitIntensity;

        float timer = 0f;

        while (timer < recoverTime)
        {
            timer += Time.deltaTime;

            float t = timer / recoverTime;

            chromaticAberration.intensity.value =
                Mathf.Lerp(hitIntensity, normalIntensity, t);

            yield return null;
        }

        chromaticAberration.intensity.value = normalIntensity;
        currentRoutine = null;
    }
}