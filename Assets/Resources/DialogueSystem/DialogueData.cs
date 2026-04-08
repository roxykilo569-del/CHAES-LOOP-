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
}

[CreateAssetMenu(fileName = "Dialogue", menuName = "HorrorGame/对话数据", order = 0)]
public class DialogueData : ScriptableObject
{
    public Sprite portraitLeft;
    public Sprite portraitRight;

    public string nameLeft = "角色A";
    public string nameRight = "角色B";

    public DialogueLine[] lines;

    [Header("打字音效（可选，覆盖 DialogueUIController 上的默认音效）")]
    [Tooltip("不为空时，本段对话使用此音效作为打字声")]
    public AudioClip typingSoundOverride;
}
