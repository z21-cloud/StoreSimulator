using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace StoreSimulator.StoreableItems
{
    public interface IPriceStorage
    {
        public void OnPriceInputChanged(float price);
        public float GetCurrentPrice();
        public float GetBasePrice();
    }
}

