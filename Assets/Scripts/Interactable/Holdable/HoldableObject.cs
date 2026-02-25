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

        private void GetRigidbody()
        {
            rb = GetComponent<Rigidbody>();
        }

        public string GetDescription()
        {
            throw new System.NotImplementedException();
        }

        public void Hold(Transform holdPoint)
        {
            SetHoldPosition(holdPoint);
            SetPhysics();
        }

        private void SetPhysics()
        {
            rb.linearVelocity = new Vector3(0, 0);
            rb.useGravity = false;
        }

        private void SetHoldPosition(Transform holdPoint)
        {
            transform.position = holdPoint.position;
            transform.rotation = holdPoint.rotation;
            transform.parent = holdPoint;
        }

        public void Release(Vector3 impulse)
        {
            if (rb == null) GetRigidbody();
            transform.parent = null;

            rb.useGravity = true;
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;

            if (impulse != Vector3.zero) rb.AddForce(impulse, ForceMode.Impulse);
        }
    }
}

