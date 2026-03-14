using System.Collections.Generic;
using StoreSimulator.Boxes;
using StoreSimulator.InteractableObjects;
using StoreSimulator.StoreableItems;
using UnityEngine;

public class DeliveryZone : MonoBehaviour, IBoxStorage, IBoxOwner
{
    [SerializeField] private List<Transform> slots;
    [SerializeField] private DeliveryConfig config;

    private List<GameObject> _placedBoxes = new List<GameObject>();

    public bool HasFreeSlot() => !config.IsFull;

    public int CurrentSlotsCount() 
    {
        int result = Mathf.Abs(config.currentCount - config.columns * config.rows * config.maxLevels);
        //Debug.Log($"[DeliveryZone] Current count in pallete slots is: {config.currentCount}");
        //Debug.Log($"[DeliveryZone] Current free slots is: {result}");
        return result;
    } 

    public bool CanTakeItem() => _placedBoxes.Count > 0;
    public void PlaceBox(GameObject box)
    {
        if(config.IsFull) return;

        int column = config.currentCount % config.columns;
        int row = (config.currentCount / config.columns) % config.rows;
        int level = config.currentCount / (config.rows * config.columns);

        int slotIndex = column + row * config.columns;
        Vector3 spawnPos = new Vector3
        (
            slots[slotIndex].position.x,
            slots[slotIndex].position.y + level * config.boxSize,
            slots[slotIndex].position.z
        );

        box.transform.position = spawnPos;
        _placedBoxes.Add(box);
        config.currentCount++;

        box.GetComponent<BoxStorage>().SetOwner(this);
    }

    public GameObject TakeBox()
    {
        throw new System.NotImplementedException();
    }

    public void OnBoxRemoved(GameObject box)
    {
        _placedBoxes.Remove(box);
        config.currentCount--;
    }
}
