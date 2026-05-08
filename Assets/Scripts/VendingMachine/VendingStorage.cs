using System;
using System.Collections.Generic;
using StoreSimulator.InteractableObjects;
using StoreSimulator.StoreableItems;
using UnityEngine;

public class VendingStorage : MonoBehaviour, IStorage
{
    [Header("Category settings")]
    [SerializeField] private bool allowAnyCategory = false;
    [SerializeField] private ItemCategory allowedCategory;
    [SerializeField] private List<ItemData> allowedSpecificItems;
    

    // [Header("Pick Up point for NPC's")]
    // [SerializeField] private Transform interactionPosition;

    // private vars
    private List<IShelf> _slots;
    private IStore _storeOwner;
    public Vector3 InteractionPoint => transform.position;

    void Awake()
    {
        _slots = new List<IShelf>(GetComponentsInChildren<IShelf>());
    }

    void OnEnable()
    {
        // Debug.Log($"Register storage");
        // StorageRegistry.Instance.RegisterStorage(this);
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

        return (allowedCategory & storable.Data.Category) != 0;
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
        }

        return taken;
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
        // StorageRegistry.Instance.UnregisterStorage(this);
    }
    public string GetDescription()
    {
        throw new NotImplementedException();
    }
}

