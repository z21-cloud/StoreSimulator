using System.Collections.Generic;
using StoreSimulator.Boxes;
using StoreSimulator.InteractableObjects;
using StoreSimulator.StoreableItems;
using UnityEngine;

public class DeliveryZone : MonoBehaviour
{
    [SerializeField] private List<Transform> slots;
    [SerializeField] private DeliveryConfig config;

    public void SpawnDeliveryBox(DeliveryOrder order)
    {
        if (config.IsFull) return;

        int column = config.currentCount % config.columns;
        int row = (config.currentCount / config.columns) % config.rows;
        int level = config.currentCount / (config.rows * config.columns);

        Debug.Log($"{row} : {column}");

        int index = column + row;
        Vector3 spawnPosition = new Vector3
        (
            slots[index].position.x,
            slots[index].position.y + slots[index].position.y * level,
            slots[index].position.z
        );

        GameObject box = Instantiate(order.BoxPrefab, spawnPosition, Quaternion.identity);
        box.GetComponent<BoxStorage>().Initialize(order);
        config.currentCount++;
    }

    public void Reset()
    {
        config.currentCount = 0;
    }
}
