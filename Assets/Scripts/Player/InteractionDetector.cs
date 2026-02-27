using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace StoreSimulator.InteractableObjects
{
    public class InteractionDetector : MonoBehaviour
    {
        [Header("Interaction distance")]
        [SerializeField] private float interactableDistance = 5f;
        public GameObject CurrentInteractable { get; private set; }

        // consts
        private readonly Vector3 CENTER_OF_CAMERASCREEN = new Vector3(0.5f, 0.5f, 0); // center of camera screen

        // components
        private Camera _mainCamera;

        private void Awake()
        {
            _mainCamera = Camera.main;
        }

        // Update is called once per frame
        private void Update()
        {
            CurrentInteractable = DetectInteractable();
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
                return hit.collider.transform.parent.gameObject;

            return null;
        }
    }
}