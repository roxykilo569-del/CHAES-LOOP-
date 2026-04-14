/// <summary>
/// 游玩子阶段（仅在 <see cref="GamePhase.Playing"/> 下有意义）。通过 <see cref="GameManager.SetGamePlayPhase"/> 切换。
/// </summary>
public enum GamePlayPhase
{
    /// <summary>障碍碰撞会触发受伤/死亡逻辑。</summary>
    Normal,
    /// <summary>障碍碰撞视为攻击，不受伤。</summary>
    Boss
}
