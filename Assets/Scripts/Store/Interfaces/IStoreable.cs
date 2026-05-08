using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using StoreSimulator.StoreableItems;

namespace StoreSimulator.InteractableObjects
{
    public interface IStoreable
    {
        public float LockedPrice { get; }
        public ItemData Data { get; }
        public IShelf CurrentShelf { get; }
        public void OnStored(GameObject slot);
        public GameObject OnPickedFromStore();
        public void ReturnToPool();
    }
}

