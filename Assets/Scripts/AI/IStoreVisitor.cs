using System.Collections.Generic;
using StoreSimulator.InteractableObjects;

public interface IStoreVisitor
{
    public IStore CurrentStore { get; }
    public List<IStoreable> Inventory { get; }
}
