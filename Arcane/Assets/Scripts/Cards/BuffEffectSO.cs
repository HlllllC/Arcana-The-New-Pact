using System.Collections;
using UnityEngine;

[CreateAssetMenu(fileName = "BuffEffect", menuName = "Card Effects/Buff")]
public class BuffEffectSO : CardEffectSO
{
    public int attackBonus = 1;
    public int armorBonus = 1;

    public override bool CanPlay(Player player, GridCell target)
    {
        // 目标必须有友方单位
        return target.currentUnit != null && target.currentUnit.ownerPlayerId == player.playerId;
    }

    public override void Execute(Player player, GridCell target)
    {
        Unit unit = target.currentUnit;
        // 添加临时增益（可以使用类似光环的机制，但这里简单地在Unit上添加一个Buff组件）
        Buff buff = unit.gameObject.AddComponent<Buff>();
        buff.Init(attackBonus, armorBonus, 1); // 持续1回合
    }
}