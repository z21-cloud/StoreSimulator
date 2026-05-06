using System.Collections.Generic;
using StoreSimulator.ArtificialIntelligence;
using StoreSimulator.InteractableObjects;
using StoreSimulator.StoreableItems;
using UnityEngine;

public class PickStoreState : INPCState
{
    private readonly NPCController _ctx;

    public PickStoreState(NPCController ctx) => _ctx = ctx;

    public void Enter()
    {
        // Get store and set destination to store enter point
        _ctx.CurrentStore = StoreRegistry.Instance.GetRandomStore();
        _ctx.Movement.SetDestination(_ctx.CurrentStore.StoreEnterPoint.position);

        // Get memory based on store ID
        var memory = NPCMemoryManager.Instance.GetOrCreateMemoryData(_ctx.NpcId, _ctx.CurrentStore.StoreID);
        // Selects memory to NPC
        _ctx.Psycho.GetComponent<NPCLoyalty>().Initialize(memory);
    }

    public void Tick()
    {
        if (!_ctx.Movement.HasReached) return;

        _ctx.StateMachine.SetState(_ctx.MovingToStore);
    }


    public void Exit() { }
}
