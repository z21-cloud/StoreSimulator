using StoreSimulator.ArtificialIntelligence;
using UnityEngine;

public class LeavingState : INPCState
{
    private readonly NPCController _ctx;

    public LeavingState(NPCController ctx) => _ctx = ctx;

    public void Enter()
    {
        Debug.Log($"[AI - {_ctx.gameObject.name}]: Leaving store");

        _ctx.Psycho.ResetReaction();

        _ctx.CurrentShelf = null;
        _ctx.CurrentCashStorage = null;
        _ctx.BoughtItems.Clear();
    }

    public void Exit()
    {
        
    }

    public void Tick()
    {
        if (_ctx.Movement.HasReached)
        {
            _ctx.StateMachine.SetState(_ctx.IdleState);
        }
    }
}
