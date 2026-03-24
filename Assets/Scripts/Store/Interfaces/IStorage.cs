using UnityEngine;

namespace StoreSimulator.InteractableObjects
{
    public interface IStorage
    {
        public Transform InteractionPoint { get; }
        public bool CanPlaceItem(IStoreable storable);
        public bool CanTakeItem();
        public bool HasFreeSlot();
        public void PlaceItem(GameObject item);
        public GameObject PeekItem();
        public GameObject TakeItem(Vector3 interactionPoint);
    }
}

