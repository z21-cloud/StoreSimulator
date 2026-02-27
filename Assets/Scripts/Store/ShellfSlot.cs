using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace StoreSimulator.InteractableObjects
{
    public class ShellfSlot : MonoBehaviour
    {
        public bool IsOccupied { get; private set; }

        private IStoreable _storeable;

        private void Start()
        {
            _storeable = null;
            IsOccupied = false;
        }

        public void Occupy(IStoreable item)
        {
            _storeable = item;
            IsOccupied = true;
        }

        public IStoreable Release()
        {
            if (!IsOccupied) return null;

            IStoreable item = _storeable;
            _storeable = null;
            IsOccupied = false;

            return item;
        }
    }
}

