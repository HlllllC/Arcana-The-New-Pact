using UnityEngine;

[CreateAssetMenu(fileName = "AttackEffect", menuName = "Card Effects/Attack")]
public class AttackEffectSO : CardEffectSO
{
    public override bool CanPlay(Player player, GridCell target)
    {
        if (player.selectedUnit == null) return false;
        Unit attacker = player.selectedUnit;
        // 目标格必须有敌方单位
        if (target.currentUnit == null || target.currentUnit.ownerPlayerId == player.playerId) return false;
        // 检查距离是否在攻击范围内（曼哈顿距离）
        int dist = Mathf.Abs(attacker.gridPos.x - target.coordinate.x) + Mathf.Abs(attacker.gridPos.y - target.coordinate.y);
        return dist <= attacker.currentAttackRange;
    }

    public override void Execute(Player player, GridCell target)
    {
        Unit attacker = player.selectedUnit;
        Unit defender = target.currentUnit;
        attacker.Attack(defender);
        player.selectedUnit = null;
    }
}