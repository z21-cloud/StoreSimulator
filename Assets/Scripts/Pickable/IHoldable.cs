using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace StoreSimulator.PickableObjects
{
    public interface IHoldable : IPickable
    {
        public void Hold(Transform holdPoint);
        public void Release();
    }
}

