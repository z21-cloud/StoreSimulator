using System.Collections.Generic;
using StoreSimulator.InteractableObjects;
using StoreSimulator.MoneySystem;
using StoreSimulator.StoreableItems;
using StoreSimulator.StoreManager;
using UnityEngine;

public class VendingMachine : MonoBehaviour, ICashStorage, IStore
{
    [Header("Interaction Points")]
    [SerializeField] private Transform interactionPoint;
    [SerializeField] private Transform storeEnterPoint;
    [SerializeField] private Transform storeLeavePoint;
    [SerializeField] private Transform deliveryPoint;

    [Header("Config")]
    [SerializeField] private VendingMachineConfig config;
    [SerializeField] private VendingCashStorage cashStorage;
    [SerializeField] private VendingStorage vendingStorage;
    [SerializeField] private PricesManager pricesManager;
    [SerializeField] private bool isPlayerStore;
    [SerializeField] private bool isOpen;

    [SerializeField] private VendingInventory vendingInventory;

    private IPriceProvider _priceProvider;

    public Vector3 InteractionPoint => interactionPoint.position;

    public Vector3 CashierPoint => throw new System.NotImplementedException();

    public bool IsOccupied => throw new System.NotImplementedException();

    public bool IsAvailable => true;

    public IPriceProvider PriceProvider => throw new System.NotImplementedException();

    public CashStorageRegistry CashStorageRegistry => throw new System.NotImplementedException();

    public StorageRegistry StorageRegistry => throw new System.NotImplementedException();

    public Transform StoreEnterPoint => storeEnterPoint;

    public Transform StoreLeavePoint => storeLeavePoint;

    public Transform DeliveryPoint => deliveryPoint;

    public string StoreID => config.StoreID;

    public bool IsOpen => throw new System.NotImplementedException();

    public bool IsAuto => true;
    public IStorage VendingShelf {get; private set;}

    private void Awake()
    {
        _priceProvider = isPlayerStore
                ? new PlayerPriceManager(pricesManager)
                : new MarketPriceManager(pricesManager);

        VendingShelf = vendingStorage;
    }

    public ItemData PeekItem(ItemCategory need)
    {
        return vendingInventory.GetItem(need);
    }

    public float GetPrice(ItemData item)
    {
        return _priceProvider.GetPrice(item);
    }

    public float GetMarketPrice(ItemData item)
    {
        return _priceProvider.GetMarketPrice(item);
    }

    public bool TryBuy(ItemData item, IWallet wallet)
    {
        float price = GetPrice(item);
        if (!wallet.CanAfford(price) || !vendingStorage.HasFreeSlot()) return false;

        GameObject spawnedGO = SpawnItem(item);
        if (spawnedGO.TryGetComponent(out IStoreable storeable) && vendingStorage.CanPlaceItem(storeable))
        {
            spawnedGO.GetComponent<Collider>().enabled = false;
            spawnedGO.GetComponent<Rigidbody>().isKinematic = true;

            vendingStorage.PlaceItem(spawnedGO);
            
            BuyItem(storeable, wallet);
            
            return true;

        }
        else
        {
            Destroy(spawnedGO);
            return false;
        }
    }

    private GameObject SpawnItem(ItemData item) => Instantiate
        (
            item.Prefab,
            deliveryPoint.position,
            Quaternion.identity
        );

    public void BuyItem(IStoreable item, IWallet wallet)
    {
        float price = GetPrice(item.Data);
        wallet.Spend(price);
    }
}
