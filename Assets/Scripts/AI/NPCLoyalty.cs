using UnityEngine;

public class NPCLoyalty : MonoBehaviour
{
    [SerializeField] private NPCMemoryData memory;
    [SerializeField] private NPCPriceReactionSO priceReaction;
    private float ratio = 0f;

    private const float MIN_VISIT_MODIFIER = 0.2f;
    private const float MAX_VISIT_MODIFIER = 1.5f;
    private const float MIN_PRICE_BONUS = 0f;
    private const float MAX_PRICE_BONUS = 0.3f;

    public float Loyalty => CalculateLoyalty();

    public float GreedRatio(float playerPrice, float marketPrice)
    {
        ratio = playerPrice / marketPrice;
        Debug.Log($"[AI - {gameObject.name}] Prices - {playerPrice}, {marketPrice}, ratio {ratio}");
        return ratio;
    }

    private float CalculateLoyalty()
    {
        float loyalty = 50f;
        
        foreach(var memory in memory.history)
        {
            foreach(var reaction in priceReaction.priceReactions)
            {
                if(memory.reactionType == reaction.reactionType)
                {
                    loyalty += reaction.loyaltyChange;
                    break;
                }
            }
        }

        Debug.Log($"[AI - {gameObject.name}] Loyalty changes - {Mathf.Clamp(loyalty, 0f, 100f)}");

        return Mathf.Clamp(loyalty, 0f, 100f);
    }

    public float GetVisitModifier()
    {
        float result = Mathf.Lerp(MIN_VISIT_MODIFIER, MAX_VISIT_MODIFIER, Loyalty / 100f);
        Debug.Log($"[AI - {gameObject.name}] GetVisitModifier: {result}");
        return result;
    }

    public float GetPriceThresholdBonus()
    {
        float result = Mathf.Lerp(MIN_PRICE_BONUS, MAX_PRICE_BONUS, Loyalty / 100f);
        Debug.Log($"[AI - {gameObject.name}] GetPriceThresholdBonus: {result}");
        return result;
    }
}
