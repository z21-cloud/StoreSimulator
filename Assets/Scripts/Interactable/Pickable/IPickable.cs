using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

namespace StoreSimulator.InteractableObjects
{
    public interface IPickable : IInteractable
    {
        public event Action OnPicked;
        public void Pick();
    }
}
