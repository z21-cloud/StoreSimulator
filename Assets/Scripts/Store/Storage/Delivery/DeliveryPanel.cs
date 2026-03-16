using System.Collections.Generic;
using StoreSimulator.Boxes;
using StoreSimulator.MoneySystem;
using StoreSimulator.StoreableItems;
using TMPro;
using UnityEngine;

namespace StoreSimulator.Delivery
{
    public class DeliveryPanel : MonoBehaviour
{
    [Header("Orders")]
    [SerializeField] private List<DeliveryOrder> availableOrders;
    [SerializeField] private OrderItem orderItemPrefab;
    [SerializeField] private Transform orderList;
    [SerializeField] private GameObject boxPrefab;
    [SerializeField] private DeliveryZone deliveryZone;

    [Header("UI")]
    [SerializeField] private TMP_Text balanceText;
    [SerializeField] private TMP_Text totalPriceText;

    private IWallet _wallet;
    private List<OrderItem> _orderItems = new List<OrderItem>();

    public void Init(IWallet wallet)
    {
        _wallet = wallet;

        if (_orderItems.Count > 0)
        {
            foreach (var item in _orderItems)
            {
                item.Reset();
            }

            UpdateTotal();
            return;
        }

        foreach (var order in availableOrders)
        {
            OrderItem item = Instantiate(orderItemPrefab, orderList);
            item.Init(order);
            item.OnChanged += UpdateTotal;
            _orderItems.Add(item);
        }
        
        UpdateTotal();
    }

    public void OnConfirm()
    {
        if(!deliveryZone.HasFreeSlot()) return;
        
        float price = CalculateTotal();

        if (!_wallet.CanAfford(price))
        {
            Debug.Log($"[DeliveryPanel]: not enough money");
            return;
        }
        
        foreach (var item in _orderItems)
        {
            if(deliveryZone.CurrentSlotsCount() < item.Quantity)
            {
                Debug.Log($"[DeliveryPanel]: Current free slots {deliveryZone.CurrentSlotsCount()} | order count {item.Quantity}");
                return;
            }

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

