using UnityEngine;
using System.Collections.Generic;
using System;
using StoreSimulator.StoreableItems;
using TMPro;

namespace StoreSimulator.InteractableObjects
{
    public class Storage : MonoBehaviour, IInteractable, IStorage, IPriceStorage
    {
        [Header("Category settings")]
        [SerializeField] private bool allowAnyCategory = false;
        [SerializeField] private ItemCategory allowedCategory;
        [SerializeField] private List<ItemData> allowedSpecificItems;

        [Header("Price visual")]
        [SerializeField] private TMP_Text priceText;

        // [Header("Pick Up point for NPC's")]
        // [SerializeField] private Transform interactionPosition;

        // private vars
        private ItemSubCategory _currentSubCategory = ItemSubCategory.None;
        private ItemData _currentItemData;
        private IStore _storeOwner;
        private List<IShelf> _slots;
        public Vector3 InteractionPoint => transform.position;

        void Awake()
        {
            ResetPrice();
            _storeOwner = GetComponentInParent<IStore>();

            _slots = new List<IShelf>(GetComponentsInChildren<IShelf>());

            foreach (var slot in _slots)
            {
                slot.Initialize(_storeOwner.PriceProvider);
            }

            IPriceTag priceTag = GetComponentInChildren<IPriceTag>();
            priceTag.Initialize(this);
        }

        void OnEnable()
        {
            // Debug.Log($"Register storage");
            _storeOwner.StorageRegistry.RegisterStorage(this);
            // StorageRegistry.Instance.RegisterStorage(this);
        }

        private float GetPrice()
        {
            float result = _storeOwner.PriceProvider.GetPrice(_currentItemData);
            Debug.Log($"[Storage]: полученная цена {result}");
            return result;
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
                // Debug.Log($"Storage: {storable} sub category = {(_currentSubCategory & storable.SubCategory) != 0}");
                return (((allowedCategory & storable.Data.Category) != 0) && ((_currentSubCategory & storable.Data.SubCategory) != 0));
            }
            // if shelf has no it's own sub category: 
            else
            {
                // Debug.Log($"Storage: {storable} category = {(allowedCategory & storable.Category) != 0}");
                return (allowedCategory & storable.Data.Category) != 0;
            }
        }

        public bool HasFreeSlot()
        {
            foreach (IShelf slot in _slots)
            {
                if (!slot.IsOccupied) return true;
            }
            return false;
        }

        public bool CanTakeItem()
        {
            foreach (IShelf slot in _slots)
            {
                if (slot.IsOccupied) return true;
            }
            return false;
        }

        public void PlaceItem(GameObject item) //Vector3 playerPosition, bool findOccupied, 
        {
            // Should I get closest to player slot to place item or not?

            // FIND FREE SLOT
            IShelf reservedSlot = null;
            foreach (IShelf slot in _slots)
            {
                if (!slot.IsOccupied)
                {
                    reservedSlot = slot;
                }
            }

            // PLACE ITEM
            if (reservedSlot != null && item.TryGetComponent<IStoreable>(out var storeable))
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
            IShelf bestSlot = null;
            float minDistance = float.MaxValue;

            foreach (IShelf slot in _slots)
            {
                if (slot.IsOccupied)
                {
                    // optimisation
                    float dist = (interactionPoint - ((MonoBehaviour)slot).transform.position).sqrMagnitude;

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

                if (taken.TryGetComponent<IStoreable>(out var storeable))
                {
                    storeable.OnPickedFromStore();
                }

                ResetStates();
            }

            return taken;
        }


        public void OnPriceInputChanged(float newPrice)
        {
            if (_currentSubCategory == ItemSubCategory.None) return;

            _storeOwner.PriceProvider.SetPrice(_currentSubCategory, newPrice);

            Debug.Log($"[Storage]: New price for {_currentSubCategory} is {newPrice}");

            UpdatePriceVisual();
        }

        public float GetCurrentPrice()
        {
            return _currentItemData == null ? 0f : GetPrice();
        }

        public float GetBasePrice()
        {
            return _currentItemData == null ? 0f : GetMarketPrice();
        }

        private float GetMarketPrice() => _storeOwner.PriceProvider.GetMarketPrice(_currentItemData);

        private void UpdateCurrentCategory(GameObject item)
        {
            if (item.TryGetComponent<IStoreable>(out var storeable))
            {
                // Lock shelf's sub category
                if (_currentSubCategory == ItemSubCategory.None)
                {
                    _currentSubCategory = storeable.Data.SubCategory;
                    _currentItemData = storeable.Data;
                }
            }
        }

        private void UpdatePriceVisual()
        {
            if (_currentSubCategory == ItemSubCategory.None) return;
            priceText.text = $"{GetPrice():F2}$";
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

        public GameObject PeekItem()
        {
            foreach (IShelf slot in _slots)
            {
                if (slot.IsOccupied)
                    return slot.GetStoredItem();
            }

            return null;
        }

        private void OnDisable()
        {
            _storeOwner.StorageRegistry.UnregisterStorage(this);
            // StorageRegistry.Instance.UnregisterStorage(this);
        }
        public string GetDescription()
        {
            throw new NotImplementedException();
        }
    }
}

