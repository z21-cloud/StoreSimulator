using System;
using TMPro;
using UnityEngine;

namespace StoreSimulator.BuildingSystem
{
    public class OrderShelf : MonoBehaviour
    {
        // ui prefab
        [SerializeField] private TMP_Text itemNameText;
        [SerializeField] private TMP_Text priceText;

        private StoreOrder _storeOrder;
        public StoreOrder StoreOrder => _storeOrder;
        public void Init(StoreOrder order)
        {
            itemNameText.text = order.ItemName;
            priceText.text = $"{order.Cost:F2}$";
            _storeOrder = order;
        }
    }
}
