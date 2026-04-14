using System.Collections.Generic;
using StoreSimulator.InteractableObjects;
using StoreSimulator.MoneySystem;
using StoreSimulator.StoreableItems;
using StoreSimulator.StoreManager;
using StoreSimulator.StoreUtility;
using UnityEngine;

namespace StoreSimulator.ArtificialIntelligence
{
    public class NPCOwner : MonoBehaviour
    {

        [Header("Parameters")]
        [SerializeField] private float waitTime = 5f;
        [SerializeField] private float pickDelay = 0.5f;

        [Header("NPC components")]
        [SerializeField] private NPCWallet wallet;

        [Header("Movement logic")]
        [SerializeField] private NPCMovement movement;

        [Header("Position of each path for NPC")]
        [SerializeField] private Transform pickUpPoint;
        [SerializeField] private string npcId;

        [Header("Store owner Logic")]
        [SerializeField] private Store store;
        [SerializeField] private ICashStorage cashStorage;
        [SerializeField] private List<StoreableItem> items;
        [SerializeField] private BoxPooling boxPooling;

        private List<StoreableItem> _temp;

        public IStorage CurrentShelf { get; set; }
        public List<IStorage> Shelves { get; set; }
        public IStoreable Storeable { get; set; }
        public BoxStorage BoxStorage { get; set; }

        public int ItemsToBuy { get; set; }

        public ICashStorage CurrentCashStorage => cashStorage;
        public string NpcId => npcId;

        public Store Store => store;
        public BoxPooling BoxPooling => boxPooling;
        public Transform PickUpPoint => pickUpPoint;
        public NPCMovement Movement => movement;
        public IWallet Wallet => wallet;

        public float WaitTime { get; private set; }
        public float PickDelay { get; private set; }
        public List<StoreableItem> StoreableItems => items;

        public NPCStateMachine StateMachine { get; private set; }
        public StoragesState StorageState { get; private set; }
        public OrderItemsState OrderItemsState { get; private set; }
        public TakeBoxState TakeBoxState { get; private set; }
        public PlaceItemState PlaceItemState { get; private set; }

        void Start()
        {
            Shelves = store.StorageRegistry.GetAllStorages();

            WaitTime = waitTime;
            PickDelay = pickDelay;

            StateMachine = new NPCStateMachine();

            StorageState = new StoragesState(this);
            OrderItemsState = new OrderItemsState(this);
            TakeBoxState = new TakeBoxState(this);
            PlaceItemState = new PlaceItemState(this);

            StateMachine.SetState(StorageState);
        }

        void Update()
        {
            movement.Tick();
            StateMachine.Tick();
        }

        private void CreateTempStoreable()
        {
            _temp = new List<StoreableItem>(StoreableItems);
        }

        public bool HaveEnoughMoney(IStoreable storeable)
        {
            // float newItemPrice = PricesManager.Instance.GetPlayerPriceForItem(storeable.Data);
            // float alreadyReserved = GetTotalCost(BoughtItems);

            // return wallet.CanAfford(newItemPrice + alreadyReserved);

            return false;
        }

        public float GetTotalCost(List<IStoreable> items)
        {
            float total = 0f;
            foreach (var item in items)
                total += PricesManager.Instance.GetPlayerPriceForItem(item.Data);
            return total;
        }

        public void HandleDropItem(IStoreable storeable)
        {
            if (((MonoBehaviour)storeable).TryGetComponent<IHoldable>(out var holdable))
            {
                holdable.Release(Vector3.zero);
            }
        }

        public StoreableItem GetRandomStoreable()
        {
            if (_temp == null || _temp.Count == 0) CreateTempStoreable();
            
            int randomIndex = Random.Range(0, _temp.Count);
            var storeable = _temp[randomIndex];
            Debug.LogWarning($"{storeable.Data.name}");            
            _temp.Remove(storeable);
            return storeable;
        }
    }
}
