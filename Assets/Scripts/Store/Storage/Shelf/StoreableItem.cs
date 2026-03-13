using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using StoreSimulator.StoreableItems;

namespace StoreSimulator.InteractableObjects
{
    public class StoreableItem : MonoBehaviour, IInteractable, IStoreable, IHoldable
    {
        [Header("Move settings")]
        [SerializeField] private MoveToPosition mover;
        [Header("Throw settings")]
        [SerializeField] private ThrowableSettings throwableSettings;
        [Header("Item's data settings")]
        [SerializeField] private ItemData itemData;

        // private components
        private Rigidbody _rb;
        private Collider _itemCollider;

        // properties
        // Linke Item -> Shelf
        public IShelf CurrentShelf { get; private set; }
        // To get Item's Data
        public ItemData Data => itemData;
        public ItemCategory Category => itemData != null ? itemData.Category : ItemCategory.None;
        public ItemSubCategory SubCategory => itemData != null ? itemData.SubCategory : ItemSubCategory.None;
        // ThrowForce
        public float ThrowForce => throwableSettings != null ? throwableSettings.GetThrowForce() : 1f;

        private void Awake()
        {
            // cache
            _rb = GetComponent<Rigidbody>();
            _itemCollider = GetComponent<Collider>();
        }

        public void OnStored(GameObject slot)
        {
            if (slot.TryGetComponent<IShelf>(out var shelf))
            {
                // keep it for now, delete later
                // don't need this line, cause I enable kinematic only on SetPhysics method
                //rb.isKinematic = true;
                
                // Link shelf and item
                CurrentShelf = shelf;

                // move to slot position and rotation (coroutine)
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

            // stop coroutine to prevert issue, when player goes back, and object follow him
            mover.StopAllCoroutines();

            // reset shelf
            CurrentShelf.Release();
            CurrentShelf = null;

            return gameObject;
        }

        public string GetDescription()
        {
            throw new System.NotImplementedException();
        }

        #region HOLDABLE
        public void Hold(Transform holdPoint)
        {
            // set object to player's hand
            SetParentPosition(holdPoint);

            // enable gravity & resets physics when hold object
            bool isKinematic = true;
            SetPhysics(isKinematic);
        }

        public void Release(Vector3 impulse)
        {
            if (_rb == null) Debug.LogError($"HoldableObject: {gameObject.name} rigidbody is null");
            transform.parent = null;

            // enable gravity & resets physics when release object
            bool isKinematic = false;
            SetPhysics(isKinematic);

            // throw object in camera direction
            if (impulse != Vector3.zero) _rb.AddForce(impulse, ForceMode.Impulse);
        }

        private void SetPhysics(bool value)
        {
            // should I reset velocity or not?
            // rb.linearVelocity = Vector3.zero;
            // rb.angularVelocity = Vector3.zero;

            _rb.isKinematic = value;

            /* 
             * kinematic = false => object in world, need collider = true
             * kinematic = true => object in hands, need collider = false
             */
            _itemCollider.enabled = !value;
        }

        private void SetParentPosition(Transform holdPoint)
        {
            transform.parent = holdPoint;
            transform.position = holdPoint.position;
            transform.rotation = holdPoint.rotation;
        }
        #endregion
    }
}

