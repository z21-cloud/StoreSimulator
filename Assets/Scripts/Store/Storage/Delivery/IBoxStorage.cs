using StoreSimulator.StoreableItems;
using UnityEngine;

namespace StoreSimulator.Boxes
{
    public interface IBoxStorage
{
    public bool HasFreeSlot();
    public bool CanTakeItem();
    public void PlaceBox(BoxStorage box);
    public BoxStorage TakeBox();
}
}

