using System.Collections.Generic;
using System.Threading;
using StoreSimulator.StoreableItems;
using UnityEngine;

public class NPCPsycho : MonoBehaviour
{
    [SerializeField] private NPCNeeds needs;
    [SerializeField] private NPCLoyalty loyalty;

    [Header("Configs")]
    [SerializeField] private NPCPriceReactionSO priceReactionConfig;
    [SerializeField] private NPCPsychoConfig psychoConfig;
    [SerializeField] private float checkStoreTimer = 20f;

    [Header("Любимые товары NPC")]
    // Заполняешь прямо в Inspector: тащишь нужные категории в список
    [SerializeField] private List<ItemCategory> favouriteItems = new();

    private NPCHungerState _hungerState = NPCHungerState.Full;
    private NPCThirstState _thirstState = NPCThirstState.Full;

    private float timer = 0f;

    private bool _wantBuyProducts = false;
    private PriceReactionType _lastReaction;

    // Это свойство читает NPCController — "хочет ли NPC в магазин?"
    public bool WantBuyProducts => _wantBuyProducts;

    // ─────────────────────────────────────────────────────────────────
    // Update: ТОЛЬКО определяем состояние. Никакой логики вероятностей!
    // ─────────────────────────────────────────────────────────────────

    private void Update()
    {
        // Вычисляем новое состояние голода
        NPCHungerState newHunger = DetermineHungerState(needs.Hunger);

        // Вычисляем новое состояние жажды
        NPCThirstState newThirst = DetermineThirstState(needs.Thirst);

        // Розыгрыш вероятностей делаем ТОЛЬКО если что-то изменилось
        bool changed = (newHunger != _hungerState) || (newThirst != _thirstState);

        if (_hungerState == NPCHungerState.Hungry || _thirstState == NPCThirstState.Thirst) HandleCheckStoreTimer();

        _hungerState = newHunger;
        _thirstState = newThirst;

        if (changed)
        {
            RecalculateProbabilities();
        }
    }

    private void HandleCheckStoreTimer()
    {
        timer += Time.deltaTime;

        if (timer >= checkStoreTimer)
        {
            RecalculateProbabilities();
            timer = 0f;
        }
    }

    private NPCHungerState DetermineHungerState(float currentValue)
    {
        foreach (var entry in psychoConfig.hungerThreshold)
        {
            if (currentValue >= entry.threshold)
            {
                return (NPCHungerState)entry.criticalityLevel;
            }
        }

        Debug.LogError($"[AI - {gameObject.name}] hungerThreshold is empty");
        return NPCHungerState.Hungry;
    }

    private NPCThirstState DetermineThirstState(float currentValue)
    {
        foreach (var entry in psychoConfig.thirstThreshold)
        {
            if (currentValue >= entry.threshold)
            {
                return (NPCThirstState)entry.criticalityLevel;
            }
        }

        Debug.LogError($"[AI - {gameObject.name}] thirstThreshold is empty");
        return NPCThirstState.Thirst;
    }

    // ─────────────────────────────────────────────────────────────────
    // RecalculateProbabilities: вызывается ОДИН РАЗ при смене состояния.
    // Это исправление главного бага: раньше вызывался из обоих
    // ChangeHungerState и ChangeThirstState и перезаписывал сам себя.
    // ─────────────────────────────────────────────────────────────────

    private void RecalculateProbabilities()
    {
        // Берём наихудший из двух параметров — он определяет срочность
        int worstLevel = Mathf.Max(GetCriticalityLevel(_hungerState),
                                   GetCriticalityLevel(_thirstState));

        var entry = psychoConfig.hungerThreshold.Find(e => e.criticalityLevel == worstLevel);
        float storeProb = entry.storeProbability;
        float loyaltyMod = loyalty.GetVisitModifier();
        float finalProb = storeProb * loyaltyMod;

        _wantBuyProducts = Random.Range(0f, 100f) < finalProb;

        Debug.Log($"[AI Psycho] Состояние изменилось. " +
                  $"Голод: {_hungerState}, Жажда: {_thirstState}. " +
                  $"Идёт в магазин: {_wantBuyProducts}");
    }

