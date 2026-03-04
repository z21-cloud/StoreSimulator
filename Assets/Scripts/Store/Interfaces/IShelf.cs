using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using StoreSimulator.StoreableItems;

namespace StoreSimulator.InteractableObjects
{
    public interface IShelf
    {
        public bool IsOccupied { get; }
        public ItemData ItemData { get; }
        public void Occupy(GameObject item);
        public GameObject GetStoredItem();
        public GameObject Release();
    }
}
