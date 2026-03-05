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
            // Interaction with holdable item
            if (HeldObject != null)
            {
                //Place item
                PlaceItem(currentInteractable);
            }
            else
            {
                if (currentInteractable == null) return;

                // interaction with object pickup or hold
                if (currentInteractable.TryGetComponent<IPickable>(out var pickable))
                {
                    Debug.Log($"Pick up");
                    pickable.Pick();
                }
                else if (currentInteractable.TryGetComponent<IHoldable>(out var holdable))
                {
                    // cache holdable object for releasing
                    Debug.Log("Taking from ground");

                    HeldObject = ((MonoBehaviour)holdable).gameObject;
                    holdable.Hold(holdPoint);
                }
                else if (currentInteractable.TryGetComponent<IPriceTag>(out var priceTag))
                {
                    priceTag.DoInteract();
                }
                else
                {
                    TakeItem(currentInteractable);
                }
            }
        }

        private void TakeItem(GameObject currentInteractable)
        {
            IStoreable targetStoreable = null;

            if (currentInteractable.TryGetComponent<IStoreable>(out var directStoreable))
            {
                if (directStoreable.CurrentShelf != null)
                {
                    Debug.Log("Taking from storage - object collider");

                    targetStoreable = directStoreable;
                }
            }
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

            else if (currentInteractable != null &&
                currentInteractable.TryGetComponent<IStorage>(out var shelfStorage) &&
                HeldObject.TryGetComponent<IStorage>(out var boxStorage))
            {
                if (!boxStorage.CanTakeItem() || !shelfStorage.HasFreeSlot()) return;
                Debug.Log("Peek item");
                GameObject peekedItem = boxStorage.PeekItem();
                if (peekedItem == null) return;

                if (!peekedItem.TryGetComponent<IStoreable>(out var peekedStoreable)) return;
                if (!shelfStorage.CanPlaceItem(peekedStoreable)) return;

                Debug.Log("Try take & place item");
                    
                GameObject item = boxStorage.TakeItem(holdPoint.position);
                if (item == null) return;

                if(item.TryGetComponent<IStoreable>(out var itemStoreable))
                {
                    itemStoreable.OnPickedFromStore();
                }

                shelfStorage.PlaceItem(item);
            }
        }

        public void ClearHeldObject()
        {
            HeldObject.transform.parent = null;
            HeldObject = null;
        }
    }
}
