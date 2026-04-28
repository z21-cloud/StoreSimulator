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

        // Enter state, 

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
                _ctx.WaitingState.SetReturn(_ctx.BuyingState);
                _ctx.StateMachine.SetState(_ctx.WaitingState);
            }
        }
    }

    public void Exit()
    {
        float totalSpent = _ctx.GetTotalCost(_ctx.BoughtItems);
        _ctx.RecordVisit(totalSpent);        
    }

    public void Tick()
    {
        Debug.Log($"[AI - {_ctx.gameObject.name} - BuyingState]: Moving to cash storage");

        // go to cashStorage -> reaches -> try to buy
        if (!_ctx.Movement.HasReached) return;

        /*while (_ctx.CurrentCashStorage.IsAvailable && _ctx.BoughtItems.Count != 0)
        {
            if (!_ctx.Wallet.CanAfford(_ctx.BoughtItems[0].LockedPrice))
            {
                Debug.LogWarning($"[AI - {_ctx.gameObject.name} - BuyingState]: Unexpected drop at cashier - {_ctx.BoughtItems[0].Data.ItemName}. Check HaveEnoughMoney logic.");
                _ctx.HandleDropItem(_ctx.BoughtItems[0]);
                continue;
            }

            Debug.Log($"[AI - {_ctx.gameObject.name} - BuyingState]: Item bought succesfully");
            _ctx.CurrentCashStorage.BuyItem(_ctx.BoughtItems[0], _ctx.Wallet);
            _ctx.Psycho.IncreaseParameters(_ctx.BoughtItems[0].Data.FoodRestore, _ctx.BoughtItems[0].Data.ThirstRestore);
            _ctx.BoughtItems.RemoveAt(0);
        }*/

        // if store-owner stays in cash store zone -> cash storage is available
        if (_ctx.CurrentCashStorage.IsAvailable)
        {
            for (int i = 0; i < _ctx.BoughtItems.Count; i++)
            {
                // if NPC can't buy item -> drop it 
                if (!_ctx.Wallet.CanAfford(_ctx.BoughtItems[i].LockedPrice))
                {
                    HandleDropItem(i);
                    continue;
                }

                // if NPC can afford item -> buy it & increase needs parameters
                HandleBuyItem(i);
            }
        }
        else
        {
            // wait state
            Debug.Log($"[AI - {_ctx.gameObject.name} - BuyingState]: Waiting player to buy item");
            _ctx.WaitingState.SetReturn(_ctx.BuyingState);
            _ctx.StateMachine.SetState(_ctx.WaitingState);
            return;
        }
        
        _ctx.Movement.SetDestination(_ctx.CurrentStore.StoreLeavePoint.position);
        _ctx.StateMachine.SetState(_ctx.LeavingState);
    }

    private void HandleDropItem(int id)
    {
        Debug.LogWarning($"[AI - {_ctx.gameObject.name} - BuyingState]: Unexpected drop at cashier - {_ctx.BoughtItems[0].Data.ItemName}. Check HaveEnoughMoney logic.");
        _ctx.HandleDropItem(_ctx.BoughtItems[id]);
    }

    private void HandleBuyItem(int id)
    {
        Debug.Log($"[AI - {_ctx.gameObject.name} - BuyingState]: Try to buy item at price: {_ctx.BoughtItems[id].LockedPrice}");
        _ctx.CurrentCashStorage.BuyItem(_ctx.BoughtItems[id], _ctx.Wallet);
    }
}
