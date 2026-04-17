using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NPCPriceReactionSO", menuName = "Configs/NPCPriceReactionSO")]
public class NPCPriceReactionSO : ScriptableObject
{
    [Header("Reaction at price")]
    public List<PriceReaction> priceReactions;
}
