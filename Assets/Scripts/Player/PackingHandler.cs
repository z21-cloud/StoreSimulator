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

            Debug.Log($"{heldObject.name} : {currentInteractable.name}");

            if (currentInteractable.TryGetComponent<IStorage>(out var boxStorage) &&
                heldObject.TryGetComponent<IStoreable>(out var storeable))
            {
                Debug.Log($"Try pack");
                if (boxStorage.CanPlaceItem(storeable))
                {
                    Debug.Log($"Packing");

                    boxStorage.PlaceItem(heldObject);
                    return true;
                }
            }

            return false;
        }
    }
}
