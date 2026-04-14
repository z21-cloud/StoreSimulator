using System;
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
        [SerializeField] private Transform orderItems;
        [SerializeField] private StorageRegistry storageRegistry;
        
        private StoreState _currentState;
        
        public StorageRegistry StorageRegistry => storageRegistry;
        public Transform StoreEnterPoint => storeEnterPoint;
        public Transform StoreLeavePoint => storeLeavePoint;
        public Transform OrderItems => orderItems;
        public Transform DeliveryZone => orderItems;
        public bool IsOpen => _currentState == StoreState.Open;
        public bool IsAuto => isAuto;

        private void Start()
        {
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

