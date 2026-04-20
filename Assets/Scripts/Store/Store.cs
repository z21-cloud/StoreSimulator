using System;
using StoreSimulator.StoreManager;
using Unity.VisualScripting;
using UnityEngine;

namespace StoreSimulator.StoreUtility
{
    public class Store : MonoBehaviour
    {
        [SerializeField] private DeliveryPriceManager deliveryPriceManager;
        [SerializeField] private bool isOpen;
        [SerializeField] private bool isAuto;
        [SerializeField] private Transform storeEnterPoint;
        [SerializeField] private Transform storeLeavePoint;
        [SerializeField] private Transform deliveryPoint;
        [SerializeField] private string storeId;
        [SerializeField] private StorageRegistry storageRegistry;
        [SerializeField] private CashStorageRegistry cashStorageRegistry;
        
        [Header("Price Manager")]
        [SerializeField] private bool isPlayerStore;
        [SerializeField] private PricesManager pricesManager;
        
        private StoreState _currentState;
        private IPriceProvider _priceProvider;

        public IPriceProvider PriceProvider => _priceProvider;
        public CashStorageRegistry CashStorageRegistry => cashStorageRegistry;
        public StorageRegistry StorageRegistry => storageRegistry;
        public Transform StoreEnterPoint => storeEnterPoint;
        public Transform StoreLeavePoint => storeLeavePoint;
        public Transform DeliveryPoint => deliveryPoint;
        public bool IsOpen => _currentState == StoreState.Open;
        public bool IsAuto => isAuto;
        public string StoreID => storeId;

        private void Awake()
        {
            _priceProvider = isPlayerStore
                ? new PlayerPriceManager(pricesManager)
                : new MarketPriceManager(pricesManager);
    
            StoreRegistry.Instance.RegisterStore(this);
            TimeManager.Instance.OnPhaseChanged += HandlePhaseChange;
        }

        public void TryOpen()
        {
            _currentState = StoreState.Open;
            Debug.Log($"[Store Manager]: Current store state is {_currentState}; IsOpen - {IsOpen}");

            deliveryPriceManager.SetNewDeliveryOrderPrice();
            Debug.Log($"[Store Manager]: Delivery Prices update");
        }
        public void TryClose()
        {
            _currentState = StoreState.Closed;
            Debug.Log($"[Store Manager]: Current store state is {_currentState}; IsOpen - {IsOpen}");
        }

        private void HandlePhaseChange(DayPhase phase)
        {
            if (!isAuto) return;

            if (phase == DayPhase.Morning) TryOpen();
            else if (phase == DayPhase.Night) TryClose();
        }

        private void OnDisable()
        {
            TimeManager.Instance.OnPhaseChanged -= HandlePhaseChange;
        }
    }
}

