using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace StoreSimulator.InteractableObjects
{
    public class PlayerInteraction : MonoBehaviour
    {
        [Header("Interaction distance")]
        [SerializeField] private float interactableDistance = 5f;
        [Header("Release force")]
        [Tooltip("For release holdable objects")]
        [SerializeField] private float releaseForce = 100f;
        [Header("Point for holding object")]
        [SerializeField] private Transform holdPoint;

        // consts
        private readonly Vector3 CENTER_OF_CAMERASCREEN = new Vector3(0.5f, 0.5f, 0); // center of camera screen
        private const float DIRECTION_CAMERA_OFFSET = 0.2f;

        // components
        private GameObject _heldObject;
        private GameObject _currentInteractable;
        private Camera _mainCamera;

        private void Awake()
        {
            _mainCamera = Camera.main;
        }

        private void Update()
        {
            _currentInteractable = DetectInteractable();
        }

        private GameObject DetectInteractable()
        {
            // create ray from center of main camera
            Ray ray = _mainCamera.ViewportPointToRay(CENTER_OF_CAMERASCREEN);

            // if hits smth in interactableDistance
            if (!Physics.Raycast(ray, out RaycastHit hit, interactableDistance)) return null;

            // Draw line for debugging
            Debug.DrawLine(ray.origin, hit.point, Color.red);

            // return parent interface IInteractable, if gameobject has implementation
            if (hit.collider.TryGetComponent<IInteractable>(out _))
                return hit.collider.gameObject;

            // to prevert cases, where component in parent's object and collider inside parent's object
            if (hit.collider.GetComponentInParent<IInteractable>() is IInteractable)
            {
                return hit.collider.transform.parent.gameObject;
            }

            return null;
        }

        // Interaction with gameobjects
        public void DoInteract()
        {
            // Interaction with holdable item
            if (_heldObject != null)
            {
                // Place item in storage
                PlaceItem();
                if (_heldObject == null) return;

                // Drop or throw gameobject
                ReleaseHoldableObject();
                return;
            }

            if (_currentInteractable == null) return;

            // interaction with object pickup or hold
            if (_currentInteractable.TryGetComponent<IPickable>(out var pickable))
            {
                pickable.Pick();
            }
            else if (_currentInteractable.TryGetComponent<IHoldable>(out var holdable))
            {
                // cache holdable object for releasing
                _heldObject = ((MonoBehaviour)holdable).gameObject;
                holdable.Hold(holdPoint);
            }
        }

        private void PlaceItem()
        {
            if (_currentInteractable != null && _currentInteractable.TryGetComponent<IStorage>(out var storage))
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
        }

        public IInteractable GetCurrentInteractable()
        {
            _currentInteractable.TryGetComponent<IInteractable>(out var interactable);
            {
                return interactable;
            }
        }
    }
}

