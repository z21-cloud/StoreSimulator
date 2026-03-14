using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using StoreSimulator.InteractableObjects;
using System;
using StoreSimulator.Boxes;

namespace StoreSimulator.StoreableItems
{
    public class BoxStorage : MonoBehaviour, IHoldable, IInteractable, IStorage
    {
        [SerializeField] private ThrowableSettings throwableSettings;
        [SerializeField] private List<SlotGroup> groups;
        
        private List<ShellfSlot> _slots = new List<ShellfSlot>();
        private ItemCategory _allowedCategory;
        private Rigidbody _rb;
        private Collider _itemCollider;
        private IBoxOwner _owner;
        public float ThrowForce => throwableSettings != null ? throwableSettings.GetThrowForce() : 1f;
        
        private void Awake()
        {
            _rb = GetComponent<Rigidbody>();
            _itemCollider = GetComponent<Collider>();
        }

        public void Initialize(DeliveryOrder order)
        {
            _allowedCategory = order.ItemData.Category;

            foreach (var group in groups)
            {
                bool match = ((group.Preset.Category & order.ItemData.Category) != 0) &&
                             ((group.Preset.SubCategory & order.ItemData.SubCategory) != 0);

                if (match)
                {
                    _slots = group.Slots;
                    group.gameObject.SetActive(match);
                    break;
                }
            }

            if (_slots.Count == 0) return;
            for (int i = 0; i < order.Quantity; i++)
            {
                //_itemPrefab.GetComponent<Rigidbody>().isKinematic = true;
                //_itemPrefab.GetComponent<Collider>().enabled = false;
                GameObject item = Instantiate
                    (
                    order.ItemData.Prefab, 
                    _slots[i].transform.position, 
                    Quaternion.identity
                    );

                item.GetComponent<Rigidbody>().isKinematic = true;
                item.GetComponent<Collider>().enabled = false;
                _slots[i].Occupy(item);
            }
        }

        public void SetOwner(IBoxOwner owner) => _owner = owner;

        #region HOLDABLE
        public void Hold(Transform holdPoint)
        {
            _owner?.OnBoxRemoved(gameObject);
            _owner = null;
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
            if(!CanTakeItem())
            {
                return FindMatchingGroup(storeable) != null;
            }

            Debug.Log($"BoxStorage: ItemCategory & AllowedCategory - {(_allowedCategory & storeable.Category) != 0}");
            if ((_allowedCategory & storeable.Category) == 0) return false;

            GameObject exsistingItem = PeekItem();

            if (exsistingItem != null && exsistingItem.TryGetComponent<IStoreable>(out var existingStoreable))
            {
                return (existingStoreable.SubCategory & storeable.SubCategory) != 0;
            }

            Debug.Log($"BoxStorage: Has free slot - {HasFreeSlot()}");
            return HasFreeSlot();
        }

        // if full
        public bool HasFreeSlot()
        {
            foreach (var slot in _slots)
            {
                if (!slot.IsOccupied) return true;
            }

            return false;
        }

        // if empty
        public bool CanTakeItem()
        {
            foreach (var slot in _slots)
            {
                if (slot.IsOccupied) return true;
            }
            return false;
        }

        public void PlaceItem(GameObject item)
        {
            if(!CanTakeItem() && item.TryGetComponent<IStoreable>(out var storeable))
            {
                SwitchSlotGroup(storeable);
            }

            foreach (var slot in _slots)
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

        private void SwitchSlotGroup(IStoreable storeable)
        {
            SlotGroup matched = FindMatchingGroup(storeable);
            if (matched == null) return;

            foreach (var group in groups)
            {
                group.gameObject.SetActive(false);
            }

            matched.gameObject.SetActive(true);
            _slots = matched.Slots;
            _allowedCategory = storeable.Category;
        }

        private SlotGroup FindMatchingGroup(IStoreable storeable)
        {
            foreach (var group in groups)
            {
                if (((group.Preset.Category & storeable.Category) != 0) &&
                    ((group.Preset.SubCategory & storeable.SubCategory) != 0))
                {
                    return group;
                }
            }

            return null;
        }

        // give item to caller (unlink from CurrentSlot)
        public GameObject TakeItem(Vector3 interactionPoint)
        {
            foreach (var slot in _slots)
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
            foreach (var slot in _slots)
            {
                if (slot.IsOccupied)
                    return slot.GetStoredItem();
            }

            return null;
        }
    }
}
