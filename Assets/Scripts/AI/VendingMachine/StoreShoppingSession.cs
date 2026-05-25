using System.Collections.Generic;
using StoreSimulator.ArtificialIntelligence;
using StoreSimulator.InteractableObjects;
using StoreSimulator.MoneySystem;
using StoreSimulator.StoreableItems;
using StoreSimulator.StoreManager;
using StoreSimulator.StoreUtility;
using UnityEngine;

public class StoreShoppingSession : IShoppingSession
{
    private enum Step { GoToShelf, GoToCashier, Done }

    private readonly Store _store;
    private readonly IWallet _wallet;
    // private readonly Transform _pickUp;
    private readonly List<IStorage> _shelves = new();
    private ICashStorage _cashier;

    private Step _currentStep;
    private int _shelfIndex;
    private List<IStoreable> _cart = new();
    private float _totalSpent;

    public bool IsComplete => _currentStep == Step.Done;
    public bool CanAdvance { get; private set; }
    public float TotalSpent => _totalSpent;
    public List<IStoreable> PurchaedItems => _cart;

    public Vector3 CurrentDestination
    {
        get
        {
            switch (_currentStep)
            {
                case Step.GoToShelf:
                    return _shelves[_shelfIndex].InteractionPoint;
                case Step.GoToCashier:
                    return _cashier.InteractionPoint;
                default:
                    return _store.StoreLeavePoint.position;
            }
        }
    }

    public StoreShoppingSession(Store store, List<ItemCategory> needs, IWallet wallet)
    {
        _store = store;
        _wallet = wallet;
        // _pickUp = pickUp;

        foreach (var need in needs)
        {
            var found = store.StorageRegistry.GetStorageByNeeds(need);
            if (found != null && found.Count > 0) _shelves.Add(found[0]);
        }

        _cashier = store.CashStorageRegistry.GetRandomCashStorage();
        _currentStep = _shelves.Count > 0 ? Step.GoToShelf : Step.Done;
    }

    public void Advance()
    {
        throw new System.NotImplementedException();
    }

    public void OnArrived(NPCController npc)
    {
        CanAdvance = false;

        switch (_currentStep)
        {
            case Step.GoToShelf:
                var shelf = _shelves[_shelfIndex];
                if (shelf.CanTakeItem())
                {
                    var go = shelf.TakeItem(npc.transform.position);
                    if (go.TryGetComponent<IStoreable>(out var item))
                    {
                        float price = item.LockedPrice;
                        float market = PricesManager.Instance.GetMarketPriceForItem(item.Data);

                        if (npc.Psycho.BuyItemOrNot(price, market) && npc.HaveEnoughMoney(item))
                        {
                            go.transform.position = npc.PickUpPoint.position;
                            go.transform.parent = npc.PickUpPoint;
                            _cart.Add(item);
                        }
                        else
                        {
                            if(shelf.CanPlaceItem(item)) shelf.PlaceItem(go);
                            else npc.HandleDropItem(item);
                        }
                    }
                }

                _shelfIndex++;
                if(_shelfIndex < _shelves.Count) _currentStep = Step.GoToShelf;
                else if(_cart.Count > 0) _currentStep = Step.GoToCashier;
                else _currentStep = Step.Done;
                
                CanAdvance = true;
                
                break;

            case Step.GoToCashier:
                if(_cashier != null && _cashier.IsAvailable)
                {
                    foreach(var item in _cart)
                    {
                        if(_wallet.CanAfford(item.LockedPrice))
                        {
                            _cashier.BuyItem(item, _wallet);
                            _totalSpent += item.LockedPrice;
                        }
                    }
                }

                _currentStep = Step.Done;
            
                CanAdvance = true;
            
                break; 
        }
    }
}
