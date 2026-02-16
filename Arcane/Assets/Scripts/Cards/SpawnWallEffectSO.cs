using UnityEngine;

[CreateAssetMenu(fileName = "SpawnWallEffect", menuName = "Card Effects/Spawn Wall")]
public class SpawnWallEffectSO : CardEffectSO
{
    public WallType wallType = WallType.Glass; // 默认玻璃，可配置

    public override bool CanPlay(Player player, GridCell target)
    {
        // 目标必须为空地且无单位
        return target != null && target.currentUnit == null && target.wallType == WallType.None;
    }

    public override void Execute(Player player, GridCell target)
    {
        target.SetWallType(wallType);
        // 如果是临时墙体，可以记录并在若干回合后消失（可选）
    }
}