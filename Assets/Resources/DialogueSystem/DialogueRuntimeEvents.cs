using System;
using UnityEngine;

/// <summary>
/// 对话运行时事件：由 <see cref="DialogueUIController"/> 在适当时机派发，
/// 场景内脚本（如 <see cref="GameManager"/>）在代码里订阅 <see cref="EventRaised"/> 即可。
/// 不要在 ScriptableObject 上绑场景引用，用 eventId 字符串区分逻辑。
/// </summary>
public static class DialogueRuntimeEvents
{
    /// <summary>
    /// eventId：在 DialogueData / DialogueLine 里配置的字符串；空则不会派发。
    /// context：当前对话资源（可为 null 则不应发生）。
    /// lineIndex：整段开始/结束为 -1；某一句为 0..n-1。
    /// </summary>
    public static event Action<string, DialogueData, int> EventRaised;

    public static void Raise(string eventId, DialogueData context, int lineIndex)
    {
        if (string.IsNullOrEmpty(eventId))
            return;
        EventRaised?.Invoke(eventId, context, lineIndex);
    }
}
