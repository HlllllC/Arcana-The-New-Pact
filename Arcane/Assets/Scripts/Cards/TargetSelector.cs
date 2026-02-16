using UnityEngine;
using System.Collections.Generic;

public class TargetSelector : MonoBehaviour
{
    public static TargetSelector Instance { get; private set; }

    private Card currentSelectedCard;
    private Player currentPlayer;
    private List<GridCell> highlightedCells = new List<GridCell>();

    void Awake()
    {
        if (Instance == null) Instance = this;
    }

    void OnEnable()
    {
        // 监听卡牌选中事件（可由CardManager触发）
        // 简单起见，我们让CardManager在SelectCard时调用TargetSelector的OnCardSelected
    }

    public void OnCardSelected(Card card, Player player)
    {
        // 清除之前的高亮
        ClearHighlights();

        currentSelectedCard = card;
        currentPlayer = player;

        // 获取所有网格，检查卡牌效果是否可用
        GridManager grid = GridManager.Instance;
        foreach (var cell in grid.Cells)
        {
            if (card.data.effect.CanPlay(player, cell))
            {
                // 高亮这个格子
                HighlightCell(cell, true);
                highlightedCells.Add(cell);
            }
        }
    }

    void HighlightCell(GridCell cell, bool highlight)
    {
        // 改变网格颜色或加轮廓，这里简单设置材质颜色
        var sr = cell.GetComponent<SpriteRenderer>();
        if (sr != null)
        {
            sr.color = highlight ? Color.yellow : Color.white; // 保存原颜色？这里简化
        }
    }

    void ClearHighlights()
    {
        foreach (var cell in highlightedCells)
        {
            // 恢复原色（根据墙体类型设置）
            GridManager.Instance.UpdateCellColor(cell); // 需要GridManager提供恢复方法
        }
        highlightedCells.Clear();
    }

    // 由GridCell点击事件调用
    public void OnGridCellClicked(GridCell cell)
    {
        if (currentSelectedCard == null || currentPlayer == null) return;

        // 检查目标是否合法
        if (!cell.IsEmpty && cell.currentUnit != null && cell.currentUnit.ownerPlayerId != currentPlayer.playerId)
        {
            // 敌方单位可能作为目标，但CanPlay中已处理
        }

        if (currentSelectedCard.data.effect.CanPlay(currentPlayer, cell))
        {
            // 执行效果
            currentSelectedCard.data.effect.Execute(currentPlayer, cell);
            // 消耗行动力（由Player处理）
            currentPlayer.PlayCard(currentSelectedCard, cell);
            // 清除选中
            ClearHighlights();
            currentSelectedCard = null;
            currentPlayer = null;
        }
        else
        {
            Debug.Log("Invalid target");
        }
    }
}