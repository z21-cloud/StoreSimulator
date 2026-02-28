using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace StoreSimulator.InteractableObjects
{
    public interface IStorage
    {
        public bool IsEmpty { get; }
        public bool CanPlaceItem(Vector3 position);
        public void PlaceItem(IStoreable item);
        public IStoreable GetPlacedItem(Vector3 position);
    }
}
