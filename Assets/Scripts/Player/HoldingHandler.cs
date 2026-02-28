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
                // Place item in storage
                PlaceItem(currentInteractable);
                if (_heldObject == null) return;

                // Drop or throw gameobject
                ReleaseHoldableObject();
                return;
            }

            if (currentInteractable == null) return;

            if (currentInteractable.TryGetComponent<IStorage>(out var storage))
            {
                if (storage.IsEmpty) return;

                Debug.Log("Taking from shelf");

                IStoreable storeable = storage.GetPlacedItem(transform.position);
                storeable.Hold(holdPoint);

                _heldObject = ((MonoBehaviour)storeable).gameObject;
            }

            // interaction with object pickup or hold
            if (currentInteractable.TryGetComponent<IPickable>(out var pickable))
            {
                pickable.Pick();
            }
            else if (currentInteractable.TryGetComponent<IHoldable>(out var holdable))
            {
                // cache holdable object for releasing
                Debug.Log("Taking from ground");

                _heldObject = ((MonoBehaviour)holdable).gameObject;
                holdable.Hold(holdPoint);
            }
            else if(currentInteractable.TryGetComponent<IStoreable>(out var storable))
            {
                if(storable.IsStored)
                {
                    return;
                }
                _heldObject = ((MonoBehaviour)storable).gameObject;
                storable.Hold(holdPoint);
            }
        }

        private void PlaceItem(GameObject currentInteractable)
        {
            if (currentInteractable != null && currentInteractable.TryGetComponent<IStorage>(out var storage))
            {
                if (_heldObject.TryGetComponent<IStoreable>(out var storeable))
                {
                    Debug.Log("Place Item");
                    if (storage.CanPlaceItem(storeable))
                    {
                        storage.PlaceItem(storeable);
                        _heldObject = null;
                        return;
                    }
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
            else if(_heldObject.TryGetComponent<IStoreable>(out var storeable))
            {
                // vector for releasing holdable
                Vector3 throwVector = (_mainCamera.transform.forward +
                    Vector3.up * DIRECTION_CAMERA_OFFSET).normalized;
                // get force from object
                float forceMultiplier = storeable.ThrowForce;
                // throw or release object (depends on force multiplier)
                storeable.Release(throwVector * releaseForce * forceMultiplier);

                // reset 
                _heldObject = null;
            }
        }
    }
}
