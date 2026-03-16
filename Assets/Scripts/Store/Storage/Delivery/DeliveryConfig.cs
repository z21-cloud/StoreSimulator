using System;
using Unity.Mathematics;
using UnityEngine;

namespace StoreSimulator.Delivery
{
    [System.Serializable]
    public class DeliveryConfig
    {
        public int columns = 2;
        public int rows = 2;
        public int maxLevels = 4;
        public float boxSize = 1f;
        public bool IsFull => currentCount >= MaxCapacity;
        public int currentCount = 0;
        public int MaxCapacity => columns * rows * maxLevels;
        public int maxItemsInQueue = 15;
    }
}

