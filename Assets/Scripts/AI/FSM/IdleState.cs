using System.Collections.Generic;
using StoreSimulator.ArtificialIntelligence;
using StoreSimulator.InteractableObjects;
using StoreSimulator.StoreableItems;
using UnityEngine;

public class IdleState : INPCState
{
    private readonly NPCController _ctx;

    public IdleState(NPCController ctx) => _ctx = ctx;

    public void Enter()
    {
        _ctx.Movement.SetDestination(_ctx.StoreEnterPoint.position);
    }

    public void Tick()
    {
        if (!_ctx.CheckStoreState()) { Leaving(); return; }
        if (!_ctx.Psycho.WantBuyProducts) { Leaving(); return; }

        if (!_ctx.Movement.HasReached) return;

        List<ItemCategory> npcNeeds = _ctx.Psycho.GetPriorityNeeds();

        if (npcNeeds.Count == 0)
        {
            Leaving();
            return;
        }

        _ctx.Shelves.Clear();

        foreach (var category in npcNeeds)
        {
            var found = StorageRegistry.Instance.GetStorageByNeeds(category);
            Debug.Log($"[AI: {_ctx.gameObject.name}] I want to buy...{category.ToString()}!");

            if (found == null || found.Count == 0)
            {
                Debug.Log($"[AI - {_ctx.gameObject.name}] Can't find needed shelf. Leaving...");

                float totalSpent = 0f;
                _ctx.RecordVisit(totalSpent);
                Leaving();
                return;
            }

            _ctx.Shelves.Add(found[0]);
        }

        _ctx.CurrentShelf = _ctx.Shelves[0];
        _ctx.ItemsToBuy = Random.Range(1, _ctx.BuyPool + 1);
        _ctx.BoughtItems.Clear();

        _ctx.StateMachine.SetState(_ctx.MovingState);
    }

    private void Leaving()
    {
        _ctx.Movement.SetDestination(_ctx.StoreLeavaPoint.position);
        _ctx.StateMachine.SetState(_ctx.LeavingState);
    }

    public void Exit()
    {

    }
}
