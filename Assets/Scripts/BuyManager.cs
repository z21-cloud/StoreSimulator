using StoreSimulator.InteractableObjects;
using StoreSimulator.MoneySystem;
using StoreSimulator.StoreManager;
using UnityEngine;

public class BuyManager : MonoBehaviour
{
    [SerializeField] private PlayerWallet playerWallet;
    public static BuyManager Instance { get; private set; }

    void Awake()
    {
        if(Instance != null && Instance != this)
        {
            Destroy(this);
            return;
        }

        Instance = this;
    }

    public void IncreasePlayerWallet(IStoreable bought)
    {
        if(bought == null) return;
        float price = PricesManager.Instance.GetPriceForItem(bought.Data);
        playerWallet.Add(price);
    }
}
