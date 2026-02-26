using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

namespace StoreSimulator.InteractableObjects
{
    public interface IHoldable
    {
        public float ThrowForce { get; }
        public void Hold(Transform holdPoint);
        public void Release(Vector3 impulse);
    }
}

