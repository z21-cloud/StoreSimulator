using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace StoreSimulator.InteractableObjects
{
    public interface IStorage
    {
        public bool CanPlaceItem(IStoreable item);
        public void PlaceItem(IStoreable item);
        public IStoreable GetPlacedItem(Vector3 position);
        public bool IsEmpty { get; }
    }
}
