using StoreSimulator.ArtificialIntelligence;
using StoreSimulator.InteractableObjects;
using StoreSimulator.StoreManager;
using UnityEngine;

public class MovingToStorage : INPCState
{
    private readonly NPCController _ctx;
    private float _pickTimer = 0f;
    private bool _acceptDeal = false;
    // private bool _steal = false;
    public MovingToStorage(NPCController ctx) => _ctx = ctx;

    public void Enter()
    {
        if (_ctx.Shelves.Count == 0)
        {
            Debug.Log($"[AI - {_ctx.gameObject.name} - MovingToStorage] I have no shelves! Leaving");
        }
        else
        {
            _ctx.CurrentShelf = _ctx.Shelves[0];
            _ctx.Movement.SetDestination(_ctx.CurrentShelf.InteractionPoint);

            _ctx.Shelves.RemoveAt(0);
            Debug.Log($"[AI - {_ctx.gameObject.name} - ShoppingState]: Shelves is not empty, moving to next storage...");
        }
    }

    public void Tick()
    {
        if (!_ctx.Movement.HasReached) return;

        if (TryTakingFromShelf() && _ctx.BoughtItems.Count < _ctx.ItemsToBuy)
        {
            _pickTimer -= Time.deltaTime;
            if (_pickTimer <= 0f)
            {
                _pickTimer = _ctx.PickDelay;
            }
            return;
        }

        /*if (_steal)
        {
            _ctx.StateMachine.SetState(_ctx.StealingState);
            return;
        }*/

        if (!_acceptDeal)
        {
            // _ctx.RecordVisit(0f);
            Leaving();
            return;
        }

        if (_ctx.BoughtItems.Count == 0)
        {
            Debug.Log($"[AI - {_ctx.gameObject.name} - ShoppingState]: Shelves are empty");

            Leaving();
            return;
        }

        if(_ctx.Shelves.Count > 0)
        {
            _ctx.StateMachine.SetState(_ctx.MovingToStorage);
            return;
        }

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

                    Debug.Log($"[AI - {_ctx.gameObject.name} - ShoppingState]: take storeable - {boughtGO.name}");
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
                        Debug.LogWarning($"[AI - ShoppingState - {_ctx.gameObject.name}]: Unexpected error. Can't place back taken storeable. Dropping item");
                        _ctx.HandleDropItem(storeable);
                    }

                    // _steal = _ctx.Psycho.StealItemOrNot();
                    // Debug.Log($"[AI - {_ctx.gameObject.name} - ShoppingState]: Do I wanna steal? Result - {_steal}");
                    return false;
                }
            }
            else
            {
                Debug.LogWarning($"[AI - ShoppingState - {_ctx.gameObject.name}]: Picked GO has no storeable component!");
            }
        }

        Debug.Log($"[AI - {_ctx.gameObject.name} - ShoppingState]: Can't take item. Shelf is empty");
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
        // _steal = false;
    }
}
