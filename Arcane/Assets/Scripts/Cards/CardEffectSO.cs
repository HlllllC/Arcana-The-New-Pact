using UnityEngine;

public abstract class CardEffectSO : ScriptableObject, ICardEffect
{
    public abstract bool CanPlay(Player player, GridCell target);
    public abstract void Execute(Player player, GridCell target);
}