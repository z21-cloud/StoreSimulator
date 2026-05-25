using System.Collections.Generic;
using StoreSimulator.ArtificialIntelligence;
using StoreSimulator.InteractableObjects;
using StoreSimulator.StoreableItems;
using StoreSimulator.StoreManager;
using UnityEngine;

public class ShoppingState : INPCState
{
    private readonly NPCController _ctx;
    
    public ShoppingState(NPCController ctx) => _ctx = ctx;

    public void Enter()
    {
        _ctx.Shelves.Clear();
        _ctx.BoughtItems.Clear();
        _ctx.ItemsToBuy = Random.Range(1, _ctx.BuyPool + 1);

        List<ItemCategory> needs = _ctx.NPCNeeds;
        foreach (ItemCategory need in needs)
        {
            Debug.Log($"[AI: {_ctx.gameObject.name} - ShoppingState] I want to buy...{need.ToString()}!");
            List<IStorage> found = _ctx.CurrentStore.StorageRegistry.GetStorageByNeeds(need);

            if (found != null && found.Count > 0) _ctx.Shelves.Add(found[0]);
        }

        if (_ctx.Shelves == null || _ctx.Shelves.Count == 0)
        {
            Debug.Log($"[AI - {_ctx.gameObject.name} - ShoppingState] Can't find needed shelf. Leaving...");
            // float totalSpent = 0f;
            // PriceReactionType reaction = PriceReactionType.Scam;
            // _ctx.RecordVisit(totalSpent, reaction);
            Leaving();
            return;
        }

        _ctx.StateMachine.SetState(_ctx.MovingToStorage);
    }

    public void Tick()
    {

    }

    private void Leaving()
    {
        _ctx.Movement.SetDestination(_ctx.CurrentStore.StoreLeavePoint.position);
        _ctx.StateMachine.SetState(_ctx.LeavingState);
    }

    public void Exit()
    {
        
    }
}
