using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using StoreSimulator.StoreableItems;
using StoreSimulator.StoreUI;

namespace StoreSimulator.InteractableObjects
{
    public class PriceTag : MonoBehaviour, IInteractable, IPriceTag
    {
        [SerializeField] private PriceEditUI priceUI;

        private IStorage storage;

        public void Initialize(IStorage storage)
        {
            this.storage = storage;
        }

        public void DoInteract()
        {
            // if can't take item => storage is empty
            if (!storage.CanTakeItem()) return;

            if (storage.PeekItem().TryGetComponent<IStoreable>(out var storeable))
            {
                if (storeable.Data.SubCategory == ItemSubCategory.None) return;
            }


            if (storage is IPriceStorage priceStorage)
            {
                priceUI.OpenForStorage(priceStorage);
            }
            else
            {
                Debug.LogError($"[Storage]: Storage has no IPriceStorage Implementation, Error expected");
            }
        }

        public string GetDescription()
        {
            throw new System.NotImplementedException();
        }
    }
}

