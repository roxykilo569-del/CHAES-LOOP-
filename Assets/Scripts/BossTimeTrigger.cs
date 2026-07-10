using UnityEngine;

public class BossGameTimeTrigger : MonoBehaviour
{
    [Header("Trigger Time")]
    public float triggerTime = 65f;

    private float playTimer = 0f;
    private bool triggered = false;

    private void Update()
    {
        if (triggered) return;

        if (GameManager.Instance == null) return;
        if (GameManager.Instance.Phase != GamePhase.Playing) return;

        playTimer += Time.deltaTime;

        if (playTimer >= triggerTime)
        {
            triggered = true;

            if (BossController.Instance != null)
            {
                BossController.Instance.ShowBoss();
            }

            Debug.Log("实际游玩时间到达 65 秒，Boss 出现");
        }
    }
}