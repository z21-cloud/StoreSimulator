using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace StoreSimulator.InteractableObjects
{
    public interface IStoreable
    {
        public IShelf CurrentShelf { get; }
        public void OnStored(GameObject slot);
        public GameObject OnPickedFromStore();
    }
}

