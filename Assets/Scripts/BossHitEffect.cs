using System.Collections;
using UnityEngine;

public class BossHitEffect : MonoBehaviour
{
    public static BossHitEffect Instance;

    [Header("Boss")]
    public Transform bossTransform;
    public SpriteRenderer bossRenderer;

    [Header("Particle Prefab")]
    public GameObject hitParticlePrefab;
    public Vector3 particleOffset = Vector3.zero;
    public float particleDestroyTime = 2f;

    [Header("Flash")]
    public Color hitColor = Color.red;
    public float flashTime = 0.08f;

    [Header("Shake")]
    public float shakePower = 0.12f;
    public float shakeTime = 0.12f;

    [Header("Debug")]
    public bool debugKey = true;

    private Color originalColor;
    private Coroutine flashRoutine;
    private Coroutine shakeRoutine;

    private void Awake()
    {
        Instance = this;

        if (bossTransform == null)
        {
            bossTransform = transform;
        }

        if (bossRenderer == null && bossTransform != null)
        {
            bossRenderer = bossTransform.GetComponent<SpriteRenderer>();
        }

        if (bossRenderer != null)
        {
            originalColor = bossRenderer.color;
        }

        if (hitParticlePrefab == null)
        {
            Debug.LogWarning("BossHitEffect：Hit Particle Prefab 没有绑定");
        }
    }

    private void Update()
    {
        if (!debugKey) return;

        if (Input.GetKeyDown(KeyCode.H))
        {
            PlayHitEffect();
        }
    }

    public void PlayHitEffect()
    {
        Debug.Log("Boss 受击特效触发");

        SpawnParticle();
        FlashBoss();
        ShakeBoss();

        if (CameraDirector2D.Instance != null)
        {
            CameraDirector2D.Instance.HitImpact();
        }
    }

    private void SpawnParticle()
    {
        if (hitParticlePrefab == null)
        {
            Debug.LogWarning("BossHitEffect：没有 Hit Particle Prefab，所以不能生成粒子");
            return;
        }

        Vector3 spawnPosition = transform.position + particleOffset;

        if (bossTransform != null)
        {
            spawnPosition = bossTransform.position + particleOffset;
        }

        GameObject particleObject = Instantiate(
            hitParticlePrefab,
            spawnPosition,
            Quaternion.identity
        );

        particleObject.SetActive(true);

        ParticleSystem[] particleSystems =
            particleObject.GetComponentsInChildren<ParticleSystem>(true);

        foreach (ParticleSystem ps in particleSystems)
        {
            ps.gameObject.SetActive(true);
            ps.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
            ps.Clear(true);
            ps.Play(true);
        }

        Destroy(particleObject, particleDestroyTime);

        Debug.Log("Boss 受击粒子生成：" + particleObject.name);
    }

    private void FlashBoss()
    {
        if (bossRenderer == null) return;

        if (flashRoutine != null)
        {
            StopCoroutine(flashRoutine);
        }

        flashRoutine = StartCoroutine(FlashRoutine());
    }

    private IEnumerator FlashRoutine()
    {
        bossRenderer.color = hitColor;

        yield return new WaitForSeconds(flashTime);

        bossRenderer.color = originalColor;

        flashRoutine = null;
    }

    private void ShakeBoss()
    {
        if (bossTransform == null) return;

        if (shakeRoutine != null)
        {
            StopCoroutine(shakeRoutine);
        }

        shakeRoutine = StartCoroutine(ShakeRoutine());
    }

    private IEnumerator ShakeRoutine()
    {
        float timer = 0f;
        Vector3 startPosition = bossTransform.position;

        while (timer < shakeTime)
        {
            timer += Time.deltaTime;

            Vector3 offset = new Vector3(
                Random.Range(-shakePower, shakePower),
                Random.Range(-shakePower, shakePower),
                0f
            );

            bossTransform.position = startPosition + offset;

            yield return null;
        }

        bossTransform.position = startPosition;

        shakeRoutine = null;
    }
}