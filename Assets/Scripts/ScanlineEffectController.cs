using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class ScanlineEffectController : MonoBehaviour
{
    public static ScanlineEffectController Instance;

    [Header("Target")]
    public RawImage rawImage;

    [Header("Normal State")]
    public float normalAlpha = 0.18f;
    public float normalScrollSpeed = 0.02f;

    [Header("Boss State")]
    public float bossAlpha = 0.45f;
    public float bossScrollSpeed = 0.08f;

    [Header("Wave")]
    public bool enableWave = true;
    public float waveStrength = 0.02f;
    public float waveSpeed = 2f;

    [Header("Glitch Flicker")]
    public bool enableGlitch = true;
    public float glitchChance = 0.03f;
    public float glitchAlpha = 0.65f;
    public float glitchDuration = 0.05f;

    private Rect uvRect;
    private Coroutine glitchRoutine;

    private float currentAlpha;
    private float currentScrollSpeed;

    private void Awake()
    {
        Instance = this;

        if (rawImage == null)
        {
            rawImage = GetComponent<RawImage>();
        }

        uvRect = rawImage.uvRect;

        currentAlpha = normalAlpha;
        currentScrollSpeed = normalScrollSpeed;

        SetAlpha(currentAlpha);
    }

    private void Update()
    {
        ScrollScanline();

        if (enableGlitch)
        {
            TryGlitch();
        }
    }

    private void ScrollScanline()
    {
        uvRect.y -= currentScrollSpeed * Time.deltaTime;

        if (enableWave)
        {
            uvRect.x = Mathf.Sin(Time.time * waveSpeed) * waveStrength;
        }

        rawImage.uvRect = uvRect;
    }

    private void TryGlitch()
    {
        if (glitchRoutine != null) return;

        if (Random.value < glitchChance * Time.deltaTime)
        {
            glitchRoutine = StartCoroutine(GlitchRoutine());
        }
    }

    private IEnumerator GlitchRoutine()
    {
        SetAlpha(glitchAlpha);

        float oldX = uvRect.x;
        uvRect.x += Random.Range(-0.05f, 0.05f);
        rawImage.uvRect = uvRect;

        yield return new WaitForSeconds(glitchDuration);

        uvRect.x = oldX;
        rawImage.uvRect = uvRect;

        SetAlpha(currentAlpha);

        glitchRoutine = null;
    }

    public void SetNormal()
    {
        currentAlpha = normalAlpha;
        currentScrollSpeed = normalScrollSpeed;
        SetAlpha(currentAlpha);
    }

    public void SetBossMode()
    {
        currentAlpha = bossAlpha;
        currentScrollSpeed = bossScrollSpeed;
        SetAlpha(currentAlpha);
    }

    public void PulseStrong(float alpha = 0.7f, float duration = 0.12f)
    {
        StartCoroutine(PulseRoutine(alpha, duration));
    }

    private IEnumerator PulseRoutine(float alpha, float duration)
    {
        SetAlpha(alpha);

        yield return new WaitForSeconds(duration);

        SetAlpha(currentAlpha);
    }

    private void SetAlpha(float alpha)
    {
        Color c = rawImage.color;
        c.a = alpha;
        rawImage.color = c;
    }
}