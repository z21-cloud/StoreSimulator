using StoreSimulator.InteractableObjects;
using StoreSimulator.MoneySystem;
using UnityEngine;

public class VendingCashStorage : MonoBehaviour, ICashStorage
{
    private IStore _storeOwner;
    public Vector3 InteractionPoint => _storeOwner.DeliveryPoint.position;

    public Vector3 CashierPoint => throw new System.NotImplementedException();

    public bool IsOccupied => throw new System.NotImplementedException();

    public bool IsAvailable => throw new System.NotImplementedException();


    public void Initailize(IStore store)
    {
        _storeOwner = store;

        _storeOwner.CashStorageRegistry.RegisterCashStorage(this);
    }

    public void BuyItem(IStoreable item, IWallet wallet)
    {
        throw new System.NotImplementedException();
    }
}