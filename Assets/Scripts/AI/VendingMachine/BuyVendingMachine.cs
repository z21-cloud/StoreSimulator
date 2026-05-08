using System.Collections.Generic;
using StoreSimulator.ArtificialIntelligence;
using StoreSimulator.InteractableObjects;
using StoreSimulator.StoreableItems;
using UnityEngine;

public class BuyVendingMachine : INPCState
{
    private readonly VendingNPCController _ctx;

    public BuyVendingMachine(VendingNPCController ctx) => _ctx = ctx;

    private List<ItemCategory> needs;

    public void Enter()
    {
        needs = new List<ItemCategory>();

        _ctx.Shelves.Clear();
        _ctx.BoughtItems.Clear();
        _ctx.ItemsToBuy = Random.Range(1, _ctx.BuyPool + 1);

        needs = _ctx.NPCNeeds;
    }

    public void Tick()
    {
        foreach (var need in needs)
        {
            ItemData pickedData = _ctx.CurrentStore.PeekItem(need);
            float vendingMachinePrice = _ctx.CurrentStore.GetPrice(pickedData);
            float marketPrice = _ctx.CurrentStore.GetMarketPrice(pickedData);

            if (_ctx.HaveEnoughMoney(pickedData) && _ctx.CurrentStore.TryBuy(pickedData, _ctx.Wallet))
            {
                _ctx.CurrentShelf = _ctx.CurrentStore.VendingShelf;

                if (_ctx.CurrentShelf.CanTakeItem())
                {
                    GameObject boughtGO = _ctx.CurrentShelf.TakeItem(_ctx.transform.position);
                    boughtGO.transform.position = _ctx.PickUpPoint.position;
                    boughtGO.transform.parent = _ctx.PickUpPoint;

                    if (boughtGO.TryGetComponent(out IStoreable storeable))
                    {
                        _ctx.BoughtItems.Add(storeable);
                    }
                }
            }
        }

        if (_ctx.BoughtItems.Count > 0)
        {
            Leaving();
        }
        else
        {
            Debug.LogError($"No items bought");
        }
    }

    private void Leaving()
    {
        _ctx.Movement.SetDestination(_ctx.CurrentStore.StoreLeavePoint.position);
        _ctx.StateMachine.SetState(_ctx.LeavingVendingMachine);
    }

    public void Exit()
    {

    }
}
