using StoreSimulator.ArtificialIntelligence;
using UnityEngine;

public class MovingToCheckout : INPCState
{
    private readonly NPCController _ctx;

    public MovingToCheckout(NPCController ctx) => _ctx = ctx;

    public void Enter()
    {
        // Enter state, 

        Debug.Log($"[AI - {_ctx.gameObject.name}]: Try to find cash storage");

        if (_ctx.CurrentCashStorage != null)
        {
            _ctx.Movement.SetDestination(_ctx.CurrentCashStorage.InteractionPoint);
        }
        else
        {
            _ctx.CurrentCashStorage = _ctx.CurrentStore.CashStorageRegistry.GetRandomCashStorage();

            // if NPC gets CurrentCashStorage and it's null, wait and try later
            if (_ctx.CurrentCashStorage == null)
            {
                Debug.Log($"[AI - {_ctx.gameObject.name} - BuyingState]: Cash Storage is null, waiting...");
                _ctx.StateMachine.SetState(_ctx.MovingToCheckout);
            }
        }
    }

    public void Tick()
    {
        Debug.Log($"[AI - {_ctx.gameObject.name} - MovingState] Moving to {((MonoBehaviour)_ctx.CurrentShelf).gameObject.name}");

        if (!_ctx.Movement.HasReached) return;

        _ctx.StateMachine.SetState(_ctx.BuyingState);
    }

    public void Exit() { }
}
