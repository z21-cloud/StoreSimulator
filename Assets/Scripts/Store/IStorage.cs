using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace StoreSimulator.InteractableObjects
{
    public interface IStorage
    {
        public bool CanPlaceItem(IStoreable item);
        public void PlaceItem(IStoreable item);
        public IHoldable GetPlacedItem();
        public bool IsEmpty { get; }
    }
}
