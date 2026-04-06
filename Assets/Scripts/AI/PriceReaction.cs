using System;
using UnityEngine;

[Serializable]
public struct PriceReaction
{
    public float ratioThreshold;
    public float loyaltyChange;
    public bool willBuy;
    public PriceReactionType reactionType;
}
