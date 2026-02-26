using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

namespace StoreSimulator.InteractableObjects
{
    [RequireComponent(typeof(Rigidbody))]
    public class HoldableObject : MonoBehaviour, IHoldable, IInteractable
    {
        [SerializeField] private ThrowableSettings throwableSettings;
        // for UI or Inventory
        public event Action OnHolding;

        private Rigidbody rb;

        public float ThrowForce => throwableSettings != null ? throwableSettings.GetThrowForce() : 1f;

        private void Awake()
        {
            // base set-up
            GetRigidbody();
        }


        public void Hold(Transform holdPoint)
        {
            SetParentPosition(holdPoint);

            // enable gravity & resets physics when hold object
            bool useGravity = false;
            SetPhysics(useGravity);
        }

        public void Release(Vector3 impulse)
        {
            if (rb == null) GetRigidbody();
            transform.parent = null;

            // enable gravity & resets physics when release object
            bool useGravity = true;
            SetPhysics(useGravity);

            if (impulse != Vector3.zero) rb.AddForce(impulse, ForceMode.Impulse);
        }

        private void SetPhysics(bool value)
        {
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            rb.useGravity = value;
        }

        private void SetParentPosition(Transform holdPoint)
        {
            transform.position = holdPoint.position;
            transform.rotation = holdPoint.rotation;
            transform.parent = holdPoint;
        }

        public string GetDescription()
        {
            throw new System.NotImplementedException();
        }

        private void GetRigidbody()
        {
            rb = GetComponent<Rigidbody>();
        }
    }
}

