using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace StoreSimulator.PickableObjects
{
    public interface IPickable : IInteractable
    {
        public void Pick();
    }
}
