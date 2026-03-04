using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace StoreSimulator.StoreableItems
{
    [System.Flags]
    public enum ItemCategory
    {
        None = 0,
        Food = 1 << 0,      // 1
        Drink = 1 << 1,     // 2
        Household = 1 << 2, // 4
        Alcohol = 1 << 3    // 8
    }
}

