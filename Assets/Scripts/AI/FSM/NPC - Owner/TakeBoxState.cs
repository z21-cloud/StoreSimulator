using StoreSimulator.ArtificialIntelligence;
using UnityEngine;

public class TakeBoxState : INPCState
{
    private readonly NPCOwner _ctx;

    public TakeBoxState(NPCOwner ctx) => _ctx = ctx;

    public void Enter()
    {
        _ctx.Movement.SetDestination(_ctx.Store.DeliveryPoint.position);
    }

    public void Exit()
    {

    }

    public void Tick()
    {
        if (_ctx.Movement.HasReached)
        {
            _ctx.BoxStorage.Hold(_ctx.PickUpPoint);

            // // Delay between actions
            _ctx.WaitingOwnerState.SetReturn(_ctx.PlaceItemState);
            _ctx.StateMachine.SetState(_ctx.WaitingOwnerState);
        }
    }
}
