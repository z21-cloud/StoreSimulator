using UnityEngine;

namespace StoreSimulator.InteractableObjects
{
    public class HoldingHandler : MonoBehaviour
    {
        [Header("Point for holding object")]
        [SerializeField] private Transform holdPoint;

        public GameObject HeldObject { get; private set; }

        public void DoInteract(GameObject currentInteractable)
        {
            // Interaction with holdable item (in playr's hands)
            if (HeldObject != null)
            {
                //Place item
                PlaceItem(currentInteractable);
            }
            else
            {
                if (currentInteractable == null) return;

                // interaction with pickable object, not in hands
                if (currentInteractable.TryGetComponent<IPickable>(out var pickable))
                {
                    Debug.Log($"Pick up");
                    pickable.Pick();
                }

                // interaction with holdable object, not in hands
                else if (currentInteractable.TryGetComponent<IHoldable>(out var holdable))
                {
                    // cache holdable object for releasing
                    Debug.Log("Taking from ground");

                    HeldObject = ((MonoBehaviour)holdable).gameObject;
                    holdable.Hold(holdPoint);
                }
                // change price label to item category (for now: only if storage has items)
                else if (currentInteractable.TryGetComponent<IPriceTag>(out var priceTag))
                {
                    priceTag.DoInteract();
                }
                else if(currentInteractable.TryGetComponent<PCInteractor>(out var pc))
                {
                    pc.Interact();
                }
                else
                {
                    // otherwise trying to take item
                    TakeItem(currentInteractable);
                }
            }
        }

        private void TakeItem(GameObject currentInteractable)
        {
            IStoreable targetStoreable = null;

            // take item directly via it's own collider
            if (currentInteractable.TryGetComponent<IStoreable>(out var directStoreable))
            {
                if (directStoreable.CurrentShelf != null)
                {
                    Debug.Log("Taking from storage - object collider");

                    targetStoreable = directStoreable;
                }
            }
            // take item from storage 
            else if (currentInteractable.TryGetComponent<IStorage>(out var storage))
            {
                if (!storage.CanTakeItem()) return;

                GameObject itemObj = storage.TakeItem(holdPoint.position);

                Debug.Log("Taking from storage - storage collider");

                if (itemObj != null)
                {
                    itemObj.TryGetComponent(out targetStoreable);
                }
            }

            // hold taken item from storage
            if (targetStoreable != null)
            {
                GameObject objToHold = targetStoreable.OnPickedFromStore();

                if (objToHold != null && objToHold.TryGetComponent<IHoldable>(out var holdable))
                {
                    Debug.Log("Taking from storage - Move to hold");

                    HeldObject = objToHold;
                    holdable.Hold(holdPoint);
                }
            }
        }

        private void PlaceItem(GameObject currentInteractable)
        {
            // place storeable item (from player's hands)
            if (currentInteractable != null &&
                currentInteractable.TryGetComponent<IStorage>(out var storage) &&
                HeldObject.TryGetComponent<IStoreable>(out var storeable))
            {
                if (storage.CanPlaceItem(storeable))
                {
                    storage.PlaceItem(HeldObject);
                    HeldObject = null;
                    return;
                }
            }

            // place storeable item (from BOX in player's hands)
            else if (currentInteractable != null &&
                currentInteractable.TryGetComponent<IStorage>(out var shelfStorage) &&
                HeldObject.TryGetComponent<IStorage>(out var boxStorage))
            {
                // if box is empty or shelf is full
                if (!boxStorage.CanTakeItem() || !shelfStorage.HasFreeSlot()) return;

                // Peek item to check it's category and shelf's category
                Debug.Log("Peek item from box");
                GameObject peekedItem = boxStorage.PeekItem();
                if (peekedItem == null) return;

                if (!peekedItem.TryGetComponent<IStoreable>(out var peekedStoreable)) return;
                if (!shelfStorage.CanPlaceItem(peekedStoreable)) return;

                // Take item from box's slot and give it to shelf's slot
                Debug.Log("Place item from box");
                GameObject item = boxStorage.TakeItem(holdPoint.position); // gives game object from slot
                if (item == null) return;

                if(item.TryGetComponent<IStoreable>(out var itemStoreable))
                {
                    // unlink item from CurrentSlot
                    itemStoreable.OnPickedFromStore();
                }

                // give gameobject to new storage
                shelfStorage.PlaceItem(item);
            }
        }

        public void ClearHeldObject()
        {
            // reset
            //HeldObject.transform.parent = null;
            HeldObject = null;
        }
    }
}
