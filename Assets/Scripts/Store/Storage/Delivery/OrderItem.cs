using System;
using StoreSimulator.Boxes;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace StoreSimulator.Delivery
{
    public class OrderItem : MonoBehaviour
    {
        [Header("Text")]
        [SerializeField] private TMP_Text quantityText;
        [SerializeField] private TMP_Text priceText;
        [SerializeField] private TMP_Text itemNameText;
        [SerializeField] private Image itemImage;

        private DeliveryOrder _order;
        private int _quantity = 0;
        private float _price = 0f;
        
        public event Action OnChanged;
        
        public int Quantity => _quantity;
        public DeliveryOrder Order => _order;

        public void Init(DeliveryOrder order, float currentPrice)
        {
            _order = order;
            _quantity = 0;
            itemImage.sprite = order.ItemData.Icon;
            itemNameText.text = order.ItemData.ItemName;
            _price = currentPrice;
            priceText.text = $"{_price:F2}$";
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
            if (_quantity > 0) _quantity--;
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
            priceText.text = $"{_price:F2}$";
            quantityText.text = $"{_quantity}";
        }

        private void UpdateTotal()
        {
            if (_quantity == 0) return;

            float total = _price * _quantity;
            priceText.text = $"{total:F2}$";
        }
    }
}

