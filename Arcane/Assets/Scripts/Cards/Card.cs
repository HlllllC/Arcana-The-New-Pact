using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Card : MonoBehaviour
{
    public CardData data;
    public Image cardImage;
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI costText;
    public TextMeshProUGUI descriptionText;

    public void Initialize(CardData cardData)
    {
        data = cardData;
        if (cardImage != null) cardImage.sprite = cardData.cardImage;
        if (nameText != null) nameText.text = cardData.cardName;
        if (costText != null) costText.text = cardData.cost.ToString();
        if (descriptionText != null) descriptionText.text = cardData.description;
    }

    // 卡牌被点击时调用（由手牌UI管理）
    public void OnCardClicked()
    {
        // 通知CardManager当前选中卡牌
        CardManager.Instance.SelectCard(this);
    }
}