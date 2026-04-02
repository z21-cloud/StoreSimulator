using System.Collections.Generic;
using StoreSimulator.StoreableItems;
using UnityEngine;

public class NPCPsycho : MonoBehaviour
{
    [SerializeField] private NPCNeeds needs;

    [Header("Любимые товары NPC")]
    // Заполняешь прямо в Inspector: тащишь нужные категории в список
    [SerializeField] private List<ItemCategory> favouriteItems = new();

    [Header("Вероятность пойти в магазин (по состоянию)")]
    [SerializeField] private float storeProbFull     = 25f;  // сыт/не хочет пить
    [SerializeField] private float storeProbLight    = 25f;  // слегка голоден/жажда
    [SerializeField] private float storeProbNormal   = 65f;  // нормально голоден/жажда
    [SerializeField] private float storeProbCritical = 100f; // очень голоден/жажда

    [Header("Шанс купить нужное (а не любимое) по состоянию")]
    // При Full   = 0%:  NPC всегда берёт любимое
    // При Light  = 25%: в 25% случаев берёт нужное, в 75% — любимое
    // При Normal = 50%: 50 на 50
    // При Critical NPC всегда берёт нужное — это жёстко зашито в логику
    [SerializeField] private float needsChanceLight  = 25f;
    [SerializeField] private float needsChanceNormal = 50f;

    private NPCHungerState _hungerState = NPCHungerState.Full;
    private NPCThirstState _thirstState = NPCThirstState.Full;

    private bool _wantBuyProducts = false;

    // Это свойство читает NPCController — "хочет ли NPC в магазин?"
    public bool WantBuyProducts => _wantBuyProducts;

    // ─────────────────────────────────────────────────────────────────
    // Update: ТОЛЬКО определяем состояние. Никакой логики вероятностей!
    // ─────────────────────────────────────────────────────────────────

    private void Update()
    {
        // Вычисляем новое состояние голода
        NPCHungerState newHunger = needs.Hunger switch
        {
            >= 75f => NPCHungerState.Full,
            >= 50f => NPCHungerState.LightHungry,
            >= 25f => NPCHungerState.NormalHungry,
            _      => NPCHungerState.Hungry
        };

        // Вычисляем новое состояние жажды
        NPCThirstState newThirst = needs.Thirst switch
        {
            >= 75f => NPCThirstState.Full,
            >= 50f => NPCThirstState.LightThirst,
            >= 25f => NPCThirstState.NormalThirst,
            _      => NPCThirstState.Thirst
        };

        // Розыгрыш вероятностей делаем ТОЛЬКО если что-то изменилось
        bool changed = (newHunger != _hungerState) || (newThirst != _thirstState);

        _hungerState = newHunger;
        _thirstState = newThirst;

        if (changed)
        {
            RecalculateProbabilities();
        }
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

        float storeProb = worstLevel switch
        {
            0 => storeProbFull,
            1 => storeProbLight,
            2 => storeProbNormal,
            _ => storeProbCritical
        };

        _wantBuyProducts = Random.Range(0f, 100f) < storeProb;

        Debug.Log($"[AI Psycho] Состояние изменилось. " +
                  $"Голод: {_hungerState}, Жажда: {_thirstState}. " +
                  $"Идёт в магазин: {_wantBuyProducts}");
    }

    // ─────────────────────────────────────────────────────────────────
    // GetPriorityNeeds: вызывается из NPCController когда NPC
    // уже в магазине и хочет понять что именно купить.
    // ─────────────────────────────────────────────────────────────────

    public List<ItemCategory> GetPriorityNeeds()
    {
        int worstLevel = Mathf.Max(GetCriticalityLevel(_hungerState),
                                   GetCriticalityLevel(_thirstState));

        List<ItemCategory> result = new List<ItemCategory>();

        // Критическое состояние — NPC игнорирует любимые товары,
        // берёт только то, что закроет потребность
        if (worstLevel == 3)
        {
            if (_hungerState == NPCHungerState.Hungry)   result.Add(ItemCategory.Food);
            if (_thirstState == NPCThirstState.Thirst)   result.Add(ItemCategory.Drink);

            _wantBuyProducts = false;
            return result;
        }

        // Для остальных состояний: бросаем кубик — нужда или любимое?
        float needsChance = worstLevel switch
        {
            0 => 0f,             // Full: никогда не берёт по нужде — только любимое
            1 => needsChanceLight,
            2 => needsChanceNormal,
            _ => 100f            // Critical обработан выше, это не достигается
        };

        bool buyByNeed = Random.Range(0f, 100f) < needsChance;

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
        NPCHungerState.Full         => 0,
        NPCHungerState.LightHungry  => 1,
        NPCHungerState.NormalHungry => 2,
        NPCHungerState.Hungry       => 3,
        _                           => 0
    };

    private int GetCriticalityLevel(NPCThirstState state) => state switch
    {
        NPCThirstState.Full         => 0,
        NPCThirstState.LightThirst  => 1,
        NPCThirstState.NormalThirst => 2,
        NPCThirstState.Thirst       => 3,
        _                           => 0
    };

    // ─────────────────────────────────────────────────────────────────
    // Вызывается из NPCController после покупки товара
    // ─────────────────────────────────────────────────────────────────

    public void IncreaseParameters(float amountHunger, float amountThirst)
    {
        needs.IncreaseParameters(amountHunger, amountThirst);
    }
}