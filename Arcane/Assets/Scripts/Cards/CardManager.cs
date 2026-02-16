using System.Collections.Generic;
using UnityEngine;

public class CardManager : MonoBehaviour
{
    public static CardManager Instance { get; private set; }

    [Header("Card Pools")]
    public List<CardData> drawPile = new List<CardData>();      // 抽牌堆
    public List<CardData> discardPile = new List<CardData>();   // 弃牌堆
    public List<Card> handCards = new List<Card>();              // 手牌（UI实例）

    [Header("Settings")]
    public int handSize = 5;                // 每回合抽牌数量
    public GameObject cardPrefab;            // 卡牌预制体
    public Transform handTransform;          // 手牌区域的父对象（如Horizontal Layout Group）

    [Header("Current Selection")]
    public Card selectedCard;                 // 当前选中的卡牌（用于使用）

    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    // 初始化卡组（从配置中加载）
    public void InitializeDeck(List<CardData> startingDeck)
    {
        drawPile.Clear();
        discardPile.Clear();
        drawPile.AddRange(startingDeck);
        ShuffleDrawPile();
    }

    // 洗牌
    public void ShuffleDrawPile()
    {
        for (int i = 0; i < drawPile.Count; i++)
        {
            CardData temp = drawPile[i];
            int randomIndex = Random.Range(i, drawPile.Count);
            drawPile[i] = drawPile[randomIndex];
            drawPile[randomIndex] = temp;
        }
    }

    // 抽一张牌到手牌
    public void DrawCard()
    {
        if (drawPile.Count == 0)
        {
            // 如果抽牌堆为空，将弃牌堆洗牌后作为新的抽牌堆
            if (discardPile.Count > 0)
            {
                drawPile.AddRange(discardPile);
                discardPile.Clear();
                ShuffleDrawPile();
            }
            else
            {
                Debug.LogWarning("No cards to draw!");
                return;
            }
        }

        CardData drawnData = drawPile[0];
        drawPile.RemoveAt(0);

        // 创建手牌UI实例
        GameObject cardObj = Instantiate(cardPrefab, handTransform);
        Card card = cardObj.GetComponent<Card>();
        card.Initialize(drawnData);
        handCards.Add(card);
    }

    // 抽多张牌
    public void DrawCards(int count)
    {
        for (int i = 0; i < count; i++)
        {
            DrawCard();
        }
    }

    // 弃掉一张牌（从手牌移除，加入弃牌堆）
    public void DiscardCard(Card card)
    {
        if (handCards.Contains(card))
        {
            handCards.Remove(card);
            discardPile.Add(card.data);
            Destroy(card.gameObject);
        }
    }

    // 使用卡牌（由Player调用）
    public void PlayCard(Card card, GridCell target)
    {
        if (card == null) return;
        // 实际效果执行由Player调用，这里只处理手牌移除和弃牌
        handCards.Remove(card);
        discardPile.Add(card.data);
        Destroy(card.gameObject);
    }

    // 选择卡牌（供Card脚本调用）
    public void SelectCard(Card card)
    {
        // 取消之前的选中（如果有）
        if (selectedCard != null)
        {
            // 可改变UI颜色等
        }
        selectedCard = card;
        // 高亮显示选中卡牌
    }

    // 取消选中
    public void DeselectCard()
    {
        selectedCard = null;
    }
}