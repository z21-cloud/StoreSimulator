using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace StoreSimulator.InteractableObjects
{
    public interface IShelf
    {
        public bool IsOccupied { get; }
        public void Occupy(GameObject item);
        public GameObject Release();
    }
}
