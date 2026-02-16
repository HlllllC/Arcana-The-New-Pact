using UnityEngine;

[CreateAssetMenu(fileName = "SummonUnitEffect", menuName = "Card Effects/Summon Unit")]
public class SummonUnitEffectSO : CardEffectSO
{
    public UnitData unitToSummon;

    public override bool CanPlay(Player player, GridCell target)
    {
        // 目标必须存在且为空地（无单位且非普通墙体）
        return target != null && target.IsEmpty;
    }

    public override void Execute(Player player, GridCell target)
    {
        // 实例化单位
        GameObject unitObj = Instantiate(unitToSummon.prefab, target.transform.position, Quaternion.identity);
        Unit unit = unitObj.GetComponent<Unit>();
        unit.ownerPlayerId = player.playerId;
        unit.gridPos = target.coordinate;
        target.currentUnit = unit;

        // 如果有分数管理器，可以触发得分（召唤单位得分）
        // ScoreManager.Instance?.AddScore(player.playerId, unitToSummon.scoreValue); // 可选

        // 将单位加入玩家的单位列表（Player脚本中维护）
        player.AddUnit(unit);
    }
}