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
        private GameObject _currentInteractable;
        private IHoldable _heldObject;
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
            if (hit.collider.GetComponentInParent<IInteractable>() is IInteractable parentInteractable)
                return hit.collider.gameObject.GetComponentInParent<GameObject>();

            return null;
        }

        // Interaction with gameobjects
        public void DoInteract()
        {
            // Drop or throw gameobject
            if (_heldObject != null)
            {
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
                _heldObject = holdable;
                holdable.Hold(holdPoint);
            }
        }

        private void ReleaseHoldableObject()
        {
            // vector for releasing holdable
            Vector3 throwVector = (_mainCamera.transform.forward +
                Vector3.up * DIRECTION_CAMERA_OFFSET).normalized;
            // get force from object
            float forceMultiplier = _heldObject.ThrowForce;
            // throw or release object (depends on force multiplier)
            _heldObject.Release(throwVector * releaseForce * forceMultiplier);

            // reset 
            _heldObject = null;
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

