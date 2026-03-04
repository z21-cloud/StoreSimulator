using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace StoreSimulator.StoreableItems
{
    public enum ItemSubCategory 
    {
        None = 0,
        Apple = 1 << 0,             // 1
        Aubergine = 1 << 1,         // 2
        Lemon = 1 << 2,             // 4
        Lettuce = 1 << 3,           // 8
        Pineapple = 1 << 4,         // 16
        Watermelon_half = 1 << 5,   // 32
        Watermelon_full = 1 << 6,   // 64
        Bottle_Cola = 1 << 7,       // 128
        Bottle_Sprite = 1 << 8,     // 256
        Bottle_Fanta = 1 << 9,      // 512
        Bottle_Water = 1 << 10,     // 1024
        Box_Granola= 1 << 11,       // 2048
        Box_Rings = 1 << 12,        // 4096
        Chips_bbq = 1 << 13,        // 8192
        Chips_chedar = 1 << 14,     // 16 384
        Chips_classic = 1 << 15,    // 32 768
        Chips_onion = 1 << 16,      // 65 536

        // 32 sub categories can be added
    }
}
