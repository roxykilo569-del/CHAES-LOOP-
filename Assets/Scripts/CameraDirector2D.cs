using System.Collections;
using UnityEngine;

public class CameraDirector2D : MonoBehaviour
{
    public static CameraDirector2D Instance;

    [Header("Debug Test Keys")]
    public bool enableDebugKeys = true;

    [Header("Smooth")]
    public float moveSmooth = 6f;
    public float zoomSmooth = 6f;
    public float rotateSmooth = 6f;

    [Header("Idle Drift")]
    public bool enableIdleDrift = true;
    public float driftStrength = 0.035f;
    public float driftSpeed = 1.4f;

    [Header("Hit Impact")]
    public float hitShakePower = 0.12f;
    public float hitShakeDuration = 0.1f;
    public float hitZoomPunch = 0.12f;
    public float hitRotationPunch = 0.8f;

    [Header("Slide Camera")]
    public Vector2 slideOffset = new Vector2(0.15f, -0.25f);
    public float slideSizeChange = 0.08f;
    public float slideRotation = -0.5f;

    [Header("Boss Camera")]
    public Vector2 bossOffset = new Vector2(1.2f, 0.25f);
    public float bossZoomIn = 0.7f;
    public float bossRotation = 0f;

    [Header("Glitch Camera")]
    public float glitchMovePower = 0.18f;
    public float glitchShakePower = 0.18f;
    public float glitchRotationPower = 1.6f;

    [Header("Jump Camera")]
    public Vector2 jumpOffset = new Vector2(0.05f, 0.18f);
    public float jumpSizeChange = -0.05f;
    public float jumpRotation = 0.3f;
    public float landShakePower = 0.08f;
    public float landShakeDuration = 0.08f;

    private Camera cam;

    private Vector3 normalPosition;
    private float normalSize;
    private float normalRotationZ;

    private Vector3 targetPosition;
    private float targetSize;
    private float targetRotationZ;

    private float shakePower;
    private float shakeTimer;

    private float zoomPunch;
    private float rotationPunch;
    private Vector3 positionPunch;

    private Coroutine sequenceRoutine;

    private void Awake()
    {
        Instance = this;

        cam = GetComponent<Camera>();

        normalPosition = transform.position;
        normalSize = cam.orthographicSize;
        normalRotationZ = transform.eulerAngles.z;

        targetPosition = normalPosition;
        targetSize = normalSize;
        targetRotationZ = normalRotationZ;
    }

