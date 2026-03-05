using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace StoreSimulator.InteractableObjects
{
    public interface IStorage
    {
        public bool CanPlaceItem(IStoreable storable);
        public bool CanTakeItem();
        public bool HasFreeSlot();
        public void PlaceItem(GameObject item);
        public GameObject PeekItem();
        public GameObject TakeItem(Vector3 interactionPoint);
    }
}

