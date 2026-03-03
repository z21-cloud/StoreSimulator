using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace StoreSimulator.InteractableObjects
{
    public interface IStorage
    {
        public bool CanPlaceItem(GameObject item);
        public bool CanTakeItem();
        public void PlaceItem(GameObject item);
        public GameObject TakeItem(Vector3 interactionPoint);
    }
}

