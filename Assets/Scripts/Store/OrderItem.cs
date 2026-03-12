using System;
using StoreSimulator.Boxes;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class OrderItem : MonoBehaviour
{
    [SerializeField] private Image itemImage;
    [SerializeField] private TMP_Text quantityText;
    [SerializeField] private TMP_Text itemNameText;
    [SerializeField] private TMP_Text priceText;

    private DeliveryOrder _order;
    private int _quantity;

    public int Quantity => _quantity;
    public DeliveryOrder Order => _order;

    public event Action OnChanged;

    public void Init(DeliveryOrder order)
    {
        _order = order;
        _quantity = 0;
        itemImage.sprite = order.ItemData.Icon;
        itemNameText.text = order.ItemData.ItemName;
        priceText.text = $"{order.BoxCost:F2}$";
        UpdateUI();
        Reset();
    }

    public void OnIncrease() 
    { 
        _quantity++;
        UpdateUI();    
    }

    public void OnDecrease()
    {
        if(_quantity > 0) _quantity--;
        UpdateUI();
    }

    private void UpdateUI()
    {
        UpdateTotal();
        
        quantityText.text = $"{_quantity}";
        OnChanged?.Invoke();
    }

    public void Reset()
    {
        _quantity = 0;
        priceText.text = $"{_order.BoxCost:F2}$";
        quantityText.text = $"{_quantity}";
    }

    private void UpdateTotal()
    {
        if(_quantity == 0) return;
        
        float total = _order.BoxCost * _quantity;
        priceText.text = $"{total:F2}$";
    }
}
