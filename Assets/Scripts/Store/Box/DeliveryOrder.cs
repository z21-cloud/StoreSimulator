using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using StoreSimulator.StoreableItems;

namespace StoreSimulator.Boxes
{
    [CreateAssetMenu(fileName = "DeliveryOrder", menuName = "Store/DeliveryOrder")]
    public class DeliveryOrder : ScriptableObject
    {
        [SerializeField] private ItemData itemData;
        [SerializeField] private Sprite icon;
        [SerializeField] private float boxCost;
        [SerializeField] private int quantity;
        [SerializeField] private GameObject boxPrefab;

        public ItemData ItemData => itemData;
        public GameObject BoxPrefab => boxPrefab;
        public Sprite Icon => icon;
        public float BoxCost => boxCost;
        public int Quantity => quantity;
    }
}

