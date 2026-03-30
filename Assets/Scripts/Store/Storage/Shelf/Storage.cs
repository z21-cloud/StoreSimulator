using UnityEngine;
using System.Collections.Generic;
using System;
using StoreSimulator.StoreableItems;
using TMPro;
using StoreSimulator.StoreManager;

namespace StoreSimulator.InteractableObjects
{
    public class Storage : MonoBehaviour, IInteractable, IStorage, IPriceStorage
    {
        [Header("General settings")]
        [SerializeField] private List<ShellfSlot> slots;

        [Header("Category settings")]
        [SerializeField] private bool allowAnyCategory = false;
        [SerializeField] private ItemCategory allowedCategory;
        [SerializeField] private List<ItemData> allowedSpecificItems;

        [Header("Price visual")]
        [SerializeField] private TMP_Text priceText;

        [Header("Price Manager")]
        [SerializeField] private PricesManager priceManager;

        [Header("Pick Up point for NPC's")]
        [SerializeField] private Transform interactionPosition;

        // private vars
        private ItemSubCategory _currentSubCategory = ItemSubCategory.None;
        private ItemData _currentItemData;

        public Vector3 InteractionPoint => interactionPosition.position;

        void Awake()
        {
            ResetPrice();
        }

        void OnEnable()
        {
            // Debug.Log($"Register storage");
            StorageRegistry.Instance.RegisterStorage(this);
            priceManager = PricesManager.Instance;
        }

        // use events instead
        public bool CanPlaceItem(IStoreable storable)
        {
            if (!HasFreeSlot()) return false;

            if (allowAnyCategory) return true;

            if (allowedSpecificItems.Count > 0)
            {
                return allowedSpecificItems.Contains(storable.Data);
            }

            // if shelf locked it's sub category:
            if (_currentSubCategory != ItemSubCategory.None)
            {
                Debug.Log($"Storage: {storable} sub category = {(_currentSubCategory & storable.SubCategory) != 0}");
                return (((allowedCategory & storable.Category) != 0) && ((_currentSubCategory & storable.SubCategory) != 0));
            }
            // if shelf has no it's own sub category: 
            else
            {
                Debug.Log($"Storage: {storable} category = {(allowedCategory & storable.Category) != 0}");
                return (allowedCategory & storable.Category) != 0;
            }
        }

        public bool HasFreeSlot()
        {
            foreach (var slot in slots)
            {
                if (!slot.IsOccupied) return true;
            }
            return false;
        }

        public bool CanTakeItem()
        {
            foreach (var slot in slots)
            {
                if (slot.IsOccupied) return true;
            }
            return false;
        }

        public void PlaceItem(GameObject item) //Vector3 playerPosition, bool findOccupied, 
        {
            // Should I get closest to player slot to place item or not?

            // FIND FREE SLOT
            ShellfSlot reservedSlot = null;
            foreach (ShellfSlot slot in slots)
            {
                if (!slot.IsOccupied)
                {
                    reservedSlot = slot;
                }
            }

            // PLACE ITEM
            if (reservedSlot != null)
            {
                reservedSlot.Occupy(item);

                // updates current sub category
                UpdateCurrentCategory(item);

                //
                UpdatePriceVisual();

            }
        }

        public GameObject TakeItem(Vector3 interactionPoint)
        {
            ShellfSlot bestSlot = null;
            float minDistance = float.MaxValue;

            foreach (var slot in slots)
            {
                if (slot.IsOccupied)
                {
                    // optimisation
                    float dist = (interactionPoint - slot.transform.position).sqrMagnitude;

                    if (minDistance > dist)
                    {
                        minDistance = dist;
                        bestSlot = slot;
                    }
                }
            }

            GameObject taken = null;
            if (bestSlot != null)
            {
                taken = bestSlot.Release();

                ResetStates();
            }

            return taken;
        }

        public void OnPriceInputChanged(float newPrice)
        {
            if (_currentSubCategory == ItemSubCategory.None) return;

            priceManager.SetSubCategoryPrice(_currentSubCategory, newPrice);
            UpdatePriceVisual();
        }

        public float GetCurrentPrice()
        {
            if (_currentItemData == null) return 0f;

            float currentPrice = priceManager.GetPriceForItem(_currentItemData);
            return currentPrice;
        }

        public float GetBasePrice()
        {
            if (_currentItemData == null) return 0f;

            float basePrice = _currentItemData.BasePrice;
            return basePrice;
        }

        private void UpdateCurrentCategory(GameObject item)
        {
            if (item.TryGetComponent<IStoreable>(out var storeable))
            {
                // Lock shelf's sub category
                if (_currentSubCategory == ItemSubCategory.None)
                {
                    _currentSubCategory = storeable.SubCategory;
                    _currentItemData = storeable.Data;
                }
            }
        }

        private void UpdatePriceVisual()
        {
            if (_currentSubCategory == ItemSubCategory.None) return;

            float price = priceManager.GetPriceForItem(_currentItemData);
            priceText.text = $"{price:F2}$";
        }

        private void ResetStates()
        {
            if (CanTakeItem()) return;

            ResetPrice();
            _currentSubCategory = ItemSubCategory.None;
            _currentItemData = null;
        }

        private void ResetPrice()
        {
            priceText.text = "No items";
        }

        public ItemSubCategory GetItemSubCategory()
        {
            return _currentSubCategory;
        }

        public GameObject PeekItem()
        {
            foreach (var slot in slots)
            {
                if (slot.IsOccupied)
                    return slot.GetStoredItem();
            }

            return null;
        }

        public string GetDescription()
        {
            throw new NotImplementedException();
        }

        public void OnDisable()
        {
            StorageRegistry.Instance.UnregisterStorage(this);
        }
    }
}

