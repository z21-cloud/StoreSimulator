using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace StoreSimulator.InteractableObjects
{
    public interface IStoreable
    {
        public Transform itemTransform { get; }
        public void OnStored(Transform shelfPoint);
    }
}
