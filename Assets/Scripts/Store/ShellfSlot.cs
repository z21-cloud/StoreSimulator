using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace StoreSimulator.InteractableObjects
{
    public class ShellfSlot : MonoBehaviour, IShelf
    {
        public bool IsOccupied { get; private set; }
        private GameObject item;

        public void Occupy(GameObject item)
        {
            this.item = item;
            IsOccupied = true;

            if(item.TryGetComponent<IStoreable>(out var storeable))
            {
                storeable.OnStored(gameObject);
            }
        }

        public GameObject Release()
        {
            IsOccupied = false;
            return item;
        }
    }
}

