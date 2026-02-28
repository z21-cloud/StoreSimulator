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

        public bool CanPlaceItem(Vector3 playerPosition)
        {
            bool isOccupied = false;
            _reservedSlot = GetClosestSlot(playerPosition, isOccupied);
            Debug.Log($"Storage: slots are available = {_reservedSlot != null}");

            return _reservedSlot != null;
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
            bool isOccupied = true;
            ShellfSlot resultSlot = GetClosestSlot(playerPosition, isOccupied);

            if (resultSlot != null)
            {
                IStoreable storeable = resultSlot.Release();
                storeable.OnPickedFromStore();
                Debug.Log($"{resultSlot.name} : isOccupied {resultSlot.IsOccupied}");
                return storeable;
            }

            return null;
        }

        private ShellfSlot GetClosestSlot(Vector3 playerPosition, bool findOccupied)
        {
            ShellfSlot bestSlot = null;
            float minDistance = float.MaxValue;

            foreach (var slot in slots)
            {
                if (slot.IsOccupied != findOccupied) continue;
                

                float distance = (playerPosition - slot.transform.position).sqrMagnitude;

                if (distance < minDistance)
                {
                    minDistance = distance;
                    bestSlot = slot;
                }
            }

            return bestSlot;
        }

        public string GetDescription()
        {
            throw new System.NotImplementedException();
        }
    }
}

