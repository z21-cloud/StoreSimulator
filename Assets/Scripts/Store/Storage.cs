using UnityEngine;
using System.Collections.Generic;
using System;
using StoreSimulator.StoreableItems;
using TMPro;

namespace StoreSimulator.InteractableObjects
{
    public class Storage : MonoBehaviour, IInteractable, IStorage
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

        // private vars
        private ItemSubCategory _currentSubCategory = ItemSubCategory.None;

        private void OnEnable() => priceManager.RegisterStorage(this);    

        private void OnDisable() => priceManager.UnregisterStorage(this);

        private void Start()
        {
            ResetPrice();
        }

        // use events instead
        public bool CanPlaceItem(GameObject item)
        {
            if (!HasFreeSlot()) return false;

            if (allowAnyCategory) return true;

            if (item.TryGetComponent<IStoreable>(out var storable))
            {
                if(allowedSpecificItems.Count > 0)
                {
                    return allowedSpecificItems.Contains(storable.Data);
                }
                
                // if shelf locked it's sub category:
                if(_currentSubCategory != ItemSubCategory.None)
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

            return false;
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

                UpdateStates(item);
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

                    if(minDistance > dist)
                    {
                        minDistance = dist;
                        bestSlot = slot;
                    }
                }
            }

            GameObject taken = null;
            if(bestSlot != null)
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
        }

        private void UpdateStates(GameObject item)
        {
            if (item.TryGetComponent<IStoreable>(out var storeable))
            {
                // Lock shelf's sub category
                if (_currentSubCategory == ItemSubCategory.None)
                {
                    _currentSubCategory = storeable.SubCategory;
                }
            }

            RefreshPriceFromManager();
        }

        public void RefreshPriceFromManager()
        {
            if (_currentSubCategory == ItemSubCategory.None) return;

            foreach (var slot in slots)
            {
                if (slot.IsOccupied && slot.GetStoredItem().TryGetComponent<IPricable>(out var pricable))
                {
                    float newPrice = priceManager.GetPriceForItem(slot.ItemData);
                    pricable.CurrentPrice = newPrice;
                    priceText.text = $"{newPrice}$";
                }
            }
        }

        private void ResetStates()
        {
            if (CanTakeItem()) return;

            ResetPrice();
            _currentSubCategory = ItemSubCategory.None;
        }

        private void ResetPrice()
        {
            priceText.text = "No items";
        }

        public string GetDescription()
        {
            throw new NotImplementedException();
        }

    }
}

