using System.Collections.Generic;
using StoreSimulator.ArtificialIntelligence;
using StoreSimulator.StoreableItems;
using StoreSimulator.StoreUtility;
using UnityEngine;

public class IdleState : INPCState
{
    private readonly NPCController _ctx;

    public IdleState(NPCController ctx) => _ctx = ctx;

    private Store _currentStore;

    public void Enter()
    {
        _currentStore = StoreRegistry.Instance.GetRandomStore();
        _ctx.Movement.SetDestination(_currentStore.StoreEnterPoint.position);
    }

    public void Tick()
    {
        if (!_currentStore.IsOpen) { Leaving(); return; }
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
            var found = _currentStore.StorageRegistry.GetStorageByNeeds(category);
            Debug.Log($"[AI: {_ctx.gameObject.name} - IdleState] I want to buy...{category.ToString()}!");

            if (found == null || found.Count == 0)
            {
                Debug.Log($"[AI - {_ctx.gameObject.name} - IdleState] Can't find needed shelf. Leaving...");

                float totalSpent = 0f;
                PriceReactionType reaction = PriceReactionType.Scam;
                _ctx.RecordVisit(totalSpent, reaction);
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
        _ctx.Movement.SetDestination(_currentStore.StoreLeavePoint.position);
        _ctx.StateMachine.SetState(_ctx.LeavingState);
    }

    public void Exit()
    {
        _currentStore = null;
    }
}
