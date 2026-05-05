using StoreSimulator.ArtificialIntelligence;
using UnityEngine;

public class MovingStateToStore : INPCState
{
    private readonly NPCController _ctx;

    public MovingStateToStore(NPCController ctx) => _ctx = ctx;

    public void Enter()
    {
        
    }

    public void Tick()
    {
        if(!_ctx.Movement.HasReached) return;

        if(!_ctx.CurrentStore.IsOpen)
        {
            Leaving();
            return;
        }

        _ctx.StateMachine.SetState(_ctx.ShoppingState);
    }

    private void Leaving()
    {
        _ctx.Movement.SetDestination(_ctx.CurrentStore.StoreLeavePoint.position);
        _ctx.StateMachine.SetState(_ctx.LeavingState);
    }

    public void Exit() { }
}
