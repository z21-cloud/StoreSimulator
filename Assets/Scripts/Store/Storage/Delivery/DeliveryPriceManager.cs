using System.Collections.Generic;
using System.Linq;
using StoreSimulator.Boxes;
using UnityEngine;

public class DeliveryPriceManager : MonoBehaviour
{
    [SerializeField] private List<DeliveryOrder> orders = new List<DeliveryOrder>();
    //public static DeliveryPriceManager Instance { get; private set; }

    private Dictionary<DeliveryOrder, float> deliveryOrders = new Dictionary<DeliveryOrder, float>();

    public List<DeliveryOrder> Orders => orders;

    private void Awake()
    {
        foreach (var order in orders)
        {
            deliveryOrders[order] = order.BoxCost;
        }
    }

    public void SetNewDeliveryOrderPrice()
    {
        var keys = deliveryOrders.Keys.ToArray();

        foreach (var key in keys)
        {
            float cost = deliveryOrders[key];
            
            float partOfOrderCost = cost * 0.05f;
            float newCost = cost + Random.Range(-partOfOrderCost, partOfOrderCost);

            deliveryOrders[key] = newCost;
        }
    }

    public float GetDeliveryOrderPrice(DeliveryOrder order)
    {
        if (!deliveryOrders.ContainsKey(order))
        {
            Debug.LogWarning("[Delivery]: There is no such order!");
            return 0f;
        }
        return deliveryOrders[order];
    }
}
