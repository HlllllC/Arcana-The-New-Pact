using UnityEngine;

public interface ICardEffect
{
    /// <summary>
    /// 检查是否可以在目标网格使用该卡牌
    /// </summary>
    /// <param name="player">使用卡牌的玩家</param>
    /// <param name="target">目标网格</param>
    /// <returns>是否合法</returns>
    bool CanPlay(Player player, GridCell target);

    /// <summary>
    /// 执行卡牌效果
    /// </summary>
    /// <param name="player">使用卡牌的玩家</param>
    /// <param name="target">目标网格</param>
    void Execute(Player player, GridCell target);
}