using StoreSimulator.ArtificialIntelligence;
using StoreSimulator.InteractableObjects;
using StoreSimulator.StoreManager;
using UnityEngine;

public class TakingState : INPCState
{
    private readonly NPCController _ctx;

    private bool _acceptDeal = false;
    //private bool _steal = false;
    private float _pickTimer = 0f;

    public TakingState(NPCController ctx) => _ctx = ctx;

    public void Enter()
    {
        
    }

    public void Tick()
    {
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
            _ctx.StateMachine.SetState(_ctx.LeavingState);
            return;
        }

        if (_ctx.BoughtItems.Count == 0)
        {
            Debug.Log($"[AI - {_ctx.gameObject.name} - TakingState]: Shelf is empty");

            _ctx.StateMachine.SetState(_ctx.LeavingState);
            return;
        }

        if (_ctx.Shelves.Count == 0) Debug.LogWarning($"[AI - {_ctx.gameObject.name} - TakingState] I have no shelves!");
        else _ctx.Shelves.RemoveAt(0);

        if (_ctx.Shelves.Count > 0)
        {
            _ctx.CurrentShelf = _ctx.Shelves[0];
            Debug.Log($"[AI - {_ctx.gameObject.name} - TakingState]: Shelves is not empty, moving to next storage...");
            
            _ctx.Movement.SetDestination(_ctx.CurrentShelf.InteractionPoint);
            // _ctx.StateMachine.SetState(_ctx.MovingToStorage);
            
            return;
        }

        _ctx.StateMachine.SetState(_ctx.BuyingState);
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
                    if(_ctx.CurrentShelf.CanPlaceItem(storeable))
                    {
                        _ctx.CurrentShelf.PlaceItem(boughtGO);
                    }
                    else
                    {
                        Debug.LogWarning($"[AI - TakingState - {_ctx.gameObject.name}]: Unexpected error. Can't place back taken storeable. Dropping item");
                        _ctx.HandleDropItem(storeable);
                    }

                    // _steal = _ctx.Psycho.StealItemOrNot();
                    // Debug.Log($"[AI - {_ctx.gameObject.name} - TakingState]: Do I wanna steal? Result - {_steal}");
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

    public void Exit()
    {
        _acceptDeal = false;
        // _steal = false;
    }
}
