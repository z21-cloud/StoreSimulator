using StoreSimulator.ArtificialIntelligence;
using StoreSimulator.InteractableObjects;
using StoreSimulator.StoreManager;
using UnityEngine;

public class TakingState : INPCState
{
    private readonly NPCController _ctx;

    private bool _acceptDeal = false;
    private float _pickTimer = 0f;

    public TakingState(NPCController ctx) => _ctx = ctx;

    public void Enter()
    {
        Debug.Log($"[AI - {_ctx.gameObject.name}]: Taking from shelf");

        if (_ctx.CurrentShelf.CanTakeItem())
        {
            var sample = _ctx.CurrentShelf.PeekItem().GetComponent<IStoreable>();
            float playerPrice = PricesManager.Instance.GetPlayerPriceForItem(sample.Data);
            float marketPrice = PricesManager.Instance.GetMarketPriceForItem(sample.Data);
            _acceptDeal = _ctx.Psycho.BuyItemOrNot(playerPrice, marketPrice);
        }
    }

    public void Tick()
    {
        if (!_acceptDeal)
        {
            _ctx.RecordVisit(0f);
            _ctx.StateMachine.SetState(_ctx.LeavingState);
            return;
        }

        if (TryTakingFromShelf() && _ctx.BoughtItems.Count < _ctx.ItemsToBuy)
        {
            _pickTimer -= Time.deltaTime;
            if (_pickTimer <= 0f)
            {
                _pickTimer = _ctx.PickDelay;
            }
            return;
        }

        if (_ctx.BoughtItems.Count == 0)
        {
            Debug.Log($"[AI - {_ctx.gameObject.name}]: Shelf is empty");

            _ctx.StateMachine.SetState(_ctx.LeavingState);
            return;
        }

        if (_ctx.Shelves.Count == 0) Debug.LogWarning($"[AI - {_ctx.gameObject.name}] I have no shelves!");
        else _ctx.Shelves.RemoveAt(0);

        if (_ctx.Shelves.Count > 0)
        {
            _ctx.CurrentShelf = _ctx.Shelves[0];
            Debug.Log($"[AI - {_ctx.gameObject.name}]: Shelves is not empty, moving to next storage...");
            _ctx.StateMachine.SetState(_ctx.MovingState);
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
            if (go.TryGetComponent<IStoreable>(out var storeable) && _ctx.HaveEnoughMoney(storeable))
            {
                _ctx.BoughtItems.Add(storeable);

                GameObject boughtGO = storeable.OnPickedFromStore();
                boughtGO.transform.position = _ctx.StorageForItems.position;
                boughtGO.transform.parent = _ctx.StorageForItems;

                Debug.Log($"[AI - {_ctx.gameObject.name}]: take storeable - {boughtGO.name}");
                return true;
            }

            if (!_ctx.HaveEnoughMoney(storeable)) Debug.Log($"[AI - {_ctx.gameObject.name}] Have not enough money!");
            // else if(storeable.Category != wantedProducts) Debug.Log($"[AI - {gameObject.name}] Different categories! \n Wanter category: {wantedProducts.ToString()}. Item Category: {storeable.Category}");
            else Debug.LogWarning($"[AI - {_ctx.gameObject.name}]: Item has no storeable component!");
            return false;
        }

        Debug.Log($"[AI - {_ctx.gameObject.name}]: Can't take item. Shelf is empty");
        return false;
    }

    public void Exit()
    {
        _ctx.CurrentCashStorage = CashStorageRegistry.Instance.GetRandomCashStorage();
        _acceptDeal = false;
    }
}
