using System.Collections.Generic;
using StoreSimulator.InteractableObjects;
using UnityEngine;

public class VendingStorage : MonoBehaviour
{
    [SerializeField] private List<StoreableItem> assortment;
    private IStore _storeOwner;
    private int _soldCount;
    private List<IShelf> _slots;

    public Vector3 InteractionPoint => _storeOwner.DeliveryPoint.position;

    public void Initailize(IStore store)
    {
        _storeOwner = store;

        // _storeOwner.StorageRegistry.RegisterStorage(this);

        _slots = new List<IShelf>(GetComponentsInChildren<IShelf>());

        foreach (var slot in _slots)
        {
            slot.Initialize(_storeOwner.PriceProvider);
        }
    }

    /*public bool CanTakeItem()
    {

    }

    public bool HasFreeSlot()
    {

    }

    public GameObject PeekItem()
    {

    }

    public GameObject TakeItem(Vector3 interactionPoint)
    {

    }

    public bool CanPlaceItem(IStoreable storable)
    {
        throw new System.NotImplementedException();
    }

    public void PlaceItem(GameObject item)
    {
        throw new System.NotImplementedException();
    }*/
}

