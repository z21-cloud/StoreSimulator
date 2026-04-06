using UnityEngine;

public class NPCLoyalty : MonoBehaviour
{
    [SerializeField] private NPCMemoryData memory;
    [SerializeField] private PriceReaction priceReaction;
    private float ratio = 0f;

    private const float MIN_VISIT_MODIFIER = 0.2f;
    private const float MAX_VISIT_MODIFIER = 1.5f;
    private const float MIN_PRICE_BONUS = 0f;
    private const float MAX_PRICE_BONUS = 0.3f;

    public float Loyalty => CalculateLoyalty();

    public float GreedRatio(float playerPrice, float marketPrice)
    {
        ratio = playerPrice / marketPrice;
        Debug.Log($"[AI - {gameObject.name}] Loyalty - {playerPrice}, {marketPrice}, ratio {ratio}");
        return ratio;
    }

    private float CalculateLoyalty()
    {
        float loyalty = 50f;
        foreach(var visit in memory.history)
        {
            // int daysAgo = TimeManager.Instance.CurrentDay - visit.dayIndex;
            // float recency = .1f; // Mathf.Exp(-daysAgo * 0.1f); // const number | more visit -> less buff

            switch(visit.reactionType)
            {
                case PriceReactionType.GreatDeal:
                    loyalty += priceReaction.loyaltyChange; // * recency;
                    break;
                case PriceReactionType.Fair:
                    loyalty += priceReaction.loyaltyChange; // * recency;
                    break;
                case PriceReactionType.Expensive:
                    loyalty -= priceReaction.loyaltyChange;
                    break;
                case PriceReactionType.Scam:
                    loyalty -= priceReaction.loyaltyChange;
                    break;
            }
        }

        Debug.Log($"[AI - {gameObject.name}] Loyalty changes - {Mathf.Clamp(loyalty, 0f, 100f)}");

        return Mathf.Clamp(loyalty, 0f, 100f);
    }

    public float GetVisitModifier()
    {
        return Mathf.Lerp(MIN_VISIT_MODIFIER, MAX_VISIT_MODIFIER, Loyalty / 100f);
    }

    public float GetPriceThresholdBonus()
    {
        return Mathf.Lerp(MIN_PRICE_BONUS, MAX_PRICE_BONUS, Loyalty / 100f);
    }
}
