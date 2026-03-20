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
        // �������<���������, ����> -> ���������� ���� �� ���� (��������, +10% ���� � ������� ����)
        // �������<������������, ����> -> ���������� ���� �� ��� ������� (������ ���� �� ���� ������. ��������, ���� �� ��� ����) 

        // ������� ������� ��������� � ������� �����, ������� ������� ������������ �� ����� �������� ������

        // ��������� ����� ��� ��������� ���� ��������� (��������, ������� ��������� � ����� ������ �� �����)
        // ��������� ����� ��� ��������� ���� ������������ (��������, ������� ��������� � ����� ������ �� �����)
        // ��������� ����� ��� ��������� ���� ��������� 
        // ��������� ����� ��� ��������� ���� ������������ 
        // ������� �������� ���������, ����� ������������. ��������� -> base price ; ������������ -> Current Price

        /* �������������� ���������
         * ��������, � OnEnable (��-�� �������� object pooling) ���������� ��������� ������� �������� � PriceManager
         * ���� ����� �������, ��� ���� ����������, �� ��� ���� ������ ������� �� ��������� � ������������
         * ����� ������� �����, �� � ���� ����������� � ���� �����-�� ����� � ����������� ��� ���
         * 
         * 
         * ����
         * � ����� ��������� � PricesManager � ������, ����� �������� �������, ������� ���� � ������� ��� �����
         * ����� ����������, ��� �������� ������� �� ��� ����, �� ��� �������� ��������� ���������
         * ���� ����� ������� ���� ����� ������, �� ������ �������� ChangePrices, ��� �� ����: "���, ��� �������� ����� ����"
         * ����� ��������� ���������� � PricesManager � ����� ����� �������� ��� ������� ��������
         * 
         * ��� � ����� �������:
         * 1. �������� PricesManager �� ��������� ���������, ������������ � �� ������
         * 2. ����� ������ ��� ��������� / ��������� ���
         * 3. ��������� �� ����, ��� ������ �������� ��� ���
         * 4. ������� ������������ ������� ��� ��� PricesManager
         * 5. �������� UI, ����� ����� ��� ������ ������
         */
        public static PricesManager Instance { get; private set; }

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

