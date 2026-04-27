using System.Collections.Generic;
using StoreSimulator.ArtificialIntelligence;
using StoreSimulator.Boxes;
using StoreSimulator.StoreableItems;
using UnityEngine;

public class OrderItemsState : INPCState
{
    private readonly NPCOwner _ctx;

    public OrderItemsState(NPCOwner ctx) => _ctx = ctx;
    public void Enter()
    {
        _ctx.Movement.SetDestination(_ctx.Store.DeliveryPoint.position);
    }

    public void Exit()
    {
        if(_ctx.BoxDeliverable == null) Debug.LogError($"[AI - {_ctx.gameObject.name}] BoxDeliverable is null!");
    }

    public void Tick()
    {
        if (_ctx.Movement.HasReached)
        {
            ItemData itemToBuy = _ctx.Storeable.Data;

            Dictionary<DeliveryOrder, float> orders = new Dictionary<DeliveryOrder, float>(DeliveryPriceManager.Instance.DeliveryOrders);
            foreach (var order in orders.Keys)
            {
                if (order.ItemData == itemToBuy && _ctx.Wallet.CanAfford(orders[order]))
                {
                    _ctx.Wallet.Spend(orders[order]);
                    IDeliverable box = _ctx.BoxPooling.GetBoxStorage();
                    box.Initialize(order, _ctx.StoreablePooling);
                    box.transform.position = _ctx.Store.DeliveryPoint.position;
                    _ctx.SetBox(box);

                    // // Delay between actions
                    _ctx.WaitingOwnerState.SetReturn(_ctx.TakeBoxState);
                    _ctx.StateMachine.SetState(_ctx.WaitingOwnerState);
                }
            }
        }
    }
}
