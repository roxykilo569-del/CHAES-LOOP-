using UnityEngine;

/// <summary>
/// 全局阶段：不同阶段可由场景内触发器配合不同的 DialogueData。
/// 请在场景中放一个 GameManager，或挂在常驻物体上。
/// </summary>
public class DialogueStageManager : MonoBehaviour
{
    public static DialogueStageManager Instance { get; private set; }

    [Tooltip("当前阶段（从 0 开始）。对话触发器会用它选择 dialogueByStage 的下标。")]
    [SerializeField] private int currentStage;

    public int CurrentStage
    {
        get => currentStage;
        set => currentStage = value;
    }

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    private void OnDestroy()
    {
        if (Instance == this)
            Instance = null;
    }

    /// <summary>将阶段设为指定值（例如剧情推进时调用）。</summary>
    public void SetStage(int stage)
    {
        currentStage = stage;
    }

    /// <summary>阶段 +1。</summary>
    public void AdvanceStage()
    {
        currentStage++;
    }
}
