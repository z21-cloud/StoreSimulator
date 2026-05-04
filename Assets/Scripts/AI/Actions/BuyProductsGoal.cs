using System;
using System.Collections.Generic;
using StoreSimulator.InteractableObjects;
using UnityEngine;

public class BuyProductsGoal : IGoal
{
    public string Name => "BuyProducts";

    public float GetPriority(MonoBehaviour npc)
    {
        var psychology = npc.GetComponent<NPCPsycho>();
        int hungerLvl = GetCritically(psychology.HungerState);
        int thirstLvl = GetCritically(psychology.ThirstState);
        int worst = Mathf.Max(hungerLvl, thirstLvl);

        return 20f + worst * 25f;
    }

    public bool IsSatisfied(MonoBehaviour npc)
    {
        if (!npc.TryGetComponent(out NPCPsycho psycho)) return true;

        // Цель "купить продукты" удовлетворена ТОЛЬКО когда 
        // потребности исчезли (уже поел/попил где-то или само прошло)
        // Если NPC хочет есть или пить — цель НЕ удовлетворена,
        // независимо от того, пуст ли инвентарь или сброшен ли WantBuyProducts

        bool isHungry = psycho.HungerState != NPCHungerState.Full;
        bool isThirsty = psycho.ThirstState != NPCThirstState.Full;

        return !isHungry && !isThirsty;
    }

    public bool CanPursue(MonoBehaviour npc)
    {
        if (!npc.TryGetComponent(out CustomerController customerController)) return false;
        return customerController.CurrentStore != null && customerController.CurrentStore.IsOpen;
    }

    public List<INPCAction> CreatePlan(MonoBehaviour npc)
    {
        var plan = new List<INPCAction>();
        var customer = npc.GetComponent<CustomerController>();

        var psychology = npc.GetComponent<NPCPsycho>();
        var needs = psychology.GetPriorityNeeds();


        var storages = new List<IStorage>();
        foreach (var need in needs)
        {
            storages = customer.CurrentStore.StorageRegistry.GetStorageByNeeds(need);
        }

        if (storages.Count == 0)
        {
            plan.Add(new MoveTo(customer.CurrentStore.StoreLeavePoint));
            return plan;
        }

        plan.Add(new MoveTo(storages[0].InteractionPoint));
        plan.Add(new TakeItemAction(storages[0]));

        var cashStorage = customer.CurrentStore.CashStorageRegistry.GetRandomCashStorage();
        if (cashStorage != null)
        {
            plan.Add(new MoveTo(cashStorage.InteractionPoint));
            plan.Add(new BuyAtCashierAction(cashStorage));
        }

        plan.Add(new MoveTo(customer.CurrentStore.StoreLeavePoint));

        return plan;
    }

    private int GetCritically(NPCHungerState hungerState) => hungerState switch
    {
        NPCHungerState.Full => 0,
        NPCHungerState.LightHungry => 1,
        NPCHungerState.NormalHungry => 2,
        NPCHungerState.Hungry => 3,
        _ => 0
    };

    private int GetCritically(NPCThirstState thirstState) => thirstState switch
    {
        NPCThirstState.Full => 0,
        NPCThirstState.LightThirst => 1,
        NPCThirstState.NormalThirst => 2,
        NPCThirstState.Thirst => 3,
        _ => 0
    };
}
