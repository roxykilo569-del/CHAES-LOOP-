using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Easings;
using URPGlitch;
using UnityEngine.Rendering;

public class Glitcher : MonoBehaviour
{
    //Kino.AnalogGlitch glitchAnalog;
    //Kino.DigitalGlitch glitchDigital;

    public Volume volume;
    URPGlitch.Runtime.AnalogGlitch.AnalogGlitchVolume glitchAnalog;
    URPGlitch.Runtime.DigitalGlitch.DigitalGlitchVolume glitchDigital;

    public float scanlineJitter, verticalJump, horizontalShake, colorDrift;
    public float digitalIntensity;

    public float glitchTime;

    void Awake()
    {
        volume.profile.TryGet<URPGlitch.Runtime.AnalogGlitch.AnalogGlitchVolume>(out glitchAnalog);
        volume.profile.TryGet<URPGlitch.Runtime.DigitalGlitch.DigitalGlitchVolume>(out glitchDigital);
    }
    public void GlitchIn()
    {
        StartCoroutine(Glitching(true));
    }
    public void GlitchOut()
    {
        StartCoroutine(Glitching(false));
    }
    IEnumerator Glitching(bool forward)
    {
        glitchAnalog.active = true;
        glitchDigital.active = true;
        float t = (forward) ? 0 : glitchTime;
        bool glitch = true;
        while (glitch)
        {
            t = (forward) ? t + Time.deltaTime : t - Time.deltaTime;
            t = Mathf.Clamp(t, 0f, glitchTime);
            if (forward)
            {
                if (t >= glitchTime)
                {
                    glitch = false;
                }
            }
            else
            {
                if (t <= 0)
                {
                    glitch = false;
                }
            }
            float l = (t / glitchTime);
            glitchAnalog.scanLineJitter.value = Mathf.Lerp(0, scanlineJitter, Ease.SmoothStep(l));
            glitchAnalog.verticalJump.value = Mathf.Lerp(0, verticalJump, Ease.SmoothStep(l));
            glitchAnalog.horizontalShake.value = Mathf.Lerp(0, horizontalShake, Ease.SmoothStep(l));
            glitchAnalog.colorDrift.value = Mathf.Lerp(0, colorDrift, Ease.SmoothStep(l));
            glitchDigital.intensity.value = Mathf.Lerp(0, digitalIntensity, Ease.SmoothStep(l));
            yield return null;
        }

        glitchAnalog.active = false;
        glitchDigital.active = false;
    }

    public void StopGlitch()
    {
        StopAllCoroutines();
    }
}
