using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class BossCriticalErrorEffect : MonoBehaviour
{
    public static BossCriticalErrorEffect Instance;

    [Header("Boss")]
    public Transform bossTransform;

    [Header("Volume")]
    public Volume bossVolume;

    [Header("Overlay")]
    public RawImage glitchOverlay;

    [Header("Error Text")]
    public TextMeshProUGUI errorText;

    [Header("Effect Settings")]
    public float errorDuration = 0.5f;
    public float bossShakePower = 0.25f;
    public float overlayMaxAlpha = 0.65f;

    private ChromaticAberration chromaticAberration;
    private Vignette vignette;

    private Vector3 bossOriginalPosition;
    private Coroutine currentRoutine;

    private void Awake()
    {
        Instance = this;

        if (bossTransform != null)
        {
            bossOriginalPosition = bossTransform.position;
        }

        if (bossVolume != null)
        {
            bossVolume.profile.TryGet(out chromaticAberration);
            bossVolume.profile.TryGet(out vignette);
        }

        if (glitchOverlay != null)
        {
            SetOverlayAlpha(0f);
            glitchOverlay.raycastTarget = false;
        }

        if (errorText != null)
        {
            errorText.alpha = 0f;
        }
    }

    public void PlayCriticalError()
    {
        if (currentRoutine != null)
        {
            StopCoroutine(currentRoutine);
        }

        currentRoutine = StartCoroutine(CriticalErrorRoutine());
    }

    private IEnumerator CriticalErrorRoutine()
    {
        float timer = 0f;

        if (errorText != null)
        {
            errorText.text = "SYSTEM ERROR";
            errorText.alpha = 1f;
        }

        while (timer < errorDuration)
        {
            timer += Time.deltaTime;
            float t = timer / errorDuration;

            // Boss 本体剧烈抖动
            if (bossTransform != null)
            {
                Vector3 randomOffset = new Vector3(
                    Random.Range(-bossShakePower, bossShakePower),
                    Random.Range(-bossShakePower, bossShakePower),
                    0f
                );

                bossTransform.position = bossOriginalPosition + randomOffset;
            }

            // 色差突然变强
            if (chromaticAberration != null)
            {
                chromaticAberration.intensity.value = Random.Range(0.35f, 0.75f);
            }

            // 暗角压迫
            if (vignette != null)
            {
                vignette.intensity.value = Random.Range(0.35f, 0.6f);
            }

            // 故障 Overlay 闪烁
            if (glitchOverlay != null)
            {
                float flash = Random.Range(0.15f, overlayMaxAlpha);
                SetOverlayAlpha(flash);

                Rect uv = glitchOverlay.uvRect;
                uv.x += Random.Range(-0.08f, 0.08f);
                uv.y += Random.Range(-0.04f, 0.04f);
                glitchOverlay.uvRect = uv;
            }

            // ERROR 文本抖动
            if (errorText != null)
            {
                errorText.alpha = Random.Range(0.55f, 1f);

                errorText.transform.localPosition = new Vector3(
                    Random.Range(-12f, 12f),
                    Random.Range(-8f, 8f),
                    0f
                );
            }

            // 镜头抽动
            if (CameraDirector2D.Instance != null)
            {
                CameraDirector2D.Instance.GlitchKick();
            }

            yield return new WaitForSeconds(0.04f);
        }

        // 恢复 Boss 位置
        if (bossTransform != null)
        {
            bossTransform.position = bossOriginalPosition;
        }

        // 恢复 Boss 滤镜强度
        if (chromaticAberration != null)
        {
            chromaticAberration.intensity.value = 0.18f;
        }

        if (vignette != null)
        {
            vignette.intensity.value = 0.35f;
        }

        if (glitchOverlay != null)
        {
            SetOverlayAlpha(0f);
        }

        if (errorText != null)
        {
            errorText.alpha = 0f;
            errorText.transform.localPosition = Vector3.zero;
        }

        currentRoutine = null;
    }

    private void SetOverlayAlpha(float alpha)
    {
        if (glitchOverlay == null) return;

        Color c = glitchOverlay.color;
        c.a = alpha;
        glitchOverlay.color = c;
    }
}