using System.Collections.Generic;
using StoreSimulator.ArtificialIntelligence;
using StoreSimulator.InteractableObjects;
using StoreSimulator.StoreableItems;
using StoreSimulator.StoreManager;
using UnityEngine;

public class ShoppingState : INPCState
{
    private readonly NPCController _ctx;
    private bool _acceptDeal = false;
    private bool _steal = false;
    private float _pickTimer = 0f;

    public ShoppingState(NPCController ctx) => _ctx = ctx;

    public void Enter()
    {
        _ctx.Shelves.Clear();
        _ctx.BoughtItems.Clear();
        _ctx.ItemsToBuy = Random.Range(1, _ctx.BuyPool + 1);

        List<ItemCategory> needs = _ctx.NPCNeeds;
        foreach (ItemCategory need in needs)
        {
            Debug.Log($"[AI: {_ctx.gameObject.name} - IdleState] I want to buy...{need.ToString()}!");
            List<IStorage> found = _ctx.CurrentStore.StorageRegistry.GetStorageByNeeds(need);

            if (found != null && found.Count > 0) _ctx.Shelves.Add(found[0]);
        }

        if (_ctx.Shelves == null || _ctx.Shelves.Count == 0)
        {
            Debug.Log($"[AI - {_ctx.gameObject.name} - IdleState] Can't find needed shelf. Leaving...");
            float totalSpent = 0f;
            PriceReactionType reaction = PriceReactionType.Scam;
            _ctx.RecordVisit(totalSpent, reaction);
            Leaving();
            return;
        }
    }

    public void Tick()
    {
        if (_ctx.Shelves.Count == 0) Debug.LogWarning($"[AI - {_ctx.gameObject.name} - TakingState] I have no shelves!");
        else
        {
            _ctx.CurrentShelf = _ctx.Shelves[0];
            _ctx.Shelves.RemoveAt(0);
            Debug.Log($"[AI - {_ctx.gameObject.name} - TakingState]: Shelves is not empty, moving to next storage...");
        }

        Debug.Log("Moving...");
        _ctx.Movement.SetDestination(_ctx.CurrentShelf.InteractionPoint);

        if (!_ctx.Movement.HasReached) return;
        
        _ctx.transform.LookAt(_ctx.CurrentShelf.InteractionPoint);

        if (TryTakingFromShelf() && _ctx.BoughtItems.Count < _ctx.ItemsToBuy)
        {
            _pickTimer -= Time.deltaTime;
            if (_pickTimer <= 0f)
            {
                _pickTimer = _ctx.PickDelay;
            }
            return;
        }

        if (_steal)
        {
            _ctx.StateMachine.SetState(_ctx.StealingState);
            return;
        }

        if (!_acceptDeal)
        {
            _ctx.RecordVisit(0f);
            Leaving();
            return;
        }

        if (_ctx.BoughtItems.Count == 0)
        {
            Debug.Log($"[AI - {_ctx.gameObject.name} - TakingState]: Shelves are empty");

            Leaving();
            return;
        }

        if (_ctx.Shelves.Count > 0) return;

        _ctx.StateMachine.SetState(_ctx.MovingToCheckout);
    }

    private bool TryTakingFromShelf()
    {
        _ctx.transform.LookAt(_ctx.CurrentShelf.InteractionPoint);

        if (_ctx.CurrentShelf.CanTakeItem())
        {
            GameObject go = _ctx.CurrentShelf.PeekItem();

            if (go.TryGetComponent<IStoreable>(out var storeable))
            {
                GameObject boughtGO = storeable.OnPickedFromStore();
                boughtGO.transform.position = _ctx.PickUpPoint.position;
                boughtGO.transform.parent = _ctx.PickUpPoint;

                float itemPrice = storeable.LockedPrice;
                float marketPrice = PricesManager.Instance.GetMarketPriceForItem(storeable.Data);
                _acceptDeal = _ctx.Psycho.BuyItemOrNot(itemPrice, marketPrice);

                if (_ctx.HaveEnoughMoney(storeable) && _acceptDeal)
                {
                    _ctx.BoughtItems.Add(storeable);

                    Debug.Log($"[AI - {_ctx.gameObject.name} - TakingState]: take storeable - {boughtGO.name}");
                    return true;
                }
                else
                {
                    if (_ctx.CurrentShelf.CanPlaceItem(storeable))
                    {
                        _ctx.CurrentShelf.PlaceItem(boughtGO);
                    }
                    else
                    {
                        Debug.LogWarning($"[AI - TakingState - {_ctx.gameObject.name}]: Unexpected error. Can't place back taken storeable. Dropping item");
                        _ctx.HandleDropItem(storeable);
                    }

                    _steal = _ctx.Psycho.StealItemOrNot();
                    Debug.Log($"[AI - {_ctx.gameObject.name} - TakingState]: Do I wanna steal? Result - {_steal}");
                    return false;
                }
            }
            else
            {
                Debug.LogWarning($"[AI - TakingState - {_ctx.gameObject.name}]: Picked GO has no storeable component!");
            }
        }

        Debug.Log($"[AI - {_ctx.gameObject.name} - TakingState]: Can't take item. Shelf is empty");
        return false;
    }

    private void Leaving()
    {
        _ctx.Movement.SetDestination(_ctx.CurrentStore.StoreLeavePoint.position);
        _ctx.StateMachine.SetState(_ctx.LeavingState);
    }

    public void Exit()
    {
        _acceptDeal = false;
        _steal = false;
    }
}
