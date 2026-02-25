using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

namespace StoreSimulator.InteractableObjects
{
    [RequireComponent(typeof(Rigidbody))]
    public class HoldableObject : MonoBehaviour, IHoldable
    {
        // for UI or Inventory
        public event Action OnHolding;

        private Rigidbody rb;


        private void Awake()
        {
            GetRigidbody();
        }

        public string GetDescription()
        {
            throw new System.NotImplementedException();
        }

        public void Hold(Transform holdPoint)
        {
            SetHoldPosition(holdPoint);

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

        private void SetHoldPosition(Transform holdPoint)
        {
            transform.position = holdPoint.position;
            transform.rotation = holdPoint.rotation;
            transform.parent = holdPoint;
        }

        private void GetRigidbody()
        {
            rb = GetComponent<Rigidbody>();
        }
    }
}

