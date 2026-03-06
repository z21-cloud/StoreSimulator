using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using StoreSimulator.InteractableObjects;

namespace StoreSimulator.StoreableItems
{
    public class BoxStorage : MonoBehaviour, IHoldable, IInteractable, IStorage
    {
        [SerializeField] private BoxData boxData;
        [SerializeField] private List<ShellfSlot> slots;

        [SerializeField] private ThrowableSettings throwableSettings;
        private Rigidbody _rb;
        private Collider _itemCollider;
        private GameObject _itemPrefab;
        public float ThrowForce => throwableSettings != null ? throwableSettings.GetThrowForce() : 1f;
        private void Awake()
        {
            _rb = GetComponent<Rigidbody>();
            _itemCollider = GetComponent<Collider>();
            _itemPrefab = boxData.ItemPrefab;
        }

        private void Start()
        {
            for (int i = 0; i < boxData.BoxMaxCapacity; i++)
            {
                //_itemPrefab.GetComponent<Rigidbody>().isKinematic = true;
                //_itemPrefab.GetComponent<Collider>().enabled = false;
                GameObject item = Instantiate(_itemPrefab, slots[i].transform.position, Quaternion.identity);
                item.GetComponent<Rigidbody>().isKinematic = true;
                item.GetComponent<Collider>().enabled = false;
                slots[i].Occupy(item);
            }
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

        public string GetDescription()
        {
            throw new System.NotImplementedException();
        }
        #endregion

        // can place item back in box
        public bool CanPlaceItem(IStoreable storeable)
        {
            Debug.Log($"BoxStorage: Has free slot - {HasFreeSlot()}");
            if(!HasFreeSlot()) return false;

            Debug.Log($"BoxStorage: ItemCategory & AllowedCategory - {(boxData.AllowedCategory & storeable.Category) != 0}");
            if ((boxData.AllowedCategory & storeable.Category) == 0) return false;

            GameObject exsistingItem = PeekItem();

            if (exsistingItem != null && exsistingItem.TryGetComponent<IStoreable>(out var existingStoreable))
            {
                return (existingStoreable.SubCategory & storeable.SubCategory) != 0;
            }

            return true;
        }

        // if full
        public bool HasFreeSlot()
        {
            foreach (var slot in slots)
            {
                if (!slot.IsOccupied) return true;
            }

            return false;
        }

        // if empty
        public bool CanTakeItem()
        {
            foreach (var slot in slots)
            {
                if (slot.IsOccupied) return true;
            }
            return false;
        }

        public void PlaceItem(GameObject item)
        {
            foreach (var slot in slots)
            {
                Debug.Log($"BoxStorage: Try find slot");
                if(!slot.IsOccupied)
                {
                    item.GetComponent<Rigidbody>().isKinematic = true;
                    item.GetComponent<Collider>().enabled = false;

                    Debug.Log($"Box storage: PlaceItem {item.name} in slot {slot.name} ; slot isOccupied {slot.IsOccupied}");
                    slot.Occupy(item);
                    return;
                }
            }
        }

        // give item to caller (unlink from CurrentSlot)
        public GameObject TakeItem(Vector3 interactionPoint)
        {
            foreach (var slot in slots)
            {
                if (slot.IsOccupied)
                {
                    return slot.Release();
                }
            }

            return null;
        }

        // give item to caller (without unlink from CurrentSlot) 
        public GameObject PeekItem()
        {
            foreach (var slot in slots)
            {
                if (slot.IsOccupied)
                    return slot.GetStoredItem();
            }

            return null;
        }
    }
}
