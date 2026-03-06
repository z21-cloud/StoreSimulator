using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace StoreSimulator.InteractableObjects
{
    public class PackingHandler : MonoBehaviour
    {
        public bool DoPackItem(GameObject heldObject, GameObject currentInteractable)
        {
            if (heldObject == null || currentInteractable == null) return false;

            Debug.Log($"PackingHandler: {heldObject.name} - {currentInteractable.name}");

            // Item in hands, box is lying on ground 
            if (currentInteractable.TryGetComponent<IStorage>(out var lyingBoxStorage) &&
                heldObject.TryGetComponent<IStoreable>(out var heldItemStoreable))
            {
                Debug.Log($"PackingHandler: held object to box");
                if (lyingBoxStorage.CanPlaceItem(heldItemStoreable))
                {
                    Debug.Log($"PackingHandler: Packing");

                    lyingBoxStorage.PlaceItem(heldObject);
                    return true;
                }
            }


            else if (heldObject.TryGetComponent<IStorage>(out var heldBoxStorage))
            {
                // Box in hands, item is lying on ground 
                if(currentInteractable.TryGetComponent<IStoreable>(out var lyingItemStoreable))
                {
                    if (heldBoxStorage.CanPlaceItem(lyingItemStoreable))
                    {
                        Debug.Log($"PackingHandler: itemStoreable to heldBoxStorage");

                        heldBoxStorage.PlaceItem(currentInteractable);

                        // return false
                        // because either heldObject (box) will be null, that cause error
                    }
                }

                // Box in hands, item is in shelfStorage
                else if(currentInteractable.TryGetComponent<IStorage>(out var shelfStorage))
                {
                    if (!shelfStorage.CanTakeItem() || !heldBoxStorage.HasFreeSlot()) return false;

                    Debug.Log($"PackingHandler: picking item from shelfStorage to check allowedCategory in boxStorage");

                    GameObject itemToStore = shelfStorage.PeekItem();

                    if (itemToStore.TryGetComponent<IStoreable>(out var storeable) &&
                        heldBoxStorage.CanPlaceItem(storeable))
                    {
                        GameObject taken = shelfStorage.TakeItem(transform.position);

                        Debug.Log($"PackingHandler: itemStoreable from storage to heldBoxStorage");

                        heldBoxStorage.PlaceItem(taken);

                        // return false
                        // because either heldObject (box) will be null, that cause error
                    }
                }
            }

            return false;

            /*else if (currentInteractable.TryGetComponent<IStoreable>(out var itemStoreable)
                && heldObject.TryGetComponent<IStorage>(out var heldBoxStorage))
            {
                if (heldBoxStorage.CanPlaceItem(itemStoreable))
                {
                    Debug.Log($"PackingHandler: itemStoreable to heldBoxStorage");

                    heldBoxStorage.PlaceItem(currentInteractable);

                    // return false
                    // because either heldObject (box) will be null, that cause error
                }
            }

            else if (currentInteractable.TryGetComponent<IStorage>(out var shelfStorage)
                && heldObject.TryGetComponent<IStorage>(out var boxStorage))
            {
                if (!shelfStorage.CanTakeItem() || !boxStorage.HasFreeSlot()) return false;

                Debug.Log($"PackingHandler: picking item from shelfStorage to check allowedCategory in boxStorage");

                GameObject itemToStore = shelfStorage.PeekItem();

                if (itemToStore.TryGetComponent<IStoreable>(out var storeable) &&
                    boxStorage.CanPlaceItem(storeable))
                {
                    GameObject taken = shelfStorage.TakeItem(transform.position);

                    Debug.Log($"PackingHandler: itemStoreable from storage to heldBoxStorage");

                    boxStorage.PlaceItem(taken);

                    // return false
                    // because either heldObject (box) will be null, that cause error
                }
            }*/
        }
    }
}
