using System;
using StoreSimulator.StoreManager;
using Unity.VisualScripting;
using UnityEngine;

namespace StoreSimulator.StoreUtility
{
    public class Store : MonoBehaviour, IStore
    {
        [Header("Store Config and settings")]
        [SerializeField] private StoreConfig config;
        [SerializeField] private bool isOpen;
        [SerializeField] private string storeId;
        
        [Header("Transform points")]
        [SerializeField] private Transform storeEnterPoint;
        [SerializeField] private Transform storeLeavePoint;
        [SerializeField] private Transform deliveryPoint;
       
        [Header("Services")]
        [SerializeField] private StorageRegistry storageRegistry;
        [SerializeField] private CashStorageRegistry cashStorageRegistry;
        
        [Header("Price Manager")]
        [SerializeField] private bool isPlayerStore;
        [SerializeField] private PricesManager pricesManager;
        [SerializeField] private DeliveryPriceManager deliveryPriceManager;
        
        private StoreState _currentState;
        private IPriceProvider _priceProvider;

        // services
        public IPriceProvider PriceProvider => _priceProvider;
        public CashStorageRegistry CashStorageRegistry => cashStorageRegistry;
        public StorageRegistry StorageRegistry => storageRegistry;
        
        // interaction points
        public Transform StoreEnterPoint => storeEnterPoint;
        public Transform StoreLeavePoint => storeLeavePoint;
        public Transform DeliveryPoint => deliveryPoint;

        // config
        public string StoreID => config.StoreID;
        public bool IsAuto => config.IsAuto;

        public bool IsOpen => _currentState == StoreState.Open;

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
            if (!IsAuto) return;

            if (phase == DayPhase.Morning) TryOpen();
            else if (phase == DayPhase.Night) TryClose();
        }

        private void OnDisable()
        {
            TimeManager.Instance.OnPhaseChanged -= HandlePhaseChange;
        }
    }
}

