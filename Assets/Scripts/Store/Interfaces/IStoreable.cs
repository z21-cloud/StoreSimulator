using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using StoreSimulator.StoreableItems;

namespace StoreSimulator.InteractableObjects
{
    public interface IStoreable
    {
        public ItemData Data { get; }
        public ItemCategory Category { get; }
        public ItemSubCategory SubCategory { get; }
        public IShelf CurrentShelf { get; }
        public void OnStored(GameObject slot);
        public GameObject OnPickedFromStore();
    }
}

