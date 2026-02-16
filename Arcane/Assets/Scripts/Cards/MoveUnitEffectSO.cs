using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "MoveUnitEffect", menuName = "Card Effects/Move Unit")]
public class MoveUnitEffectSO : CardEffectSO
{
    public override bool CanPlay(Player player, GridCell target)
    {
        // 必须有选中的单位
        if (player.selectedUnit == null) return false;
        Unit unit = player.selectedUnit;
        // 检查目标是否可通行（允许忽略选中单位自己）
        GridManager grid = GridManager.Instance;
        if (grid == null) return false;
        if (!grid.IsCellPassable(target.coordinate, unit)) return false;
        // 检查路径是否在移动力范围内
        List<Vector2Int> path = grid.FindPath(unit.gridPos, target.coordinate, unit);
        if (path == null || path.Count - 1 > unit.currentMoveRange) return false; // path包含起点，步数为path.Count-1
        return true;
    }

    public override void Execute(Player player, GridCell target)
    {
        Unit unit = player.selectedUnit;
        GridManager grid = GridManager.Instance;
        List<Vector2Int> path = grid.FindPath(unit.gridPos, target.coordinate, unit);
        // 移动单位（实际可调用Unit.MoveTo，但这里需要沿着路径移动？简单起见直接移动到目标格）
        unit.MoveTo(target.coordinate);
        // 清除选中
        player.selectedUnit = null;
    }
}