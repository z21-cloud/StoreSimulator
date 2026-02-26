using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using StoreSimulator.PhysicalObjects;

namespace StoreSimulator.InteractableObjects
{
    [RequireComponent(typeof(GetRigidBody), typeof(ResetObjectPhysics), typeof(SetObjectParentPosition))]
    public class HoldableObject : MonoBehaviour, IHoldable, IInteractable
    {
        [SerializeField] private ThrowableSettings throwableSettings;
        // for UI or Inventory
        public event Action OnHolding;

        private GetRigidBody getRigidBody;
        private Rigidbody rb;
        private SetObjectParentPosition setParentPositon;
        private ResetObjectPhysics resetObjectPhysics;

        public float ThrowForce => throwableSettings != null ? throwableSettings.GetThrowForce() : 1f;

        private void Awake()
        {
            // base set-up
            GetRigidbody();

            setParentPositon = GetComponent<SetObjectParentPosition>();
            resetObjectPhysics = GetComponent<ResetObjectPhysics>();
        }


        public void Hold(Transform holdPoint)
        {
            setParentPositon.SetParentPosition(holdPoint);

            // enable gravity & resets physics when hold object
            bool useGravity = false;
            resetObjectPhysics.SetPhysics(rb, useGravity);
        }

        public void Release(Vector3 impulse)
        {
            if (rb == null) GetRigidbody();
            transform.parent = null;

            // enable gravity & resets physics when release object
            bool useGravity = true;
            resetObjectPhysics.SetPhysics(rb, useGravity);

            if (impulse != Vector3.zero) rb.AddForce(impulse, ForceMode.Impulse);
        }
        
        public string GetDescription()
        {
            throw new System.NotImplementedException();
        }

        private void GetRigidbody()
        {
            getRigidBody = GetComponent<GetRigidBody>();
            rb = getRigidBody.GetRB;
        }
    }
}

