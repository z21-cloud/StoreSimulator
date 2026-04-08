using System.Collections.Generic;
using StoreSimulator.InteractableObjects;
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
        [SerializeField] private NPCStates states;
        [SerializeField] private NPCPsycho psycho;
        [SerializeField] private NPCWallet wallet;
        [SerializeField] private NPCMemoryData memoryData;

        [Header("Movement logic")]
        [SerializeField] private NPCMovement movement;

        [Header("Position of each path for NPC")]
        [SerializeField] private Transform storeEnterPoint;
        [SerializeField] private Transform storeLeavePoint;
        [SerializeField] private Transform storageForItems;
        [SerializeField] private string npcId;

        private IStorage shelf;
        private ICashStorage cashStorage;
        private List<IStoreable> boughtItems;
        private List<IStorage> shelves;
        private float waitBeforeShop = 0f;
        private float pickTimer = 0f;
        private int itemsToBuy = 0;
        private bool _priceEvaluatedForCurrentShelf = false;
        private bool _currentShelfDealAccepted = false;
        public string NpcId => npcId;

        void Start()
        {
            NPCMemoryManager.Instance.Register(NpcId, memoryData);
            NPCMemoryManager.Instance.RecordVisit(NpcId, new VisitRecord());

            waitBeforeShop = waitTime;

            shelves = new List<IStorage>();
            boughtItems = new List<IStoreable>(buyPool);
            
            ChangeState(states.CurrentState);
        }

        void Update()
        {
            movement.Tick();

            switch (states.CurrentState)
            {
                case NPCState.Idle: HandleIdle(); break;
                case NPCState.MovingToStorage: HandleMoving(); break;
                case NPCState.TakingFromShelf: HandleTakingFromShelf(); break;
                case NPCState.Buying: HandleBuying(); break;
                case NPCState.Leaving: HandleLeaving(); break;
            }
        }

        private void ChangeState(NPCState newState)
        {
            if (newState == NPCState.Idle)
            {
                Debug.Log($"[AI - {gameObject.name}]: Idle state");
                movement.SetDestination(storeEnterPoint.position);
            }
            if (newState == NPCState.MovingToStorage)
            {
                Debug.Log($"[AI - {gameObject.name}]: Moving to shelf");
                movement.SetDestination(shelf.InteractionPoint);
            }
            if (newState == NPCState.Buying)
            {
                movement.SetDestination(cashStorage.InteractionPoint);
            }
            if (newState == NPCState.Leaving)
            {
                movement.SetDestination(storeLeavePoint.position);
            }

            states.SetState(newState);
        }

        private void HandleIdle()
        {
            if (!CheckStoreState()) ChangeState(NPCState.Leaving);

            if (!psycho.WantBuyProducts) ChangeState(NPCState.Leaving);

            if (movement.HasReached)
            {
                List<ItemCategory> npcNeeds = psycho.GetPriorityNeeds();

                if (npcNeeds.Count == 0)
                {
                    ChangeState(NPCState.Leaving);
                    return;
                }

                shelves.Clear();

                foreach (var category in npcNeeds)
                {
                    var foundShelves = StorageRegistry.Instance.GetStorageByNeeds(category);
                    Debug.Log($"[AI: {gameObject.name}] I want to buy...{category.ToString()}!");

                    if (foundShelves == null)
                    {
                        Debug.Log($"[AI - {gameObject.name}] Can't find needed shelf. Leaving...");

                        float totalSpent = 0f;
                        RecordVisit(totalSpent);
                        ChangeState(NPCState.Leaving);
                        return;
                    }

                    if (foundShelves.Count > 0)
                    {
                        shelves.Add(foundShelves[0]);
                    }
                }

                Debug.Log($"[AI - {gameObject.name}]: Try get shelf");

                if (shelves.Count > 0)
                {
                    shelf = shelves[0];
                    var peekedStoreable = shelf.PeekItem().GetComponent<IStoreable>();
                    Debug.Log($"[AI - {gameObject.name}] Moving to {peekedStoreable.Category}");
                    itemsToBuy = Random.Range(1, buyPool + 1);
                    boughtItems.Clear();
                    ChangeState(NPCState.MovingToStorage);
                }
                return;
            }
        }

        private void HandleMoving()
        {
            if (movement.HasReached)
            {
                ChangeState(NPCState.TakingFromShelf);
                return;
            }
        }

        private void HandleTakingFromShelf()
        {
            Debug.Log($"[AI - {gameObject.name}]: Taking from shelf");

            if (!_priceEvaluatedForCurrentShelf && shelf.CanTakeItem())
            {
                var sample = shelf.PeekItem().GetComponent<IStoreable>();
                float playerPrice = PricesManager.Instance.GetPlayerPriceForItem(sample.Data);
                float marketPrice = PricesManager.Instance.GetMarketPriceForItem(sample.Data);
                _currentShelfDealAccepted = psycho.BuyItemOrNot(playerPrice, marketPrice);
                _priceEvaluatedForCurrentShelf = true;
            }

            if (!_currentShelfDealAccepted)
            {
                RecordVisit(0f);
                ChangeState(NPCState.Leaving);
                return;
            }

            if (TryTakingFromShelf() && boughtItems.Count < itemsToBuy)
            {
                pickTimer -= Time.deltaTime;
                if (pickTimer <= 0f)
                {
                    pickTimer = pickDelay;
                }
                return;
            }

            if (boughtItems.Count == 0)
            {
                Debug.Log($"[AI - {gameObject.name}]: Shelf is empty");

                ChangeState(NPCState.Leaving);
                return;
            }

            if (shelves.Count == 0) Debug.LogWarning($"[AI - {gameObject.name}] I have no shelves!");
            else shelves.RemoveAt(0);

            if (shelves.Count > 0)
            {
                shelf = shelves[0];
                Debug.Log($"[AI - {gameObject.name}]: Shelves is not empty, moving to next storage...");
                ChangeState(NPCState.MovingToStorage);
                return;
            }

            cashStorage = GetCashStorage();

            Debug.Log($"[AI - {gameObject.name}]: Try to find cash storage");

            if (cashStorage != null)
            {
                ChangeState(NPCState.Buying);
                return;
            }
            else
            {
                Debug.Log($"[AI - {gameObject.name}]: Cash Storage is null, waiting...");
                HandleWaiting();
            }
        }

        private void HandleBuying()
        {
            Debug.Log($"[AI - {gameObject.name}]: Moving to cash storage");

            if (movement.HasReached)
            {
                Debug.Log($"[AI - {gameObject.name}]: Try to buy item");

                float totalSpent = GetTotalCost(boughtItems);

                while (cashStorage.IsAvailable && boughtItems.Count != 0)
                {
                    if (!wallet.CanAfford(PricesManager.Instance.GetPlayerPriceForItem(boughtItems[0].Data)))
                    {
                        Debug.LogWarning($"[AI - {gameObject.name}]: Unexpected drop at cashier - {boughtItems[0].Data.ItemName}. Check HaveEnoughMoney logic.");
                        HandleDropItem(boughtItems[0]);
                        continue;
                    }

                    Debug.Log($"[AI - {gameObject.name}]: Item bought succesfully");
                    cashStorage.BuyItem(boughtItems[0], wallet);
                    psycho.IncreaseParameters(boughtItems[0].Data.FoodRestore, boughtItems[0].Data.ThirstRestore);
                    boughtItems.RemoveAt(0);
                }
                if (boughtItems.Count != 0)
                {
                    Debug.Log($"[AI - {gameObject.name}]: Waiting player to buy item");
                    HandleWaiting();
                    return;
                }

                RecordVisit(totalSpent);
                ChangeState(NPCState.Leaving);
                return;
            }

        }

        private void HandleLeaving()
        {
            Debug.Log($"[AI - {gameObject.name}]: Leaving store");

            psycho.ResetReaction();

            shelf = null;
            cashStorage = null;
            boughtItems.Clear();

            if (movement.HasReached)
            {
                ChangeState(NPCState.Idle);
                return;
            }
        }

        private void RecordVisit(float totalSpent = 0f)
        {
            VisitRecord visit = new VisitRecord();
            //visit.dayIndex = 

            visit.totalSpent = totalSpent;

            visit.reactionType = psycho.GetLastReaction();

            // visit.foundAllItems = boughtItems.Count > 0;

            NPCMemoryManager.Instance.RecordVisit(npcId, visit);

            Debug.Log($"[AI - {gameObject.name}] New visit recorded: Total Spent: {visit.totalSpent} \n, Reaction type: {visit.reactionType} \n, All Items Found: {visit.foundAllItems}");
        }

        private ICashStorage GetCashStorage()
        {
            return CashStorageRegistry.Instance.GetRandomCashStorage();
        }

        private bool TryTakingFromShelf()
        {
            transform.LookAt(shelf.InteractionPoint);

            if (shelf.CanTakeItem())
            {

                GameObject go = shelf.PeekItem();
                if (go.TryGetComponent<IStoreable>(out var storeable) && HaveEnoughMoney(storeable))
                {
                    boughtItems.Add(storeable);

                    GameObject boughtGO = storeable.OnPickedFromStore();
                    boughtGO.transform.position = storageForItems.position;
                    boughtGO.transform.parent = storageForItems;

                    Debug.Log($"[AI - {gameObject.name}]: take storeable - {boughtGO.name}");
                    return true;
                }

                if (!HaveEnoughMoney(storeable)) Debug.Log($"[AI - {gameObject.name}] Have not enough money!");
                // else if(storeable.Category != wantedProducts) Debug.Log($"[AI - {gameObject.name}] Different categories! \n Wanter category: {wantedProducts.ToString()}. Item Category: {storeable.Category}");
                else Debug.LogWarning($"[AI - {gameObject.name}]: Item has no storeable component!");
                return false;
            }

            Debug.Log($"[AI - {gameObject.name}]: Can't take item. Shelf is empty");
            return false;
        }

        private bool HaveEnoughMoney(IStoreable storeable)
        {
            float newItemPrice = PricesManager.Instance.GetPlayerPriceForItem(storeable.Data);
            float alreadyReserved = GetTotalCost(boughtItems);

            return wallet.CanAfford(newItemPrice + alreadyReserved);
        }

        private float GetTotalCost(List<IStoreable> items)
        {
            float total = 0f;
            foreach (var item in items)
                total += PricesManager.Instance.GetPlayerPriceForItem(item.Data);
            return total;
        }

        private void HandleWaiting()
        {
            Debug.Log($"[AI - {gameObject.name}]: Waiting state...");

            waitBeforeShop -= Time.deltaTime;
            if (waitBeforeShop <= 0f)
            {
                ChangeState(states.CurrentState);
                waitBeforeShop = waitTime;
            }
        }

        private void HandleDropItem(IStoreable storeable)
        {
            boughtItems.Remove(storeable);
            if (((MonoBehaviour)storeable).TryGetComponent<IHoldable>(out var holdable))
            {
                holdable.Release(Vector3.zero);
            }
        }

        private bool CheckStoreState() => StoreUtility.StoreManager.Instance.IsOpen;
    }
}
