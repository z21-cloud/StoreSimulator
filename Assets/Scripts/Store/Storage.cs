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
                if (!slot.IsOccupied)
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

        public IStoreable GetPlacedItem(Vector3 playerPosition)
        {
            ShellfSlot resultSlot = GetCloseToPlayerItem(playerPosition);

            if (resultSlot != null)
            {
                IStoreable storeable = resultSlot.Release();
                storeable.OnPickedFromStore();
                Debug.Log($"{resultSlot.name} : isOccupied {resultSlot.IsOccupied}");
                return storeable;
            }

            return null;
        }

        private ShellfSlot GetCloseToPlayerItem(Vector3 playerPosition)
        {
            Vector3 minDistance = slots[0].transform.position;
            ShellfSlot resultSlot = slots[0];
            
            if (!resultSlot.IsOccupied) return null;

            for (int i = 1; i < slots.Count; i++)
            {
                if (slots[i].IsOccupied &&
                    (Vector3.Distance(playerPosition, minDistance) >
                (Vector3.Distance(playerPosition, slots[i].transform.position))))
                {
                    minDistance = slots[i].transform.position;
                    resultSlot = slots[i];
                }
            }

            return resultSlot;
        }

        public string GetDescription()
        {
            throw new System.NotImplementedException();
        }
    }
}

