using System.Collections;
using UnityEngine;

public class IntroSequenceController : MonoBehaviour
{
    // 在 Inspector 里填入你的对话系统组件
    // public MyDialogueSystem dialogueSystem; 
    public Glitcher glitcher;
    void Start()
    {
        // 游戏一开始，启动这个开场序列协程
        StartCoroutine(IntroSequence());
    }

    IEnumerator IntroSequence()
    {
        // 1. 在这里，你的通用屏幕特效会自己播放，我们不需要管它
        glitcher.GlitchOut();
        // 2. 让这个脚本原地等待 1.2 秒 
        //（建议比 1 秒稍微多留 0.1~0.2 秒的缓冲，这样视觉上“禅意”的留白感更好，不会太突兀）
        yield return new WaitForSeconds(1.8f);

        // 3. 时间到了！在这里触发你的对话系统
        Debug.Log("特效结束，开始播放对话！");
        //GameManager.Instance.SetPrepareState();
    }
}