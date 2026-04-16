using StoreSimulator.InteractableObjects;
using StoreSimulator.MoneySystem;
using UnityEngine;

public interface ICashStorage
{
    public Vector3 InteractionPoint { get; }
    public Vector3 CashierPoint { get; }

    public bool IsOccupied { get; }
    public bool IsAvailable { get; }
    public void BuyItem(IStoreable item, IWallet wallet);
}
