using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using StoreSimulator.StoreableItems;

[CreateAssetMenu(fileName = "DeliveryOrder", menuName = "Store/DeliveryOrder")]
public class DeliveryOrder : ScriptableObject
{
    [SerializeField] private ItemData itemData;
    [SerializeField] private int quantity;

    public ItemData ItemData => itemData;
    public int Quantity => quantity;
}
