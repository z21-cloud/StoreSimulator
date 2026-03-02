using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace StoreSimulator.InteractableObjects
{
    public class StoreableItem : MonoBehaviour, IInteractable, IStoreable, IHoldable
    {
        [SerializeField] private MoveToPosition mover;
        [SerializeField] private ThrowableSettings throwableSettings;

        public IShelf CurrentShelf { get; private set; }
        
        public float ThrowForce => throwableSettings != null ? throwableSettings.GetThrowForce() : 1f;

        private Rigidbody rb;

        private void Awake()
        {
            rb = GetComponent<Rigidbody>();
        }

        public void OnStored(GameObject slot)
        {
            if (slot.TryGetComponent<IShelf>(out var shelf))
            {
                rb.isKinematic = true;
                CurrentShelf = shelf;
                mover.MoveToSlotPosition(slot.transform);
            }
            else
            {
                Debug.LogWarning($"StoreableItem: {gameObject.name} - {slot.gameObject.name} doesn't have IShelf");
            }
        }

        public GameObject OnPickedFromStore()
        {
            if(CurrentShelf == null)
            {
                Debug.LogWarning($"StoreableItem: {gameObject.name} - CurrentShelf is null");
                return null;
            }

            mover.StopAllCoroutines();
            CurrentShelf.Release();
            CurrentShelf = null;

            return gameObject;
        }

        public string GetDescription()
        {
            throw new System.NotImplementedException();
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
            transform.parent = holdPoint;
            transform.position = holdPoint.position;
            transform.rotation = holdPoint.rotation;
        }
    }
}

