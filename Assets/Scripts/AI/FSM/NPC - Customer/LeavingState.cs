using StoreSimulator.ArtificialIntelligence;
using StoreSimulator.InteractableObjects;
using UnityEngine;

public class LeavingState : INPCState
{
    private readonly NPCController _ctx;

    public LeavingState(NPCController ctx) => _ctx = ctx;

    public void Enter()
    {
        /*if (_ctx.BoughtItems.Count > 0)
        {
            float totalSpent = _ctx.GetTotalCost(_ctx.BoughtItems);
            _ctx.RecordVisit(totalSpent);
        }*/
    }

    public void Exit()
    {
        // _ctx.Psycho.ResetReaction();
    }

    public void Tick()
    {
        if (!_ctx.Movement.HasReached) return;
        
        _ctx.CleanUpAfterVisit();
        _ctx.RequestDespawn();
        // _ctx.UtilityPlanner.OnCurrentActionDone();
    }
}
