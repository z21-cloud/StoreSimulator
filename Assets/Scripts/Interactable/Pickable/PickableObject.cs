using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

namespace StoreSimulator.InteractableObjects
{
    public class PickableObject : MonoBehaviour, IPickable, IInteractable
    {
        public string GetDescription()
        {
            throw new System.NotImplementedException();
        }

        public void Pick()
        {
            gameObject.SetActive(false);
        }
    }
}
