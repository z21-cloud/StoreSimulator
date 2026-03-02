using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using System;

namespace StoreSimulator.InteractableObjects
{
    public class Storage : MonoBehaviour, IInteractable, IStorage
    {
        [SerializeField] private List<ShellfSlot> slots;

        //public bool IsEmpty { get; private set; }

        //public bool IsFull { get; private set; }

        private int _occupiedCount = 0;

        public bool CanPlaceItem()
        {
            foreach (var slot in slots)
            {
                if (!slot.IsOccupied) return true;
            }
            return false;
        }

        public bool CanTakeItem()
        {
            foreach (var slot in slots)
            {
                if (slot.IsOccupied) return true;
            }
            return false;
        }

        public void PlaceItem(GameObject item) //Vector3 playerPosition, bool findOccupied, 
        {
            // FIND FREE SLOT
            //ShellfSlot reservedSlot = GetClosestToPlayer(playerPosition, findOccupied);

            ShellfSlot reservedSlot = null;
            foreach (ShellfSlot slot in slots)
            {
                if (!slot.IsOccupied)
                {
                    reservedSlot = slot;
                }
            }

            // PLACE ITEM
            if (reservedSlot != null)
            {
                reservedSlot.Occupy(item);
                //_occupiedCount++;
                //_occupiedCount = Mathf.Clamp(_occupiedCount, 0, slots.Count);
                //UpdateStates();
            }
        }

        public GameObject TakeItem(Vector3 interactionPoint)
        {
            ShellfSlot bestSlot = null;
            float minDistance = float.MaxValue;

            foreach (var slot in slots)
            {
                if (slot.IsOccupied)
                {
                    float dist = (interactionPoint - slot.transform.position).sqrMagnitude;

                    if(minDistance > dist)
                    {
                        minDistance = dist;
                        bestSlot = slot;
                    }
                }
            }

            return bestSlot != null ? bestSlot.Release() : null;
        }

        private void UpdateStates()
        {
            //IsEmpty = _occupiedCount == 0;
            //IsFull = _occupiedCount >= slots.Count;
        }

        public string GetDescription()
        {
            throw new NotImplementedException();
        }

        /*private ShellfSlot GetClosestToPlayer(Vector3 playerPosition, bool findOccupied)
        {
            foreach (var slot in slots)
            {
                if(slot.IsOccupied == findOccupied)
                {
                    return slot;
                }
            }

            return null;
        }*/
    }
}

