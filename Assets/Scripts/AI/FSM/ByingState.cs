using StoreSimulator.ArtificialIntelligence;
using StoreSimulator.StoreManager;
using UnityEngine;

public class BuyingState : INPCState
{
    private readonly NPCController _ctx;

    public BuyingState(NPCController ctx) => _ctx = ctx;

    public void Enter()
    {
        Debug.Log($"[AI - {_ctx.gameObject.name}]: Try to find cash storage");

        if (_ctx.CurrentCashStorage == null)
        {
            Debug.Log($"[AI - {_ctx.gameObject.name} - BuyingState]: Cash Storage is null, waiting...");
            _ctx.WaitingState.SetReturn(_ctx.BuyingState);
            _ctx.StateMachine.SetState(_ctx.WaitingState);
            // _ctx.StateMachine.SetState(_ctx.LeavingState);
        }
        else
        {
            _ctx.Movement.SetDestination(_ctx.CurrentCashStorage.InteractionPoint);
        }
    }

    public void Exit()
    {

    }

    public void Tick()
    {
        Debug.Log($"[AI - {_ctx.gameObject.name} - BuyingState]: Moving to cash storage");

        if (!_ctx.Movement.HasReached) return;

        Debug.Log($"[AI - {_ctx.gameObject.name} - BuyingState]: Try to buy item");

        float totalSpent = _ctx.GetTotalCost(_ctx.BoughtItems);

        while (_ctx.CurrentCashStorage.IsAvailable && _ctx.BoughtItems.Count != 0)
        {
            if (!_ctx.Wallet.CanAfford(PricesManager.Instance.GetPlayerPriceForItem(_ctx.BoughtItems[0].Data)))
            {
                Debug.LogWarning($"[AI - {_ctx.gameObject.name} - BuyingState]: Unexpected drop at cashier - {_ctx.BoughtItems[0].Data.ItemName}. Check HaveEnoughMoney logic.");
                _ctx.HandleDropItem(_ctx.BoughtItems[0]);
                continue;
            }

            Debug.Log($"[AI - {_ctx.gameObject.name} - BuyingState]: Item bought succesfully");
            _ctx.CurrentCashStorage.BuyItem(_ctx.BoughtItems[0], _ctx.Wallet);
            _ctx.Psycho.IncreaseParameters(_ctx.BoughtItems[0].Data.FoodRestore, _ctx.BoughtItems[0].Data.ThirstRestore);
            _ctx.BoughtItems.RemoveAt(0);
        }
        if (_ctx.BoughtItems.Count != 0)
        {
            Debug.Log($"[AI - {_ctx.gameObject.name} - BuyingState]: Waiting player to buy item");
            _ctx.WaitingState.SetReturn(_ctx.BuyingState);
            _ctx.StateMachine.SetState(_ctx.WaitingState);
            return;
        }

        _ctx.RecordVisit(totalSpent);
        _ctx.StateMachine.SetState(_ctx.LeavingState);
    }
}
