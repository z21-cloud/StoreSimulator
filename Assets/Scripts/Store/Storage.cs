using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
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

        private void Start()
        {
            UpdatePrice();
        }

        // use events instead
        public bool CanPlaceItem(GameObject item)
        {
            if (!HasFreeSlot()) return false;

            if (allowAnyCategory) return true;

            if (item.TryGetComponent<StoreableItem>(out var storable))
            {
                if(allowedSpecificItems.Count > 0)
                {
                    return allowedSpecificItems.Contains(storable.Data);
                }

                Debug.Log($"Storage: {storable.gameObject.name} category = {(allowedCategory & storable.Category) != 0}");
                return (allowedCategory & storable.Category) != 0;
            }

            return false;
        }

        private bool HasFreeSlot()
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
                if(item.TryGetComponent<IPricable>(out var pricable))
                {
                    priceText.text = $"{pricable.CurrentPrice}$";
                }
                //UpdateStates();
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
                UpdatePrice();
            }

            return taken;
        }

        private void UpdatePrice()
        {
            if (CanTakeItem()) return;

            priceText.text = "No items";
        }

        public string GetDescription()
        {
            throw new NotImplementedException();
        }

    }
}

