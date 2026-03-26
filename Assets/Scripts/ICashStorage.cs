using StoreSimulator.InteractableObjects;
using UnityEngine;

public interface ICashStorage
{
    public Vector3 InteractionPoint { get; }
    public bool IsOccupied { get; }
    public bool IsAvailable { get; }
    public void BuyItem(IStoreable item);
}
