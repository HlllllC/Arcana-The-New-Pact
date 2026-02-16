using UnityEngine;

[CreateAssetMenu(fileName = "NewCard", menuName = "Game/CardData")]
public class CardData : ScriptableObject
{
    public string cardName;
    public int cost;                // 消耗行动力
    public Sprite cardImage;
    [TextArea] public string description;
    public CardEffectSO effect;      // 关联的效果
}