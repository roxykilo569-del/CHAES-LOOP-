using System.Collections;
using UnityEngine;

/// <summary>
/// 挂在提示用 Sprite 上：由 <see cref="HintManager"/> 在预定时刻调用。
/// 推荐<strong>继承</strong>本类写新脚本（如 <see cref="HintVisualFadeSpinScale"/>），不要把各关卡特效都写进基类。
/// 子类重写 <see cref="OnHintTriggered"/>，在其中 <c>StartCoroutine(...)</c> 实现闪烁等效果。
/// </summary>
public class HintVisualBase : MonoBehaviour
{
    /// <param name="hintIndex">本次调度中的第几次触发（0起，按时间顺序含同秒多次）。</param>
    public virtual void OnHintTriggered(int hintIndex)
    {
    }

    /// <summary>占位协程，无逻辑；子类可参考此签名编写特效协程。</summary>
    protected IEnumerator PlaceholderRoutine()
    {
        yield break;
    }
}
