using StoreSimulator.ArtificialIntelligence;
using StoreSimulator.StoreManager;
using UnityEngine;

public class BuyingState : INPCState
{
    private readonly NPCController _ctx;

    public BuyingState(NPCController ctx) => _ctx = ctx;

    public void Enter()
    {
        if(_ctx.BoughtItems == null || _ctx.BoughtItems.Count == 0)
        {
            Leaving();
            return;
        }
    }

    public void Exit()
    {
        
    }

    public void Tick()
    {
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
            /*Debug.Log($"[AI - {_ctx.gameObject.name} - BuyingState]: Waiting player to buy item");
            _ctx.WaitingState.SetReturn(_ctx.BuyingState);
            return;*/
            _ctx.StateMachine.SetState(_ctx.BuyingState);
        }
        
        Leaving();
    }

    private void Leaving()
    {
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
