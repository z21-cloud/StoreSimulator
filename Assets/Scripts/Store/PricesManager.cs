using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using StoreSimulator.StoreableItems;
using StoreSimulator.InteractableObjects;
using System;

namespace StoreSimulator.StoreManager
{
    public class PricesManager : MonoBehaviour
    {
        // Словарь<Категория, Цена> -> регулирует цены на ТИПЫ (например, +10% цены к каждому типу)
        // Словарь<ПодКатегория, Цена> -> регулирует цены на все подтипы (меняет цену на весь подтип. Например, цены на ВСЮ воду) 

        // Создаем словарь категорий с базовой ценой, создаем словарь подкатегорий со всеми текущими ценами

        // Публичный метод для изменения цены категорий (например, будущий компьютер и через ценник на полке)
        // Публичный метод для изменения цены подкатегорий (например, будущий компьютер и через ценник на полке)
        // Публичный метод для получения цены категорий 
        // Публичный метод для получения цены подкатегорий 
        // Сначала задается категория, после подкатегория. Категория -> base price ; Подкатегория -> Current Price

        /* Взаимодействие предметов
         * Наверное, в OnEnable (из-за будущего object pooling) происходит обращение каждого предмета к PriceManager
         * Либо через события, что цена изменилась, но это надо каждое событие на категорию и подкатегорию
         * Через события проще, но я хочу подолбиться с этим какое-то время и попробовать без них
         * 
         * 
         * ЛИБО
         * Я делаю обращение к PricesManager в момент, когда ставится предмет, получаю цену и передаю это полке
         * Тогда получается, что предмету плевать на его цену, за все отвечает отдельный компонент
         * Если игрок изменил цену через ценник, то ценник вызывает ChangePrices, что по сути: "Хей, нам передали новую цену"
         * Тогда хранилище обращается к PricesManager и берет новое значение для каждого предмета
         * 
         * Что я делаю сначала:
         * 1. Набросок PricesManager со словарями категорий, подкатегорий и их ценами
         * 2. Делаю методы для получения / изменения цен
         * 3. Охреневаю от того, как сильно разросся мой код
         * 4. Начинаю переписывать систему цен под PricesManager
         * 5. Добавляю UI, чтобы игрок уже ВВОДИЛ ценник
         */
        public static PricesManager Instance;

        private Dictionary<ItemSubCategory, float> _subCategoryOverride = new Dictionary<ItemSubCategory, float>();
        private Dictionary<ItemCategory, float> _categoryMultipliers = new Dictionary<ItemCategory, float>();

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
        }

        public float GetPriceForItem(ItemData data)
        {
            if (_subCategoryOverride.TryGetValue(data.SubCategory, out float customPrice))
            {
                return customPrice;
            }

            if (_categoryMultipliers.TryGetValue(data.Category, out float multiplier))
            {
                return data.BasePrice * multiplier;
            }

            return data.BasePrice;
        }

        public float GetPriceForSubCategory(ItemSubCategory subCategory)
        {
            if (_subCategoryOverride.TryGetValue(subCategory, out float customPrice))
            {
                Debug.Log($"Update prpice");

                return customPrice;
            }

            return 0f;
        }

        public float GetPriceForCategory(ItemCategory category)
        {
            if (_categoryMultipliers.TryGetValue(category, out float multiplier))
            {
                return multiplier;
            }

            return 0f;
        }

        public void SetSubCategoryPrice(ItemSubCategory subCategory, float price)
        {
            _subCategoryOverride[subCategory] = Mathf.Max(0, price);
            Debug.Log($"[PricesManager] New price for {subCategory} - {price}");
        }

        public void SetCategoryMultiplier(ItemCategory category, float multiplier)
        {
            _categoryMultipliers[category] = Mathf.Max(0, multiplier);
            Debug.Log($"[PricesManager] New multiplier for {category} - {multiplier * 100}%");
        }
    }
}

