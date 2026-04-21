using StoreSimulator.StoreableItems;
using UnityEngine;

namespace StoreSimulator.Delivery
{
    public interface IDeliveryStorage
    {
        public bool HasFreeSlot();
        public bool CanTakeItem();
        public void PlaceBox(IDeliverable box);
        public IDeliverable TakeBox();
    }
}

