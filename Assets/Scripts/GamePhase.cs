/// <summary>
/// 游戏流程阶段。通过 GameManager.SetPhase / SetGamePhase 切换。
/// </summary>
public enum GamePhase
{
    /// <summary>开局/倒计时/未正式开始，障碍不移动、生成器不刷怪（可接 UI 后再 SetPhase(Playing)）。</summary>
    Preparation,
    /// <summary>正式游玩：障碍移动、生成器工作。</summary>
    Playing,
    /// <summary>暂停（如暂停菜单）。</summary>
    Paused,
    /// <summary>死亡结算（弹窗等）。</summary>
    Dead
}
