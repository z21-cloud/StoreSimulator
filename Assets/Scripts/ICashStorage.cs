using StoreSimulator.InteractableObjects;
using UnityEngine;

public interface ICashStorage
{
    public bool IsOccupied { get; }
    public bool IsAvailable { get; }
    public void BuyItem(IStoreable item);
}
