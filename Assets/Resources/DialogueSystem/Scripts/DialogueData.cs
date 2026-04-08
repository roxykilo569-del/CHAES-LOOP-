using UnityEngine;
using UnityEngine.Serialization;

/// <summary>本句对话由左侧还是右侧角色说出（对应 DialogueData 里的头像与名字）。</summary>
public enum DialogueSpeakerSide
{
    Left,
    Right
}

[System.Serializable]
public class DialogueLine
{
    [Tooltip("说话的是左侧还是右侧角色")]
    [FormerlySerializedAs("speakerIndex")]
    public DialogueSpeakerSide speaker;

    [TextArea(2, 6)]
    public string text;

    [Header("本句事件 ID（可选，代码里订阅 DialogueRuntimeEvents）")]
    [Tooltip("非空时：轮到本句开始时派发（打字开始前）。例：line_open_door")]
    public string lineBeginEventId;

    [Tooltip("非空时：本句已完整显示时派发。例：line_tutorial_done")]
    public string lineCompleteEventId;
}

[CreateAssetMenu(fileName = "Dialogue", menuName = "HorrorGame/对话数据", order = 0)]
public class DialogueData : ScriptableObject
{
    public Sprite portraitLeft;
    public Sprite portraitRight;

    public string nameLeft = "角色A";
    public string nameRight = "角色B";

    public DialogueLine[] lines;

    [Header("整段对话事件 ID（可选，代码里订阅 DialogueRuntimeEvents）")]
    [Tooltip("非空时：StartDialogue 成功后立刻派发。例：dlg_intro_start")]
    public string dialogueStartEventId;

    [Tooltip("非空时：本段对话结束、关闭面板前派发。例：dlg_intro_end")]
    public string dialogueEndEventId;

    [Header("打字音效（可选，覆盖 DialogueUIController 上的默认音效）")]
    [Tooltip("不为空时，本段对话使用此音效作为打字声")]
    public AudioClip typingSoundOverride;
}
