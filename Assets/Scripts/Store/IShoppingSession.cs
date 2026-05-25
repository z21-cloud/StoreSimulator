using System.Collections.Generic;
using StoreSimulator.ArtificialIntelligence;
using StoreSimulator.InteractableObjects;
using UnityEngine;

public interface IShoppingSession
{
    public bool IsComplete { get; }
    public Vector3 CurrentDestination { get; }
    public void OnArrived(NPCController npc);
    public bool CanAdvance { get; }
    public void Advance();
    public float TotalSpent { get; }
    public List<IStoreable> PurchaedItems { get; }
}
