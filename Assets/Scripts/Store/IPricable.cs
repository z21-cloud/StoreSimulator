using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace StoreSimulator.StoreableItems
{
    public interface IPricable 
    {
        public float CurrentPrice { get; set; }
    }
}

