using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace StoreSimulator.InteractableObjects
{
    public class PriceTag : MonoBehaviour, IInteractable, IPriceTag
    {
        [SerializeField] private Storage storage;
        [SerializeField] private PriceEditUI priceUI;

        public void DoInteract()
        {
            // if can't take item => storage is empty
            if (!storage.CanTakeItem()) return;

            priceUI.OpenForStorage(storage);
        }
        
        public string GetDescription()
        {
            throw new System.NotImplementedException();
        }
    }
}

