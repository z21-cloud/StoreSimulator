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
            _reservedSlot = GetClosestToPlayerFree(playerPosition);
            
            if (_reservedSlot == null)
            {
                Debug.Log($"Storage: all slots are not available");
                return false;
            }

            return true;
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
            ShellfSlot resultSlot = GetClosestToPlayerOccupied(playerPosition);

            if (resultSlot != null)
            {
                IStoreable storeable = resultSlot.Release();
                storeable.OnPickedFromStore();
                Debug.Log($"{resultSlot.name} : isOccupied {resultSlot.IsOccupied}");
                return storeable;
            }

            return null;
        }

        private ShellfSlot GetClosestToPlayerOccupied(Vector3 playerPosition)
        {
            Vector3 minDistance = slots[0].transform.position;
            ShellfSlot resultSlot = slots[0];


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

            if (!resultSlot.IsOccupied) return null;
            return resultSlot;
        }

        private ShellfSlot GetClosestToPlayerFree(Vector3 playerPosition)
        {
            Vector3 minDistance = slots[0].transform.position;
            ShellfSlot resultSlot = slots[0];

            for (int i = 1; i < slots.Count; i++)
            {
                if (!slots[i].IsOccupied &&
                (Vector3.Distance(playerPosition, minDistance) >
                (Vector3.Distance(playerPosition, slots[i].transform.position))))
                {
                    minDistance = slots[i].transform.position;
                    resultSlot = slots[i];
                }
            }

            if (resultSlot.IsOccupied) return null;
            return resultSlot;
        }

        /*private ShellfSlot GetClosestToPlayerPosition(Vector3 playerPosition, List<ShellfSlot> slots)
        {
            Vector3 minDistance = slots[0].transform.position;
            ShellfSlot resultSlot = slots[0];

            for (int i = 1; i < slots.Count; i++)
            {
                if ((Vector3.Distance(playerPosition, minDistance) >
                (Vector3.Distance(playerPosition, slots[i].transform.position))))
                {
                    minDistance = slots[i].transform.position;
                    resultSlot = slots[i];
                }
            }

            return resultSlot;
        }*/

        public string GetDescription()
        {
            throw new System.NotImplementedException();
        }
    }
}