    private void Update()
    {
        if (enableDebugKeys)
        {
            DebugKeys();
        }

        Vector3 finalPosition = targetPosition;
        float finalSize = targetSize;
        float finalRotationZ = targetRotationZ;

        // 普通状态轻微漂移
        if (enableIdleDrift)
        {
            float x = Mathf.Sin(Time.time * driftSpeed) * driftStrength;
            float y = Mathf.Cos(Time.time * driftSpeed * 0.7f) * driftStrength;
            finalPosition += new Vector3(x, y, 0f);
        }

        // 镜头震动
        if (shakeTimer > 0f)
        {
            shakeTimer -= Time.deltaTime;

            Vector2 shake = Random.insideUnitCircle * shakePower;
            finalPosition += new Vector3(shake.x, shake.y, 0f);

            shakePower = Mathf.Lerp(shakePower, 0f, Time.deltaTime * 10f);
        }

           

        // 横向抽动
        finalPosition += positionPunch;
        positionPunch = Vector3.Lerp(positionPunch, Vector3.zero, Time.deltaTime * 12f);

        // 拉近冲击
        finalSize -= zoomPunch;
        zoomPunch = Mathf.Lerp(zoomPunch, 0f, Time.deltaTime * 10f);

        // 倾斜冲击
        finalRotationZ += rotationPunch;
        rotationPunch = Mathf.Lerp(rotationPunch, 0f, Time.deltaTime * 8f);

        transform.position = Vector3.Lerp(transform.position, finalPosition, Time.deltaTime * moveSmooth);
        cam.orthographicSize = Mathf.Lerp(cam.orthographicSize, finalSize, Time.deltaTime * zoomSmooth);

        Quaternion targetRotation = Quaternion.Euler(0f, 0f, finalRotationZ);
        transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, Time.deltaTime * rotateSmooth);
    }

    private void DebugKeys()
    {
        // 1：回普通镜头
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            SetNormalCamera();
        }

        // 2：测试打击镜头
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            HitImpact();
        }

        // 3：测试滑铲镜头
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            SlideCamera();
        }

        // 4：结束滑铲镜头
        if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            EndSlideCamera();
        }

        // 5：测试 Boss 入场镜头
        if (Input.GetKeyDown(KeyCode.Alpha5))
        {
            PlayBossIntro();
        }

        // 6：测试故障镜头
        if (Input.GetKeyDown(KeyCode.Alpha6))
        {
            GlitchKick();
        }
        // 7：测试跳跃镜头
        if (Input.GetKeyDown(KeyCode.Alpha7))
        {
            JumpCamera();
        }

        // 8：测试落地镜头
        if (Input.GetKeyDown(KeyCode.Alpha8))
        {
            EndJumpCamera();
        }
    }

    private void SetCameraTarget(Vector3 position, float size, float rotationZ)
    {
        targetPosition = position;
        targetSize = size;
        targetRotationZ = rotationZ;
    }

    public void SetNormalCamera()
    {
        if (sequenceRoutine != null)
        {
            StopCoroutine(sequenceRoutine);
            sequenceRoutine = null;
        }

        SetCameraTarget(normalPosition, normalSize, normalRotationZ);
    }

    public void HitImpact()
    {
        Shake(hitShakePower, hitShakeDuration);
        ZoomPunch(hitZoomPunch);
        RotationPunch(Random.Range(-hitRotationPunch, hitRotationPunch));
    }

    public void SlideCamera()
    {
        Vector3 pos = normalPosition + new Vector3(slideOffset.x, slideOffset.y, 0f);
        float size = normalSize + slideSizeChange;
        float rot = normalRotationZ + slideRotation;

        SetCameraTarget(pos, size, rot);
    }

    public void EndSlideCamera()
    {
        SetNormalCamera();
    }

    public void PlayBossIntro()
    {
        if (sequenceRoutine != null)
        {
            StopCoroutine(sequenceRoutine);
        }

        sequenceRoutine = StartCoroutine(BossIntroRoutine());
    }

    private IEnumerator BossIntroRoutine()
    {
        // 第一步：轻微停顿，像发现异常
        SetCameraTarget(
            normalPosition + new Vector3(0f, 0.15f, 0f),
            normalSize + 0.15f,
            normalRotationZ
        );

        yield return new WaitForSeconds(0.35f);

        // 第二步：向右推进，给 Boss 留位置
        SetCameraTarget(
            normalPosition + new Vector3(bossOffset.x + 0.3f, bossOffset.y, 0f),
            normalSize - bossZoomIn * 0.65f,
            normalRotationZ + 0.5f
        );

        yield return new WaitForSeconds(0.6f);

        // 第三步：强烈冲击
        Shake(0.25f, 0.25f);
        ZoomPunch(0.25f);
        RotationPunch(-2f);

        yield return new WaitForSeconds(0.3f);

        // 第四步：进入 Boss 固定镜头
        SetBossCamera();

        sequenceRoutine = null;
    }

    public void SetBossCamera()
    {
        Vector3 pos = normalPosition + new Vector3(bossOffset.x, bossOffset.y, 0f);
        float size = normalSize - bossZoomIn;
        float rot = normalRotationZ + bossRotation;

        SetCameraTarget(pos, size, rot);
    }

    public void GlitchKick()
    {
        float x = Random.Range(-glitchMovePower, glitchMovePower);
        positionPunch += new Vector3(x, 0f, 0f);

        Shake(glitchShakePower, 0.08f);
        RotationPunch(Random.Range(-glitchRotationPower, glitchRotationPower));
        ZoomPunch(0.06f);
    }

    public void Shake(float power, float duration)
    {
        shakePower = power;
        shakeTimer = duration;
    }

    public void ZoomPunch(float amount)
    {
        zoomPunch = amount;
    }

    public void RotationPunch(float amount)
    {
        rotationPunch = amount;
    }
    public void JumpCamera()
    {
        Vector3 pos = normalPosition + new Vector3(jumpOffset.x, jumpOffset.y, 0f);
        float size = normalSize + jumpSizeChange;
        float rot = normalRotationZ + jumpRotation;

        SetCameraTarget(pos, size, rot);
    }

    public void EndJumpCamera()
    {
        SetNormalCamera();

        Shake(landShakePower, landShakeDuration);
    }
}