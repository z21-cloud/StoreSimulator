using StoreSimulator.StoreableItems;
using StoreSimulator.StoreManager;

public class PlayerPriceManager : IPriceProvider
{
    private readonly PricesManager _manager;

    public PlayerPriceManager(PricesManager manager) => _manager = manager;

    public float GetMarketPrice(ItemData data)
    {
        return _manager.GetMarketPriceForItem(data);
    }

    public float GetPrice(ItemData data)
    {
        return _manager.GetPlayerPriceForItem(data);
    }

    public void SetPrice(ItemSubCategory subCategory, float price)
    {
        _manager.SetSubCategoryPrice(subCategory, price);
    }
}
