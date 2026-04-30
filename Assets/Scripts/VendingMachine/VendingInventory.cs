using System.Collections.Generic;
using StoreSimulator.StoreableItems;
using UnityEngine;

public class VendingInventory : MonoBehaviour
{
    [SerializeField] private List<ItemData> assortment;
    [SerializeField] private int capacity;
    [SerializeField] private int soldCount;

    public ItemData GetItem(ItemCategory category)
    {
        int attempts = 100;
        var tempAssortment = CreateNewList();
        for (int i = 0; i < attempts; i++)
        {
            if (tempAssortment.Count == 0) tempAssortment = CreateNewList();

            int id = Random.Range(0, tempAssortment.Count);
            var candidat = tempAssortment[id];
            if (candidat.Category == category)
            {
                return candidat;
            }

            tempAssortment.Remove(candidat);
        }

        if (tempAssortment.Count == 0) tempAssortment = CreateNewList();
        return tempAssortment[0];
    }

    private List<ItemData> CreateNewList()
    {
        return new List<ItemData>(assortment);
    }
}
