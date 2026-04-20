using StoreSimulator.StoreableItems;
using StoreSimulator.StoreManager;
using UnityEngine;

public class MarketPriceManager : IPriceProvider
{
    private readonly PricesManager _manager;
    
    public MarketPriceManager(PricesManager manager) => _manager = manager;

    public float GetMarketPrice(ItemData data)
    {
        return _manager.GetMarketPriceForItem(data);
    }

    public float GetPrice(ItemData data)
    {
        return _manager.GetMarketPriceForItem(data);
    }

    public void SetPrice(ItemSubCategory subCategory, float price)
    {
        
    }
}
