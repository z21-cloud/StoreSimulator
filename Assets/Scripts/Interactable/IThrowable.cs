using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace StoreSimulator.InteractableObjects
{
    public interface IThrowable
    {
        public void Throw(Vector3 direction, float force);
    }
}

