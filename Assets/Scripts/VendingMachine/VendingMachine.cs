using System.Collections.Generic;
using StoreSimulator.InteractableObjects;
using StoreSimulator.MoneySystem;
using StoreSimulator.StoreableItems;
using StoreSimulator.StoreManager;
using UnityEngine;

public class VendingMachine : MonoBehaviour
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

    private void Awake()
    {
        _priceProvider = isPlayerStore
                ? new PlayerPriceManager(pricesManager)
                : new MarketPriceManager(pricesManager);
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
        if (!wallet.CanAfford(price)) return false;
        
        wallet.Spend(price);
        
        SpawnItem(item);

        return true;
    }

    private void SpawnItem(ItemData item)
    {
        GameObject gameObject = Instantiate
        (
            item.Prefab,
            deliveryPoint.position,
            Quaternion.identity
        );
    }
}
