using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace StoreSimulator.InteractableObjects
{
    public class PlayerThrowHandler : MonoBehaviour
    {
        [Header("Release force")]
        [Tooltip("For release holdable objects")]
        [SerializeField] private float releaseForce = 100f;
        
        // consts
        private const float DIRECTION_CAMERA_OFFSET = 0.2f;

        private Camera _mainCamera;

        private void Awake()
        {
            _mainCamera = Camera.main;
        }

        public bool DoThrow(GameObject heldObject)
        {
            if (heldObject != null)
            {
                ReleaseHoldableObject(heldObject);
                return true;
            }
            return false;
        }

        private void ReleaseHoldableObject(GameObject heldObject)
        {
            if (heldObject.TryGetComponent<IHoldable>(out var holdable))
            {
                // vector for releasing holdable
                Vector3 throwVector = (_mainCamera.transform.forward +
                    Vector3.up * DIRECTION_CAMERA_OFFSET).normalized;
                // get force from object
                float forceMultiplier = holdable.ThrowForce;
                // throw or release object (depends on force multiplier)
                holdable.Release(throwVector * releaseForce * forceMultiplier);
            }
        }
    }
}

