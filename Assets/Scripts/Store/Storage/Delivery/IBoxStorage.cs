using UnityEngine;

namespace StoreSimulator.Boxes
{
    public interface IBoxStorage
{
    public bool HasFreeSlot();
    public bool CanTakeItem();
    public void PlaceBox(GameObject box);
    public GameObject TakeBox();
}
}

