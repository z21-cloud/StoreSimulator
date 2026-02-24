using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace StoreSimulator.PickableObjects
{
    public interface IHoldable : IInteractable
    {
        public void Hold(Transform holdPoint);
        public void Release();
    }
}

