using UnityEngine;

public class BossErrorTimeTrigger : MonoBehaviour
{
    [Header("Trigger Times")]
    public float phase2Time = 83f;
    public float secondErrorTime = 135f;

    [Header("Actual Gameplay Time")]
    public bool onlyCountWhenPlaying = true;
    public float gameTimer = 0f;

    [Header("Runtime")]
    public bool phase2Triggered = false;
    public bool secondErrorTriggered = false;

    [Header("Debug")]
    public bool debugLog = true;

    private void Update()
    {
        if (onlyCountWhenPlaying)
        {
            if (GameManager.Instance == null)
            {
                return;
            }

            if (GameManager.Instance.Phase != GamePhase.Playing)
            {
                return;
            }
        }

        gameTimer += Time.deltaTime;

        if (!phase2Triggered && gameTimer >= phase2Time)
        {
            phase2Triggered = true;

            Debug.Log("BossErrorTimeTrigger：到达二阶段时间 " + phase2Time);

            if (BossPhaseController.Instance != null)
            {
                BossPhaseController.Instance.EnterPhase2();
                Debug.Log("BossErrorTimeTrigger：已调用 BossPhaseController.EnterPhase2()");
            }
            else
            {
                Debug.LogError("BossErrorTimeTrigger：找不到 BossPhaseController.Instance");
            }
        }

        if (!secondErrorTriggered && gameTimer >= secondErrorTime)
        {
            secondErrorTriggered = true;

            Debug.Log("BossErrorTimeTrigger：到达第二次 ERROR 时间 " + secondErrorTime);

            if (BossCriticalErrorEffect.Instance != null)
            {
                BossCriticalErrorEffect.Instance.PlayCriticalError();
                Debug.Log("BossErrorTimeTrigger：已调用 BossCriticalErrorEffect.PlayCriticalError()");
            }
            else
            {
                Debug.LogError("BossErrorTimeTrigger：找不到 BossCriticalErrorEffect.Instance");
            }
        }
    }

    public void ResetTrigger()
    {
        gameTimer = 0f;
        phase2Triggered = false;
        secondErrorTriggered = false;
    }
}