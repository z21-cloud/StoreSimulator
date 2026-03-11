using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using StoreSimulator.InteractableObjects;

public class SlotGroup : MonoBehaviour
{
    [SerializeField] private BoxSlotPreset preset;
    [SerializeField] private List<ShellfSlot> slots;

    public BoxSlotPreset Preset => preset;
    public List<ShellfSlot> Slots => slots;
}
