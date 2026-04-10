using StoreSimulator.ArtificialIntelligence;
using UnityEngine;

public class MovingState : INPCState
{
    private readonly NPCController _ctx;

    public MovingState(NPCController ctx) => _ctx = ctx;

    public void Enter()
    {
        _ctx.Movement.SetDestination(_ctx.CurrentShelf.InteractionPoint);
    }

    public void Tick()
    {
        Debug.Log($"[AI - {_ctx.gameObject.name}] Moving to {((MonoBehaviour)_ctx.CurrentShelf).gameObject.name}");

        if(_ctx.Movement.HasReached)
            _ctx.StateMachine.SetState(_ctx.TakingState);
    }

    public void Exit() { }
}
