using UnityEngine;

[CreateAssetMenu(fileName = "NewUnitData", menuName = "Game/UnitData")]
public class UnitData : ScriptableObject
{
    public string unitName = "Unit";
    public int maxHP = 10;
    public int speed = 5;
    public int moveRange = 3;          // 移动力
    public int attackRange = 1;         // 攻击范围（近战为1，远程可设为2-3）
    public int attackPower = 3;
    public int armor = 0;
    public AttackType attackType = AttackType.Physical;
    public int scoreValue = 10;         // 消灭该单位获得的分数
    public GameObject prefab;            // 单位预制体（用于召唤）

    [Header("Aura (Optional)")]
    public AuraEffect auraEffect;        // 光环效果（如果单位有光环）
}

// 攻击类型枚举（可扩展）
public enum AttackType
{
    Physical,
    Magic
}

// 光环效果数据结构（也可单独做成ScriptableObject，这里简单用类）
[System.Serializable]
public class AuraEffect
{
    public int range = 1;                // 光环影响范围（曼哈顿距离）
    public int attackBonus = 0;           // 增加攻击力
    public int armorBonus = 0;            // 增加护甲
    // 可添加其他增益如速度、移动力等
}