    public void ResetReaction() => _lastReaction = PriceReactionType.Fair;

    public List<ItemCategory> GetPriorityNeeds()
    {
        int worstLevel = Mathf.Max(GetCriticalityLevel(_hungerState),
                                   GetCriticalityLevel(_thirstState));

        List<ItemCategory> result = new List<ItemCategory>();

        // Критическое состояние — NPC игнорирует любимые товары,
        // берёт только то, что закроет потребность
        if (worstLevel == 3)
        {
            if (_hungerState == NPCHungerState.Hungry) result.Add(ItemCategory.Food);
            if (_thirstState == NPCThirstState.Thirst) result.Add(ItemCategory.Drink);

            _wantBuyProducts = false;
            return result;
        }

        var entry = psychoConfig.hungerThreshold.Find(e => e.criticalityLevel == worstLevel);
        bool buyByNeed = Random.Range(0f, 100f) < entry.needsChance;

        if (buyByNeed)
        {
            // Добавляем только то, чего реально не хватает
            if (_hungerState != NPCHungerState.Full) result.Add(ItemCategory.Food);
            if (_thirstState != NPCThirstState.Full) result.Add(ItemCategory.Drink);
        }

        // Если нужды нет или кубик показал "любимое" — идём за любимым товаром
        if (result.Count == 0)
        {
            if (favouriteItems.Count > 0)
            {
                ItemCategory favourite = favouriteItems[Random.Range(0, favouriteItems.Count)];
                result.Add(favourite);
            }
            // Если и список любимого пуст — NPC уходит без покупок (result == empty)
        }

        string log = string.Join(", ", result);
        Debug.Log($"[AI Psycho] Хочу купить: {log}");

        _wantBuyProducts = false; // Сбрасываем — NPCController уже прочитал
        return result;
    }

    // ─────────────────────────────────────────────────────────────────
    // Вспомогательный метод: переводит enum в число от 0 до 3.
    // 0 = сыт/не хочет, 3 = критически нуждается.
    // Это нужно чтобы сравнивать голод и жажду между собой.
    // ─────────────────────────────────────────────────────────────────

    private int GetCriticalityLevel(NPCHungerState state) => state switch
    {
        NPCHungerState.Full => 0,
        NPCHungerState.LightHungry => 1,
        NPCHungerState.NormalHungry => 2,
        NPCHungerState.Hungry => 3,
        _ => 0
    };

    private int GetCriticalityLevel(NPCThirstState state) => state switch
    {
        NPCThirstState.Full => 0,
        NPCThirstState.LightThirst => 1,
        NPCThirstState.NormalThirst => 2,
        NPCThirstState.Thirst => 3,
        _ => 0
    };

    // ─────────────────────────────────────────────────────────────────
    // Вызывается из NPCController после покупки товара
    // ─────────────────────────────────────────────────────────────────

    public void IncreaseParameters(float amountHunger, float amountThirst)
    {
        needs.IncreaseParameters(amountHunger, amountThirst);
    }

    public bool BuyItemOrNot(float playerPrice, float marketPrice)
    {
        float ratio = loyalty.GreedRatio(playerPrice, marketPrice);
        float bonus = loyalty.GetPriceThresholdBonus();

        foreach (var reaction in priceReactionConfig.priceReactions)
        {
            float adjustedThreshold = reaction.ratioThreshold + bonus;

            if (ratio >= adjustedThreshold)
            {
                _lastReaction = reaction.reactionType;
                Debug.Log($"[AI - {gameObject.name}] Psycho: reaction is {reaction.reactionType}");
                return reaction.willBuy;
            }
        }

        Debug.LogError($"[AI - {gameObject.name}] Can't find reaction");
        return false;
    }

    public PriceReactionType GetLastReaction()
    {
        return _lastReaction;
    }
}