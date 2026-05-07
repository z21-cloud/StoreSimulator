using StoreSimulator.ArtificialIntelligence;
using UnityEngine;

public interface IUtilityAction
{
    public float Score(NPCWorldState state);
    public void Execute(NPCController ctx);
    public string Name { get; }
    public bool CanBeInterrupted { get; }
}
