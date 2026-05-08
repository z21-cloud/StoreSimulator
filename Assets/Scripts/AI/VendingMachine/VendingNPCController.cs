using System.Collections.Generic;
using StoreSimulator.InteractableObjects;
using StoreSimulator.MoneySystem;
using StoreSimulator.StoreableItems;
using StoreSimulator.StoreManager;
using UnityEngine;

namespace StoreSimulator.ArtificialIntelligence
{
    public class VendingNPCController : MonoBehaviour
    {
        [Header("Parameters")]
        [SerializeField] private float waitTime = 5f;
        [SerializeField] private float pickDelay = 0.5f;
        [SerializeField] private int buyPool = 5;

        [Header("NPC components")]
        [SerializeField] private NPCPsycho psycho;
        [SerializeField] private NPCWallet wallet;
        [SerializeField] private NPCMemoryData memoryData;

        [Header("Movement logic")]
        [SerializeField] private NPCMovement movement;

        [Header("NPC's pick-up point")]
        [SerializeField] private Transform pickUpPoint;
        [Header("NPC's id")]
        [SerializeField] private string npcId;
        [Header("Smoking area")]
        [SerializeField] private SmokingArea smokingArea;
        [SerializeField] private VendingMachine vendingMachine;

        public VendingMachine CurrentStore => vendingMachine;
        public IStorage CurrentShelf { get; set; }
        public List<IStoreable> BoughtItems { get; set; }
        public List<IStorage> Shelves { get; set; }

        public int ItemsToBuy { get; set; }

        public string NpcId => npcId;

        public Transform PickUpPoint => pickUpPoint;
        public SmokingArea SmokingArea => smokingArea;

        public NPCMovement Movement => movement;
        public NPCPsycho Psycho => psycho;
        public IWallet Wallet => wallet;

        public int BuyPool { get; private set; }
        public float WaitTime { get; private set; }
        public float PickDelay { get; private set; }

        public List<ItemCategory> NPCNeeds => Psycho.GetPriorityNeeds();

        // Added

        public NPCStateMachine StateMachine { get; private set; }

        public GoToMachine GoToMachine { get; private set; }
        public BuyVendingMachine BuyVendingMachine { get; private set; }
        public LeavingVendingMachine LeavingVendingMachine { get; private set; }

        void Start()
        {
            BoughtItems = new List<IStoreable>(buyPool);
            Shelves = new List<IStorage>();

            StateMachine = new NPCStateMachine();
            GoToMachine = new GoToMachine(this);
            BuyVendingMachine = new BuyVendingMachine(this);
            LeavingVendingMachine = new LeavingVendingMachine(this);

            BuyPool = buyPool;
            WaitTime = waitTime;
            PickDelay = pickDelay;

            // uncomment
            StateMachine.SetState(GoToMachine);
        }

        void Update()
        {
            movement.Tick();
            StateMachine.Tick();
        }

        public void RecordVisit(float totalSpent = 0f, PriceReactionType priceReactionType = PriceReactionType.Fair)
        {
            VisitRecord visit = new VisitRecord();
            //visit.dayIndex = 

            visit.totalSpent = totalSpent;

            if (priceReactionType == PriceReactionType.Fair) visit.reactionType = psycho.GetLastReaction();
            else visit.reactionType = priceReactionType;

            // visit.foundAllItems = boughtItems.Count > 0;

            NPCMemoryManager.Instance.RecordVisit(npcId, "VendingMachine", visit);

            Debug.Log($"[AI - {gameObject.name}] New visit recorded: Total Spent: {visit.totalSpent} \n, Reaction type: {visit.reactionType} \n, All Items Found: {visit.foundAllItems}");
        }

        public bool HaveEnoughMoney(ItemData itemData)
        {
            float newItemPrice = PricesManager.Instance.GetPlayerPriceForItem(itemData);
            float alreadyReserved = GetTotalCost(BoughtItems);

            return wallet.CanAfford(newItemPrice + alreadyReserved);
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
            BoughtItems.Remove(storeable);
            if (((MonoBehaviour)storeable).TryGetComponent<IHoldable>(out var holdable))
            {
                holdable.Release(Vector3.zero);
            }
        }
    }
}