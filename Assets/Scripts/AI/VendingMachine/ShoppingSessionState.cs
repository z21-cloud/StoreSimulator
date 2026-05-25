using StoreSimulator.ArtificialIntelligence;
using UnityEngine;

public class ShoppingSessionState : INPCState
{
    private readonly NPCController _ctx;
    private IShoppingSession _session;

    public ShoppingSessionState(NPCController ctx) => _ctx = ctx;

    public void Enter()
    {
        _session = _ctx.CurrentProvider.StartSession(_ctx.NPCNeeds, _ctx.Wallet, _ctx.PickUpPoint);

        if (_session == null || _session.IsComplete)
        {
            Leave();
            return;
        }

        _ctx.Movement.SetDestination(_session.CurrentDestination);
    }

    public void Tick()
    {
        if (_session.IsComplete)
        {
            Leave();
            return;
        }

        if (!_ctx.Movement.HasReached)
        {
            _ctx.Movement.SetDestination(_session.CurrentDestination);
            return;
        }

        _session.OnArrived(_ctx);

        if (_session.CanAdvance)
        {
            _session.Advance();

            if (_session.IsComplete)
            {
                _ctx.Movement.SetDestination(_session.CurrentDestination);
            }
        }
    }

    private void Leave()
    {
        _ctx.LastSession = _session;
        _ctx.Movement.SetDestination(_ctx.CurrentProvider.LeavePoint.position);
        _ctx.StateMachine.SetState(_ctx.LeavingState);
    }

    public void Exit() { }
}
