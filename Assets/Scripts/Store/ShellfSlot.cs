using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace StoreSimulator.InteractableObjects
{
    public class ShellfSlot : MonoBehaviour
    {
        public Transform SpawnPoint { get; private set; }
        public bool IsOccupied { get; private set; }

        private IStoreable storeable;

        private void Start()
        {
            storeable = null;
            SpawnPoint = transform;
            IsOccupied = false;
        }

        public void Occupy(IStoreable item)
        {
            storeable = item;
            IsOccupied = true;
        }

        public IStoreable Release()
        {
            IsOccupied = false;
            if (storeable != null)
            {
                IStoreable result = storeable;
                storeable = null;

                return result;
            }
            
            return null;
        }
    }
}

