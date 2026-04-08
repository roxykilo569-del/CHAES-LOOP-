using UnityEngine;

/// <summary>
/// 鼠标点击本物体（需 Collider2D）时，根据 GameManager 的当前阶段
/// 从 dialogueByStage 里取对应 DialogueData 并交给 DialogueUIController 播放。
/// </summary>
[RequireComponent(typeof(Collider2D))]
public class DialogueSpriteClickTrigger : MonoBehaviour
{
    public int CurrentStage;
    [Tooltip("下标 = 阶段：Element 0 对应阶段 0，依此类推。某阶段不需要对话可留空。")]
    [SerializeField] private DialogueData[] dialogueByStage;

    [Tooltip("若已有对话在播放，则忽略本次点击")]
    [SerializeField] private bool skipIfDialogueAlreadyPlaying = true;

    private void OnMouseDown()
    {
        int stage = CurrentStage;
        if (dialogueByStage == null || stage < 0 || stage >= dialogueByStage.Length)
        {
            Debug.LogWarning($"DialogueSpriteClickTrigger: 当前阶段 {stage} 超出 dialogueByStage 配置范围。", this);
            return;
        }

        DialogueData data = dialogueByStage[stage];
        if (data == null)
        {
            Debug.LogWarning($"DialogueSpriteClickTrigger: 阶段 {stage} 未配置 DialogueData。", this);
            return;
        }

        if (DialogueUIController.Instance == null)
        {
            Debug.LogWarning("DialogueSpriteClickTrigger: DialogueUIController 不存在。", this);
            return;
        }

        DialogueUIController.Instance.StartDialogue(data);
    }
}
