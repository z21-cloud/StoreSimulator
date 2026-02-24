using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace StoreSimulator.PickableObjects
{
    public class PlayerInteraction : MonoBehaviour
    {
        [SerializeField] private float interactableDistance = 5f;
        [SerializeField] private Transform holdPoint;

        private const float CENTER_CAMERA = 0.5f;
        private Camera _mainCamera;

        private IInteractable _currentInteractable;

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
            Ray ray = _mainCamera.ViewportPointToRay(new Vector3(CENTER_CAMERA, CENTER_CAMERA, 0));

            if (Physics.Raycast(ray, out RaycastHit hit, interactableDistance))
            {
                Debug.DrawLine(ray.origin, hit.point, Color.red);
                return hit.collider.GetComponent<IInteractable>();
            }

            return null;
        }

        public void DoInteract()
        {
            if (_currentInteractable == null) return;

            if(_currentInteractable is IPickable pickable)
            {
                pickable.Pick();
            }
            else if(_currentInteractable is IHoldable holdable)
            {
                holdable.Hold(holdPoint);
            }
        }

        public IInteractable GetCurrentInteractable() => _currentInteractable;
    }
}

