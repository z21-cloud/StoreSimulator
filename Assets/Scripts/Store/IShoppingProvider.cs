using System.Collections.Generic;
using StoreSimulator.MoneySystem;
using StoreSimulator.StoreableItems;
using UnityEngine;

public interface IShoppingProvider
{
    public string ProviderID { get; }
    public Transform EnterPoint { get; }
    public Transform LeavePoint { get; }
    public bool IsOpen { get; }

    public IShoppingSession StartSession(List<ItemCategory> needs, IWallet wallet, Transform pickUpPoint);
}
