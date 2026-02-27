using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace StoreSimulator.InteractableObjects
{
    public class Storage : MonoBehaviour, IInteractable, IStorage
    {
        [SerializeField] private List<ShellfSlot> slots;

        private ShellfSlot _reservedSlot;
        
        public bool IsEmpty
        {
            get
            {
                foreach (var slot in slots)
                {
                    if (slot.IsOccupied) return false;
                }
                return true;
            }
        }

        public bool CanPlaceItem(IStoreable item)
        {
            foreach (var slot in slots)
            {
                if(!slot.IsOccupied)
                {
                    _reservedSlot = slot;
                    return true;
                }
            }

            Debug.Log($"Storage: all slots are not available");
            return false;
        }

        public void PlaceItem(IStoreable item)
        {
            if (_reservedSlot.IsOccupied || _reservedSlot == null) return;
            
            _reservedSlot.Occupy(item);
            Debug.Log($"{_reservedSlot.gameObject.name} : isOccupied {_reservedSlot.IsOccupied}");

            item.OnStored(_reservedSlot.transform);

            
            _reservedSlot = null;
        }

        public IHoldable GetPlacedItem()
        {
            foreach (var slot in slots)
            {
                if (slot.IsOccupied)
                {
                    IStoreable storeable = slot.Release();
                    storeable.OnPickedFromStore();
                    
                    Debug.Log($"{slot.name} : isOccupied {slot.IsOccupied}");
                    // StoreableItem requires HoldableObject, so
                    return storeable as IHoldable;
                }
            }

            return null;
        }

        private void GetCloseToPlayerItem()
        {

        }

        public string GetDescription()
        {
            throw new System.NotImplementedException();
        }
    }
}

