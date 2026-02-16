using UnityEngine;
using System.Collections.Generic;

public class Unit : MonoBehaviour
{
    [Header("References")]
    public UnitData data;
    public int ownerPlayerId;           // 0=玩家，1=AI
    public Vector2Int gridPos;          // 当前网格坐标

    [Header("Runtime Stats")]
    public int currentHP;
    public int currentMoveRange;        // 可能受光环影响
    public int currentAttackRange;
    public int currentAttackPower;
    public int currentArmor;
    public int currentSpeed;

    // 事件（用于分数管理器等）
    public static event System.Action<Unit> OnUnitDied;
    public static event System.Action<Unit> OnUnitDamaged;

    private UnitAura auraComponent;      // 如果存在光环
    private List<AuraEffect> activeAuras = new List<AuraEffect>(); // 从其他单位接收的光环

    void Start()
    {
        InitializeFromData();
        // 如果有光环效果，确保有Aura组件
        if (data.auraEffect != null && data.auraEffect.range > 0)
        {
            auraComponent = gameObject.AddComponent<UnitAura>();
            auraComponent.Init(this, data.auraEffect);
        }
    }

    void InitializeFromData()
    {
        currentHP = data.maxHP;
        currentMoveRange = data.moveRange;
        currentAttackRange = data.attackRange;
        currentAttackPower = data.attackPower;
        currentArmor = data.armor;
        currentSpeed = data.speed;
    }

    public void TakeDamage(int damage)
    {
        int netDamage = Mathf.Max(1, damage - currentArmor);
        currentHP -= netDamage;
        OnUnitDamaged?.Invoke(this);
        if (currentHP <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        // 清除网格引用
        GridManager grid = FindObjectOfType<GridManager>(); // 简单获取，实际可使用单例
        if (grid != null)
        {
            var cell = grid.GetCell(gridPos);
            if (cell != null) cell.currentUnit = null;
        }

        // 触发死亡事件
        OnUnitDied?.Invoke(this);

        // 销毁前移除光环影响（如果有光环管理器，通知管理器移除）
        Destroy(gameObject);
    }

    // 移动单位到新坐标（由移动卡牌调用）
    public void MoveTo(Vector2Int newPos)
    {
        GridManager grid = GridManager.Instance;  // 使用单例
        if (grid == null)
        {
            Debug.LogError("GridManager instance not found!");
            return;
        }

        // 获取旧网格，清空引用
        GridCell oldCell = grid.GetCell(gridPos);
        if (oldCell != null && oldCell.currentUnit == this)
        {
            oldCell.currentUnit = null;
        }

        // 获取新网格，设置引用
        GridCell newCell = grid.GetCell(newPos);
        if (newCell != null)
        {
            // 确保新网格为空（应在调用前检查）
            if (newCell.currentUnit != null)
            {
                Debug.LogWarning($"Target cell {newPos} is already occupied by another unit!");
                return;
            }
            newCell.currentUnit = this;
            gridPos = newPos;
            transform.position = new Vector3(newPos.x, newPos.y, 0); // 假设网格单位大小为1
        }
        else
        {
            Debug.LogError($"Invalid grid position: {newPos}");
        }
    }

    // 攻击目标单位
    public void Attack(Unit target)
    {
        if (target == null) return;
        target.TakeDamage(currentAttackPower);
    }

    // 重置每回合属性（例如行动力恢复等，由回合管理器调用）
    public void OnTurnStart()
    {
        // 目前没有每回合重置的属性，可留空
    }

    // 添加光环增益（由光环管理器或Aura组件调用）
    public void AddAuraBonus(AuraEffect aura)
    {
        activeAuras.Add(aura);
        RecalculateStats();
    }

    public void RemoveAuraBonus(AuraEffect aura)
    {
        activeAuras.Remove(aura);
        RecalculateStats();
    }

    void RecalculateStats()
    {
        // 从基础值开始
        currentAttackPower = data.attackPower;
        currentArmor = data.armor;
        currentSpeed = data.speed;
        // 叠加所有光环
        foreach (var aura in activeAuras)
        {
            currentAttackPower += aura.attackBonus;
            currentArmor += aura.armorBonus;
            // 其他属性同理
        }
        // 确保属性不低于0
        currentAttackPower = Mathf.Max(0, currentAttackPower);
        currentArmor = Mathf.Max(0, currentArmor);
    }

    // 用于获取速度总和（回合管理器会调用所有单位）
    public int GetSpeed()
    {
        return currentSpeed;
    }
}