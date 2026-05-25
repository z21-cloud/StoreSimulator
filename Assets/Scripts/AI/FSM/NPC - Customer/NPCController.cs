using System.Collections.Generic;
using StoreSimulator.InteractableObjects;
using StoreSimulator.MoneySystem;
using StoreSimulator.StoreableItems;
using StoreSimulator.StoreManager;
using UnityEngine;

namespace StoreSimulator.ArtificialIntelligence
{
    public class NPCController : MonoBehaviour
    {
        [Header("Parameters")]
        [SerializeField] private float waitTime = 5f;
        [SerializeField] private float pickDelay = 0.5f;
        [SerializeField] private int buyPool = 5;

        [Header("NPC components")]
        [SerializeField] private NPCPsycho psycho;
        [SerializeField] private NPCWallet wallet;
        // [SerializeField] private NPCMemoryData memoryData;

        [Header("Movement logic")]
        [SerializeField] private NPCMovement movement;

        [Header("NPC's pick-up point")]
        [SerializeField] private Transform pickUpPoint;
        [Header("NPC's id")]
        [SerializeField] private string npcId;
        [Header("Smoking area")]
        [SerializeField] private SmokingArea smokingArea;

        //
        public IShoppingProvider CurrentProvider { get; private set; }
        public IShoppingSession LastSession { get; set; }
        //
        public IStore CurrentStore { get; set; }
        public IStorage CurrentShelf { get; set; }
        public ICashStorage CurrentCashStorage { get; set; }
        public List<IStoreable> BoughtItems { get; set; }
        public List<IStorage> Shelves { get; set; }
        public Vector3 CurrentInteractionPoint { get; set; }

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
        // public UtilityPlanner UtilityPlanner { get; private set; }

        public NPCStateMachine StateMachine { get; private set; }
        // public IdleState IdleState { get; private set; }
        public PickStoreState PickStoreState { get; private set; }
        public MovingToStore MovingToStore { get; private set; }
        public MovingToStorage MovingToStorage { get; private set; }
        public TakingState TakingState { get; private set; }
        public ShoppingState ShoppingState { get; private set; }
        public MovingToCheckout MovingToCheckout { get; private set; }
        public BuyingState BuyingState { get; private set; }
        public LeavingState LeavingState { get; private set; }
        // public StealingState StealingState { get; private set; }
        public WaitingState WaitingState { get; private set; }
        public SmokingState SmokingState { get; private set; }

        public void Initialize(Vector3 position)
        {
            transform.position = position;
        }

        void Start()
        {
            CurrentProvider = StoreRegistry.Instance.GetRandomStore() as IShoppingProvider;
            //

            BoughtItems = new List<IStoreable>(buyPool);
            Shelves = new List<IStorage>();

            StateMachine = new NPCStateMachine();
            // IdleState = new IdleState(this);
            MovingToStore = new MovingToStore(this);
            TakingState = new TakingState(this);
            BuyingState = new BuyingState(this);
            // StealingState = new StealingState(this);
            LeavingState = new LeavingState(this);
            WaitingState = new WaitingState(this);
            SmokingState = new SmokingState(this);
            ShoppingState = new ShoppingState(this);
            MovingToCheckout = new MovingToCheckout(this);
            PickStoreState = new PickStoreState(this);
            MovingToStorage = new MovingToStorage(this);

            BuyPool = buyPool;
            WaitTime = waitTime;
            PickDelay = pickDelay;

            // UtilityPlanner = new UtilityPlanner(this);

            // uncomment
            // StateMachine.SetState(PickStoreState);
        }

        void Update()
        {
            movement.Tick();
            StateMachine.Tick();
            // UtilityPlanner.Tick();
        }

        /*public void RecordVisit(float totalSpent = 0f, PriceReactionType priceReactionType = PriceReactionType.Fair)
        {
            VisitRecord visit = new VisitRecord();
            //visit.dayIndex = 

            visit.totalSpent = totalSpent;

            if (priceReactionType == PriceReactionType.Fair) visit.reactionType = psycho.GetLastReaction();
            else visit.reactionType = priceReactionType;

            // visit.foundAllItems = boughtItems.Count > 0;

            NPCMemoryManager.Instance.RecordVisit(npcId, CurrentStore.StoreID, visit);

            Debug.Log($"[AI - {gameObject.name}] New visit recorded: Total Spent: {visit.totalSpent} \n, Reaction type: {visit.reactionType} \n, All Items Found: {visit.foundAllItems}");
        }*/

        public bool HaveEnoughMoney(IStoreable storeable)
        {
            float newItemPrice = PricesManager.Instance.GetPlayerPriceForItem(storeable.Data);
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

        /*public void OnActionCompleted()
        {
            UtilityPlanner.OnCurrentActionDone();
        }*/
    }
}
