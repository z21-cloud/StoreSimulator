using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace StoreSimulator.InteractableObjects
{
    public class HoldingHandler : MonoBehaviour
    {
        [Header("Release force")]
        [Tooltip("For release holdable objects")]
        [SerializeField] private float releaseForce = 100f;
        [Header("Point for holding object")]
        [SerializeField] private Transform holdPoint;

        // components
        private GameObject _heldObject;
        private Camera _mainCamera;

        // consts
        private const float DIRECTION_CAMERA_OFFSET = 0.2f;

        private void Awake()
        {
            _mainCamera = Camera.main;
        }

        public void DoInteract(GameObject currentInteractable)
        {
            // Interaction with holdable item
            if (_heldObject != null)
            {
                //Place item
                PlaceItem(currentInteractable);

                if (_heldObject == null) return;

                // Drop or throw gameobject
                ReleaseHoldableObject();
                return;
            }

            if (currentInteractable == null) return;
            
            TakeItem(currentInteractable);

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

                _heldObject = ((MonoBehaviour)holdable).gameObject;
                holdable.Hold(holdPoint);
            }
            else if(currentInteractable.TryGetComponent<IPriceTag>(out var priceTag))
            {
                priceTag.DoInteract();
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

                if (itemObj != null)
                {
                    itemObj.TryGetComponent(out targetStoreable);
                }
            }

            if(targetStoreable != null)
            {
                GameObject objToHold = targetStoreable.OnPickedFromStore();

                if(objToHold != null && objToHold.TryGetComponent<IHoldable>(out var holdable))
                {
                    _heldObject = objToHold;
                    holdable.Hold(holdPoint);
                }
            }
        }

        private void PlaceItem(GameObject currentInteractable)
        {
            if (currentInteractable != null &&
                currentInteractable.TryGetComponent<IStorage>(out var storage) &&
                _heldObject.TryGetComponent<IStoreable>(out var storeable))
            {
                if (storage.CanPlaceItem(_heldObject))
                {
                    storage.PlaceItem(_heldObject);
                    _heldObject = null;
                    return;
                }
            }
        }

        private void ReleaseHoldableObject()
        {
            if (_heldObject.TryGetComponent<IHoldable>(out var holdable))
            {
                // vector for releasing holdable
                Vector3 throwVector = (_mainCamera.transform.forward +
                    Vector3.up * DIRECTION_CAMERA_OFFSET).normalized;
                // get force from object
                float forceMultiplier = holdable.ThrowForce;
                // throw or release object (depends on force multiplier)
                holdable.Release(throwVector * releaseForce * forceMultiplier);

                // reset 
                _heldObject = null;
            }
        }
    }
}
