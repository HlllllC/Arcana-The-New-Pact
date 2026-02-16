using UnityEngine;

public class Buff : MonoBehaviour
{
    private int attackBonus;
    private int armorBonus;
    private int duration; // 剩余回合数

    public void Init(int atk, int arm, int dur)
    {
        attackBonus = atk;
        armorBonus = arm;
        duration = dur;
        Apply();
    }

    void Apply()
    {
        Unit unit = GetComponent<Unit>();
        if (unit != null)
        {
            unit.currentAttackPower += attackBonus;
            unit.currentArmor += armorBonus;
        }
    }

    void Remove()
    {
        Unit unit = GetComponent<Unit>();
        if (unit != null)
        {
            unit.currentAttackPower -= attackBonus;
            unit.currentArmor -= armorBonus;
        }
    }

    // 由回合管理器每回合结束时调用（或者Unit自己监听回合结束事件）
    public void OnTurnEnd()
    {
        duration--;
        if (duration <= 0)
        {
            Remove();
            Destroy(this);
        }
    }
}