using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using StoreSimulator.StoreableItems;

[CreateAssetMenu(fileName = "Slot Preset", menuName = "Box Slots/SlotsPreset")]
public class BoxSlotPreset : ScriptableObject
{
    [SerializeField] private ItemCategory category;
    [SerializeField] private ItemSubCategory subCategory;

    public ItemCategory Category => category;
    public ItemSubCategory SubCategory => subCategory;
}
