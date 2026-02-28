using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace StoreSimulator.InteractableObjects
{
    public class StoreableItem : MonoBehaviour, IStoreable, IInteractable
    {
        [SerializeField] private ThrowableSettings throwableSettings;
        [SerializeField] private MoveToPosition mover;

        public bool IsStored { get; private set; }
        public ShellfSlot CurrentSlot { get; private set; }
        public float ThrowForce => throwableSettings != null ? throwableSettings.GetThrowForce() : 1f;

        private Rigidbody rb;

        #region holdable
        private void Awake()
        {
            // base set-up
            rb = GetComponent<Rigidbody>();
        }

        public void Hold(Transform holdPoint)
        {
            SetParentPosition(holdPoint);

            // enable gravity & resets physics when hold object
            bool isKinematic = true;
            SetPhysics(isKinematic);
        }

        public void Release(Vector3 impulse)
        {
            if (rb == null) Debug.LogError($"HoldableObject: {gameObject.name} rigidbody is null");
            transform.parent = null;

            // enable gravity & resets physics when release object
            bool isKinematic = false;
            SetPhysics(isKinematic);

            if (impulse != Vector3.zero) rb.AddForce(impulse, ForceMode.Impulse);
        }

        private void SetPhysics(bool value)
        {
            // rb.linearVelocity = Vector3.zero;
            // rb.angularVelocity = Vector3.zero;
            rb.isKinematic = value;
        }

        private void SetParentPosition(Transform holdPoint)
        {
            transform.position = holdPoint.position;
            transform.rotation = holdPoint.rotation;
            transform.parent = holdPoint;
        }

        #endregion

        public void OnStored(Transform storeSlot)
        {
            rb.isKinematic = true;

            CurrentSlot = storeSlot.GetComponent<ShellfSlot>();

            mover.MoveToSlotPosition(storeSlot);
            IsStored = true;
        }

        public void OnPickedFromStore()
        {
            mover.StopAllCoroutines();
            IsStored = false;
            CurrentSlot = null;
        }

        public string GetDescription()
        {
            throw new System.NotImplementedException();
        }
    }
}

