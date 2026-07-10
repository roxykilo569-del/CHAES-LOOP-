using System.Collections;
using UnityEngine;

public class BossPhaseController : MonoBehaviour
{
    public static BossPhaseController Instance;

    [Header("Boss Sprite Renderer")]
    public SpriteRenderer bossRenderer;

    [Header("Boss Sprites")]
    public Sprite phase1Sprite;
    public Sprite phase2Sprite;

    [Header("Phase 2")]
    public bool isPhase2 = false;
    public float switchDelayAfterError = 0.5f;

    [Header("Debug")]
    public bool debugKey = true;
    public KeyCode debugEnterPhase2Key = KeyCode.P;
    public KeyCode debugResetKey = KeyCode.O;

    private Coroutine phaseRoutine;

    private void Awake()
    {
        Instance = this;

        if (bossRenderer == null)
        {
            bossRenderer = GetComponent<SpriteRenderer>();
        }

        if (bossRenderer != null && phase1Sprite == null)
        {
            phase1Sprite = bossRenderer.sprite;
        }

        Debug.Log("BossPhaseController Awake");
    }

    private void Update()
    {
        if (!debugKey) return;

        if (Input.GetKeyDown(debugEnterPhase2Key))
        {
            Debug.Log("手动测试：进入 Boss 二阶段");
            EnterPhase2();
        }

        if (Input.GetKeyDown(debugResetKey))
        {
            Debug.Log("手动测试：恢复 Boss 一阶段");
            ResetToPhase1();
        }
    }

    public void EnterPhase2()
    {
        if (isPhase2)
        {
            Debug.Log("Boss 已经是二阶段，不重复切换");
            return;
        }

        isPhase2 = true;

        if (phaseRoutine != null)
        {
            StopCoroutine(phaseRoutine);
        }

        phaseRoutine = StartCoroutine(EnterPhase2Routine());
    }

    private IEnumerator EnterPhase2Routine()
    {
        Debug.Log("Boss 二阶段流程开始");

        if (BossCriticalErrorEffect.Instance != null)
        {
            BossCriticalErrorEffect.Instance.PlayCriticalError();
        }
        else
        {
            Debug.LogWarning("没有找到 BossCriticalErrorEffect.Instance");
        }

        yield return new WaitForSeconds(switchDelayAfterError);

        SwitchToPhase2Sprite();

        phaseRoutine = null;
    }

    private void SwitchToPhase2Sprite()
    {
        if (bossRenderer == null)
        {
            Debug.LogError("BossPhaseController：Boss Renderer 没有绑定");
            return;
        }

        if (phase2Sprite == null)
        {
            Debug.LogError("BossPhaseController：Phase 2 Sprite 没有绑定");
            return;
        }

        bossRenderer.sprite = phase2Sprite;

        Debug.Log("Boss 已切换到二阶段图片：" + phase2Sprite.name);
    }

    public void ResetToPhase1()
    {
        isPhase2 = false;

        if (bossRenderer == null)
        {
            Debug.LogError("BossPhaseController：Boss Renderer 没有绑定，不能恢复一阶段");
            return;
        }

        if (phase1Sprite == null)
        {
            Debug.LogError("BossPhaseController：Phase 1 Sprite 没有绑定，不能恢复一阶段");
            return;
        }

        bossRenderer.sprite = phase1Sprite;

        Debug.Log("Boss 已恢复一阶段图片：" + phase1Sprite.name);
    }
}