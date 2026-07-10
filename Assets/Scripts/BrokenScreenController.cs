using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class BrokenScreenController : MonoBehaviour
{
    public static BrokenScreenController Instance;

    [Header("Volume")]
    public Volume volume;

    [Header("Overlay")]
    public RawImage glitchOverlay;

    [Header("Normal Broken Feeling")]
    public float normalChromatic = 0.08f;
    public float bossChromatic = 0.22f;

    [Header("Random Glitch")]
    public bool enableRandomGlitch = true;
    public float glitchChance = 0.025f;
    public float glitchDuration = 0.05f;
    public float glitchChromatic = 0.45f;
    public float overlayFlashAlpha = 0.45f;

    private ChromaticAberration chromaticAberration;
    private Coroutine glitchRoutine;

    private float currentBaseChromatic;

    private void Awake()
    {
        Instance = this;

        currentBaseChromatic = normalChromatic;

        if (volume != null)
        {
            volume.profile.TryGet(out chromaticAberration);
        }

        if (chromaticAberration != null)
        {
            chromaticAberration.intensity.value = normalChromatic;
        }

        if (glitchOverlay != null)
        {
            SetOverlayAlpha(0f);
            glitchOverlay.raycastTarget = false;
        }
    }

    private void Update()
    {
        if (!enableRandomGlitch) return;

        if (Random.value < glitchChance * Time.deltaTime)
        {
            PlaySmallGlitch();
        }
    }

    public void SetNormalBroken()
    {
        currentBaseChromatic = normalChromatic;

        if (chromaticAberration != null)
        {
            chromaticAberration.intensity.value = currentBaseChromatic;
        }
    }

    public void SetBossBroken()
    {
        currentBaseChromatic = bossChromatic;

        if (chromaticAberration != null)
        {
            chromaticAberration.intensity.value = currentBaseChromatic;
        }
    }

    public void PlaySmallGlitch()
    {
        if (glitchRoutine != null)
        {
            StopCoroutine(glitchRoutine);
        }

        glitchRoutine = StartCoroutine(GlitchRoutine());
    }

    private IEnumerator GlitchRoutine()
    {
        if (chromaticAberration != null)
        {
            chromaticAberration.intensity.value = glitchChromatic;
        }

        if (glitchOverlay != null)
        {
            SetOverlayAlpha(overlayFlashAlpha);

            Rect uv = glitchOverlay.uvRect;
            uv.x += Random.Range(-0.05f, 0.05f);
            glitchOverlay.uvRect = uv;
        }

        if (CameraDirector2D.Instance != null)
        {
            CameraDirector2D.Instance.GlitchKick();
        }

        yield return new WaitForSeconds(glitchDuration);

        if (chromaticAberration != null)
        {
            chromaticAberration.intensity.value = currentBaseChromatic;
        }

        if (glitchOverlay != null)
        {
            SetOverlayAlpha(0f);
        }

        glitchRoutine = null;
    }

    private void SetOverlayAlpha(float alpha)
    {
        if (glitchOverlay == null) return;

        Color c = glitchOverlay.color;
        c.a = alpha;
        glitchOverlay.color = c;
    }
}