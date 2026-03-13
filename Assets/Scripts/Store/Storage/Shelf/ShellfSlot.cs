using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using StoreSimulator.StoreableItems;

namespace StoreSimulator.InteractableObjects
{
    public class ShellfSlot : MonoBehaviour, IShelf
    {
        public ItemData ItemData { get; private set; }
        public bool IsOccupied { get; private set; }
        private GameObject item;

        public void Occupy(GameObject item)
        {
            this.item = item;
            IsOccupied = true;

            if(item.TryGetComponent<IStoreable>(out var storeable))
            {
                storeable.OnStored(gameObject);
                ItemData = storeable.Data;
            }
        }

        public GameObject GetStoredItem()
        {
            return item;
        }

        public GameObject Release()
        {
            IsOccupied = false;
            return item;
        }
    }
}

