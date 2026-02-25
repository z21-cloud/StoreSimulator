using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace StoreSimulator.InteractableObjects
{
    public class PlayerInteraction : MonoBehaviour
    {
        [Header("Interaction distance")]
        [SerializeField] private float interactableDistance = 5f;
        [Header("Throw force")]
        [Tooltip("For throw throwable objects")]
        [SerializeField] private float throwForce = 1000f;
        [Header("Release force")]
        [Tooltip("For release holdable objects")]
        [SerializeField] private float releaseForce = 100f;
        [Header("Point for holding object")]
        [SerializeField] private Transform holdPoint;

        // consts
        private readonly Vector3 CENTER_OF_CAMERASCREEN = new Vector3(0.5f, 0.5f, 0); // center of camera screen
        private const float DIRECTION_CAMERA_OFFSET = 0.2f;
        // components
        private IInteractable _currentInteractable;
        private Transform _heldObject;
        //private GameObject _heldObjectGO;
        private Camera _mainCamera;

        private Vector3 throwVector;

        private void Awake()
        {
            _mainCamera = Camera.main;
        }

        private void Update()
        {
            _currentInteractable = DetectInteractable();
        }

        private IInteractable DetectInteractable()
        {
            // create ray from center of main camera
            Ray ray = _mainCamera.ViewportPointToRay(CENTER_OF_CAMERASCREEN);

            // if hits smth in interactableDistance
            if (!Physics.Raycast(ray, out RaycastHit hit, interactableDistance)) return null;
            
            // Draw line for debugging
            Debug.DrawLine(ray.origin, hit.point, Color.red);

            // return parent interface IInteractable, if gameobject has implementation
            if (hit.collider.TryGetComponent<IInteractable>(out var interactable))
                return interactable;

            // to prevert cases, where component in parent's object and collider inside parent's object
            if (hit.collider.GetComponentInParent<IInteractable>() is IInteractable parentInteractable)
                return parentInteractable;

            return null;
        }

        // IPickable & IHoldable inhirates from IInteractable, both got simular functional for UI
        public void DoInteract()
        {
            throwVector = (_mainCamera.transform.forward + Vector3.up * DIRECTION_CAMERA_OFFSET).normalized;
            // Drop or throw gameobject
            if (_heldObject != null)
            {
                if (_heldObject.TryGetComponent<IThrowable>(out var throwable))
                    throwable.Throw(throwVector, throwForce);

                if (_heldObject.TryGetComponent<IHoldable>(out var holdable))
                    holdable.Release(throwVector * releaseForce);

                // reset 
                _heldObject = null;
                //_heldObjectGO = null;
                return;
            }

            // 
            switch (_currentInteractable)
            {
                case IPickable pickable:
                    pickable.Pick();
                    break;

                case IHoldable holdable:
                    _heldObject = ((MonoBehaviour)holdable).transform;
                    //_heldObjectGO = ((MonoBehaviour)holdable).gameObject;
                    holdable.Hold(holdPoint);
                    break;
            }
        }

        public IInteractable GetCurrentInteractable() => _currentInteractable;
    }
}

