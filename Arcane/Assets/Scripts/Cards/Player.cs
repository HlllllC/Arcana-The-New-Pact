using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public int playerId;
    public int currentActionPoints;
    public int maxActionPoints = 10;
    public List<Unit> units = new List<Unit>();        // 场上单位
    public Unit selectedUnit;                          // 当前选中的单位

    public void AddUnit(Unit unit)
    {
        units.Add(unit);
    }

    public void RemoveUnit(Unit unit)
    {
        units.Remove(unit);
    }

    public bool CanPayCost(int cost)
    {
        return currentActionPoints >= cost;
    }

    public void PayCost(int cost)
    {
        currentActionPoints -= cost;
    }

    // 使用卡牌（由TargetSelector调用）
    public void PlayCard(Card card, GridCell target)
    {
        if (!CanPayCost(card.data.cost)) return;
        PayCost(card.data.cost);
        CardManager.Instance.PlayCard(card, target); // 移出手牌并弃牌
    }

    // 获取所有单位速度总和
    public int GetTotalSpeed()
    {
        int total = 0;
        foreach (var unit in units)
            total += unit.currentSpeed;
        return total;
    }

    // 回合开始
    public void StartTurn()
    {
        currentActionPoints = maxActionPoints;
        // 抽卡
        CardManager.Instance.DrawCards(5); // 假设每回合抽5张
        // 重置单位行动状态？可暂不处理
    }
}