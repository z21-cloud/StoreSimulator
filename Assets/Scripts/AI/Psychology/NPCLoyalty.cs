using UnityEngine;

public class NPCLoyalty : MonoBehaviour
{
    [SerializeField] private NPCPriceReactionSO priceReaction;
    private const float MIN_VISIT_MODIFIER = 0.2f;
    private const float MAX_VISIT_MODIFIER = 1.5f;
    private const float MIN_PRICE_BONUS = 0f;
    private const float MAX_PRICE_BONUS = 0.3f;
    private NPCMemoryData _memory;

    public float Loyalty => CalculateLoyalty();

    public void Initialize(NPCMemoryData activeMemory)
    {
        _memory = activeMemory;
    }

    public float GreedRatio(float playerPrice, float marketPrice)
    {
        float ratio = playerPrice / marketPrice;
        Debug.Log($"[AI - {gameObject.name} - LOYALTY] Prices - {playerPrice}, {marketPrice}, ratio {ratio}");
        return ratio;
    }

    private float CalculateLoyalty()
    {
        float loyalty = 50f;

        foreach(var record in _memory.history)
        {
            foreach(var reaction in priceReaction.priceReactions)
            {
                if(record.reactionType == reaction.reactionType)
                {
                    loyalty += reaction.loyaltyChange;
                    break;
                }
            }
        }

        Debug.Log($"[AI - {gameObject.name} - LOYALTY] Loyalty changes - {Mathf.Clamp(loyalty, 0f, 100f)}");

        return Mathf.Clamp(loyalty, 0f, 100f);
    }

    public float GetVisitModifier()
    {
        float result = Mathf.Lerp(MIN_VISIT_MODIFIER, MAX_VISIT_MODIFIER, Loyalty / 100f);
        Debug.Log($"[AI - {gameObject.name} - LOYALTY] Visit Modifier: {result}");
        return result;
    }

    public float GetPriceThresholdBonus()
    {
        float result = Mathf.Lerp(MIN_PRICE_BONUS, MAX_PRICE_BONUS, Loyalty / 100f);
        Debug.Log($"[AI - {gameObject.name} - LOYALTY] Price Threshold Bonus: {result}");
        return result;
    }
}
