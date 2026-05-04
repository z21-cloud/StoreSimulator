using System.Collections.Generic;
using StoreSimulator.ArtificialIntelligence;
using StoreSimulator.InteractableObjects;
using StoreSimulator.MoneySystem;
using StoreSimulator.StoreManager;
using StoreSimulator.StoreUtility;
using UnityEngine;

public class CustomerController : MonoBehaviour,
ITransformProvider, IMovable, IWalletOwner,
IPickupPointOwner, IStoreVisitor, ISmoker,
IActionProvider, IStateMachineProvider, ILogProvider,
IPlannerProvider
{
    [Header("Base Components")]
    [SerializeField] private Store store;
    [SerializeField] private NPCMovement movement;
    [SerializeField] private NPCPsycho psycho;
    [SerializeField] private NPCWallet wallet;
    [SerializeField] private Transform pickUpPoint;
    [SerializeField] private SmokingArea smokingArea;
    [SerializeField] private int buyPool = 5;
    [SerializeField] private string npcId;

    public IStore CurrentStore { get; private set; }
    public List<IStoreable> Inventory { get; private set; }
    public IStorage CurrentStorage { get; set; }
    public ICashStorage CurrentCashStorage { get; set; }
    public List<IStorage> Storages { get; private set; }
    public NPCActionQueue ActionQueue { get; private set; }
    public NPCPlanner Planner { get; private set; }
    public NPCStateMachine StateMachine { get; private set; }
    public int ItemsToBuy { get; set; }
    public string NpcId => npcId;

    public NPCMovement Movement => movement;
    public SmokingArea SmokingArea => smokingArea;
    public IWallet Wallet => wallet;
    public Transform PickUpPoint => pickUpPoint;
    public Transform Transform => transform;
    public int BuyPool => buyPool;

    private List<IGoal> _goals;

    private void Start()
    {
        CurrentStore = store;
        
        ActionQueue = new NPCActionQueue();
        Planner = new NPCPlanner();
        StateMachine = new NPCStateMachine();

        Inventory = new List<IStoreable>(BuyPool);
        Storages = new List<IStorage>();

        _goals = new List<IGoal>
        {
            new BuyProductsGoal(),
            new SmokeGoal(),
            new WaitGoal()
        };
        
        StateMachine.SetState(new GoalSelectionState(this));
    }

    private void Update()
    {
        movement.Tick();
        StateMachine.Tick();
    }

    public void Log(string msg)
    {
        Debug.Log($"[AI - {gameObject.name}] {msg}");
    }

    public List<IGoal> GetAvailableGoals() => _goals;

    public bool HaveEnoughMoney(IStoreable storeable)
    {
        float newItemPrice = PricesManager.Instance.GetPlayerPriceForItem(storeable.Data);
        float alreadyReserved = GetTotalCost(Inventory);

        return wallet.CanAfford(newItemPrice + alreadyReserved);
    }

    public void HandleDropItem(IStoreable storeable)
    {
        Inventory.Remove(storeable);
        if (((MonoBehaviour)storeable).TryGetComponent<IHoldable>(out var holdable))
        {
            holdable.Release(Vector3.zero);
        }
    }

    public void RecordVisit(float totalSpent = 0f, PriceReactionType priceReactionType = PriceReactionType.Fair)
    {
        VisitRecord visit = new VisitRecord();
        //visit.dayIndex = 

        visit.totalSpent = totalSpent;

        if (priceReactionType == PriceReactionType.Fair) visit.reactionType = psycho.GetLastReaction();
        else visit.reactionType = priceReactionType;

        // visit.foundAllItems = boughtItems.Count > 0;

        NPCMemoryManager.Instance.RecordVisit(npcId, CurrentStore.StoreID, visit);

        Debug.Log($"[AI - {gameObject.name}] New visit recorded: Total Spent: {visit.totalSpent} \n, Reaction type: {visit.reactionType} \n, All Items Found: {visit.foundAllItems}");
    }

    public float GetTotalCost(List<IStoreable> items)
    {
        float total = 0f;
        foreach (var item in items)
            total += PricesManager.Instance.GetPlayerPriceForItem(item.Data);
        return total;
    }
}
