using System.Collections.Generic;
using StoreSimulator.StoreableItems;
using UnityEngine;

public class NPCNeeds : MonoBehaviour
{
    [SerializeField] private float hungerThreshold = 20f;
    [SerializeField] private float thirstsThreshold = 20f;
    [SerializeField] private float thirstDecrease = 1f;
    [SerializeField] private float hungerDecrease = 1f;
    [SerializeField] private float decreaseTimer = 1f;

    private float timer = 0f;
    private float hunger = 100f;
    private float thirst = 100f;

    public float Hunger => hunger;
    public float Thirst => thirst;

    private void Start()
    {
        hunger = 100f;
        thirst = 100f;
    }

    private void Update()
    {
        timer += Time.deltaTime;
        if (timer >= decreaseTimer)
        {
            hunger -= hungerDecrease;
            hunger = Mathf.Max(hunger, 0f);

            thirst -= thirstDecrease;
            thirst = Mathf.Max(thirst, 0f);

            timer = 0f;
            // Debug.Log($"[AI] Current Hunger is: {hunger} ; Current Thirst is: {thirst}");
        }
    }

    public List<ItemCategory> GetPriorityNeed()
    {
        List<ItemCategory> activeNeeds = new List<ItemCategory>();

        if (hunger <= hungerThreshold) activeNeeds.Add(ItemCategory.Food);
        if (thirst <= thirstsThreshold) activeNeeds.Add(ItemCategory.Drink);
    
        return activeNeeds;
    }

    public void IncreaseHunger(float amount)
    {
        hunger += amount;
        hunger = Mathf.Min(hunger, 100f);
    }

    public void DecreaseHunger(float amount)
    {
        hunger -= amount;
        hunger = Mathf.Max(hunger, 0f);
    }

    public void IncreaseThirst(float amount)
    {
        thirst += amount;
        thirst = Mathf.Min(thirst, 100f);
    }

    public void DecreaseThirst(float amount)
    {
        thirst -= amount;
        thirst = Mathf.Max(thirst, 0f);
    }
}
