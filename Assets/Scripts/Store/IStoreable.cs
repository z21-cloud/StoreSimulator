using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace StoreSimulator.InteractableObjects
{
    public interface IStoreable
    {
        public bool IsStored { get; }
        public float ThrowForce { get; }
        public ShellfSlot CurrentSlot { get; }
        public void OnStored(Transform shelfPoint);
        public void OnPickedFromStore();
        public void Hold(Transform holdPoint);
        public void Release(Vector3 impulse);
    }
}
