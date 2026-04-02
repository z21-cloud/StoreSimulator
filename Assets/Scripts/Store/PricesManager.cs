using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using StoreSimulator.StoreableItems;
using StoreSimulator.InteractableObjects;
using System;
using StoreSimulator.Boxes;
using Unity.VisualScripting;

namespace StoreSimulator.StoreManager
{
    public class PricesManager : MonoBehaviour
    {
        [SerializeField] private List<ItemData> items;

        public static PricesManager Instance { get; private set; }

        private Dictionary<ItemSubCategory, float> _playerPrice = new Dictionary<ItemSubCategory, float>();
        // private Dictionary<ItemCategory, float> _categoryMultipliers = new Dictionary<ItemCategory, float>();

        private Dictionary<ItemSubCategory, float> _marketPrice = new Dictionary<ItemSubCategory, float>();

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
        }

        private void Start()
        {
            foreach(var item in items)
            {
                _marketPrice[item.SubCategory] = item.BasePrice;
            }
        }

        public float GetPlayerPriceForItem(ItemData data)
        {
            if (_playerPrice.TryGetValue(data.SubCategory, out float customPrice))
            {
                return customPrice;
            }

            // if (_categoryMultipliers.TryGetValue(data.Category, out float multiplier))
            // {
            //     return data.BasePrice * multiplier;
            // }

            return data.BasePrice;
        }

        public float GetMarketPriceForItem(ItemData data)
        {
            if (_marketPrice.TryGetValue(data.SubCategory, out float marketPrice))
            {
                return marketPrice;
            }

            Debug.LogError($"[PriceManager] There is now Market Price for this item - {data.ItemName}!");
            return 0f;
        }

        public void SetSubCategoryPrice(ItemSubCategory subCategory, float price)
        {
            _playerPrice[subCategory] = Mathf.Max(0, price);
            Debug.Log($"[PricesManager] New price for {subCategory} - {price}");
        }

        // public void SetCategoryMultiplier(ItemCategory category, float multiplier)
        // {
        //     _categoryMultipliers[category] = Mathf.Max(0, multiplier);
        //     Debug.Log($"[PricesManager] New multiplier for {category} - {multiplier * 100}%");
        // }
    }
}

