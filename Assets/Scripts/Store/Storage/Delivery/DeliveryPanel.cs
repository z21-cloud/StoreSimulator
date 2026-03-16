using System.Collections.Generic;
using StoreSimulator.Boxes;
using StoreSimulator.MoneySystem;
using StoreSimulator.StoreableItems;
using TMPro;
using UnityEditor.EditorTools;
using UnityEngine;

namespace StoreSimulator.Delivery
{
    public class DeliveryPanel : MonoBehaviour
    {
        [Header("Orders")]
        [Tooltip("List of all orders player can buy")]
        [SerializeField] private List<DeliveryOrder> availableOrders;
        [Tooltip("Prefab of order in panel")]
        [SerializeField] private OrderItem orderItemPrefab;
        [Tooltip("Spawn point for orderItem prefab")]
        [SerializeField] private Transform orderList;
        [Tooltip("Delivery Zone script that spawns orders at pallete")]
        [SerializeField] private DeliveryZone deliveryZone;

        [Header("UI")]
        [SerializeField] private TMP_Text balanceText;
        [SerializeField] private TMP_Text totalPriceText;

        // current wallet (for now only for player)
        private IWallet _wallet;
        // cache orderItems. 1 delivery order = 1 orderItem prefab in panel
        private List<OrderItem> _orderItems = new List<OrderItem>();

        // initialization, every time player opens delivery panel
        public void Init(IWallet wallet)
        {
            _wallet = wallet;

            // if player already have orders
            if (_orderItems.Count > 0)
            {
                foreach (var item in _orderItems)
                {
                    item.Reset();
                }

                UpdateTotal();
                return;
            }

            // first time player opens panel
            foreach (var order in availableOrders)
            {
                OrderItem item = Instantiate(orderItemPrefab, orderList);
                item.Init(order);
                item.OnChanged += UpdateTotal;
                _orderItems.Add(item);
            }

            UpdateTotal();
        }

        // complete deal and buy boxes
        public void OnConfirm()
        {
            // future: box add in queue. Need to add limit in config
            if (!deliveryZone.HasFreeSlot()) return;

            float price = CalculateTotal();

            if (!_wallet.CanAfford(price))
            {
                Debug.Log($"[DeliveryPanel]: not enough money");
                return;
            }

            // goes throw ALL orderItems and check their QUANTITY 
            foreach (var item in _orderItems)
            {
                // if player didn't increase item's quantity => skip
                if(item.Quantity <= 0) continue;

                // if delivery zone have enough slots for current order
                if (deliveryZone.CurrentSlotsCount() < item.Quantity)
                {
                    Debug.Log($"[DeliveryPanel]: Current free slots {deliveryZone.CurrentSlotsCount()} | order count {item.Quantity}");
                    return;
                }

                // Instatiate order
                for (int i = 0; i < item.Quantity; i++)
                {
                    GameObject box = Instantiate(item.Order.BoxPrefab, transform.position, Quaternion.identity);
                    box.GetComponent<BoxStorage>().Initialize(item.Order);
                    deliveryZone.PlaceBox(box);
                }
            }

            Debug.Log($"[DeliveryPanel]: spended {price}$");
            _wallet.Spend(price);

            UpdateTotal();
        }

        private void UpdateTotal()
        {
            balanceText.text = $"Balance: {_wallet.Balance:F2}$";
            totalPriceText.text = $"Summary: {CalculateTotal():F2}$";
        }

        private float CalculateTotal()
        {
            float total = 0;
            foreach (var item in _orderItems)
                total += item.Order.BoxCost * item.Quantity;

            return total;
        }
    }
}

