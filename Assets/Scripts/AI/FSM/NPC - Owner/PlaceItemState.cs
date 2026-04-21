using StoreSimulator.ArtificialIntelligence;
using StoreSimulator.InteractableObjects;
using UnityEngine;

public class PlaceItemState : INPCState
{
    private readonly NPCOwner _ctx;

    public PlaceItemState(NPCOwner ctx) => _ctx = ctx;

    public void Enter()
    {
        _ctx.Movement.SetDestination(_ctx.CurrentShelf.InteractionPoint);
    }

    public void Exit()
    {
        
    }

    public void Tick()
    {
        if (_ctx.Movement.HasReached && _ctx.BoxStorage.CanTakeItem())
        {
            var peekedGO = _ctx.BoxStorage.PeekItem();
            if (peekedGO.TryGetComponent<IStoreable>(out var storeable))
            {
                while (_ctx.BoxStorage.CanTakeItem() && _ctx.CurrentShelf.CanPlaceItem(storeable))
                {
                    GameObject taken = _ctx.BoxStorage.TakeItem(_ctx.PickUpPoint.position);
                    _ctx.CurrentShelf.PlaceItem(taken);

                    // // Delay between actions
                    _ctx.WaitingOwnerState.SetReturn(this);
                    _ctx.StateMachine.SetState(_ctx.WaitingOwnerState);
                }
            }
        }

        if (!_ctx.BoxStorage.CanTakeItem() || 
            !_ctx.CurrentShelf.CanPlaceItem(_ctx.BoxStorage.PeekItem()?.GetComponent<IStoreable>()))
        {
            // Need to release box storage, because it enables
            Vector3 impulseTemp = new Vector3(1f, 1f, 1f);
            _ctx.BoxHoldable.Release(impulseTemp);

            _ctx.BoxPooling.ReturnBoxStorage(_ctx.BoxDeliverable);
            _ctx.ResetBox();

            _ctx.StateMachine.SetState(_ctx.CheckCashBoxState);
        }
    }
}
