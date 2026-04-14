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
        _ctx.Movement.SetDestination(_ctx.Store.OrderItems.position);
    }

    public void Exit()
    {
        
    }

    public void Tick()
    {
        if(_ctx.Movement.HasReached)
        {
            ItemData itemToBuy = _ctx.Storeable.Data;
            
            Dictionary <DeliveryOrder, float> orders = new Dictionary<DeliveryOrder, float> (DeliveryPriceManager.Instance.DeliveryOrders);
            foreach(var order in orders.Keys)
            {
                if(order.ItemData == itemToBuy)
                {
                    BoxStorage box = _ctx.BoxPooling.GetBoxStorage();
                    box.Initialize(order);
                    box.transform.position = _ctx.Store.DeliveryZone.position;
                    _ctx.BoxStorage = box;
                    _ctx.StateMachine.SetState(_ctx.TakeBoxState);
                    return;
                }
            }
        }
    }
}
