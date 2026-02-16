using UnityEngine;
using System.Collections.Generic;

public class UnitAura : MonoBehaviour
{
    public Unit owner;
    public AuraEffect auraData;
    private List<Unit> affectedUnits = new List<Unit>();

    public void Init(Unit ownerUnit, AuraEffect effect)
    {
        owner = ownerUnit;
        auraData = effect;
    }

    void Update()
    {
        // 每帧检测（性能可接受范围内），也可以改为每0.5秒或每回合更新
        UpdateAffectedUnits();
    }

    void UpdateAffectedUnits()
    {
        GridManager grid = FindObjectOfType<GridManager>();
        if (grid == null) return;

        // 获取当前所有在光环范围内的单位
        List<Unit> currentInRange = new List<Unit>();
        Vector2Int center = owner.gridPos;
        for (int x = center.x - auraData.range; x <= center.x + auraData.range; x++)
        {
            for (int y = center.y - auraData.range; y <= center.y + auraData.range; y++)
            {
                Vector2Int pos = new Vector2Int(x, y);
                if (ManhattanDistance(center, pos) > auraData.range) continue; // 曼哈顿距离限制
                GridCell cell = grid.GetCell(pos);
                if (cell != null && cell.currentUnit != null && cell.currentUnit != owner)
                {
                    // 检查是否为友方（相同ownerPlayerId）
                    if (cell.currentUnit.ownerPlayerId == owner.ownerPlayerId)
                    {
                        currentInRange.Add(cell.currentUnit);
                    }
                }
            }
        }

        // 找出新进入范围的单位，添加增益
        foreach (var unit in currentInRange)
        {
            if (!affectedUnits.Contains(unit))
            {
                unit.AddAuraBonus(auraData);
                affectedUnits.Add(unit);
            }
        }

        // 找出离开范围的单位，移除增益
        List<Unit> toRemove = new List<Unit>();
        foreach (var unit in affectedUnits)
        {
            if (!currentInRange.Contains(unit))
            {
                unit.RemoveAuraBonus(auraData);
                toRemove.Add(unit);
            }
        }
        foreach (var unit in toRemove)
        {
            affectedUnits.Remove(unit);
        }
    }

    int ManhattanDistance(Vector2Int a, Vector2Int b)
    {
        return Mathf.Abs(a.x - b.x) + Mathf.Abs(a.y - b.y);
    }

    void OnDestroy()
    {
        // 单位销毁时，移除对所有受影响单位的增益
        foreach (var unit in affectedUnits)
        {
            if (unit != null)
                unit.RemoveAuraBonus(auraData);
        }
    }
}