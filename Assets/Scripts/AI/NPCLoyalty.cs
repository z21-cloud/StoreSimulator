using StoreSimulator.InteractableObjects;
using StoreSimulator.StoreManager;
using UnityEngine;

public class NPCLoyalty : MonoBehaviour
{
    [SerializeField] private float loyalty = 50f;

    public float Loyalty => loyalty;

    private float ratio = 0f;

    public float GreedRatio(float playerPrice, float marketPrice)
    {
        ratio = playerPrice / marketPrice;
        Debug.Log($"[AI - {gameObject.name}] Loyalty - {playerPrice}, {marketPrice}, ratio {ratio}");
        return ratio;
    }

    public void IncreaseLoyalty(float amount)
    {
        loyalty += amount;
        loyalty = Mathf.Min(loyalty, 100f);
    }

    public void DecreaseLoyalty(float amount)
    {
        loyalty -= amount;
        loyalty = Mathf.Max(loyalty, 0f);
    }
}
