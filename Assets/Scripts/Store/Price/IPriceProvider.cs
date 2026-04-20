using StoreSimulator.StoreableItems;

public interface IPriceProvider
{
    public float GetPrice(ItemData data);
    public void SetPrice(ItemSubCategory subCategory, float price);
    public float GetMarketPrice(ItemData data);
}
