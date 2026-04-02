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

        [Header("NPC states")]
        [SerializeField] private NPCStates states;
        [SerializeField] private NPCPsycho psycho;
        [SerializeField] private NPCWallet wallet;

        [Header("Movement logic")]
        [SerializeField] private NPCMovement movement;

        [Header("Position of each path for NPC")]
        [SerializeField] private Transform storeEnterPoint;
        [SerializeField] private Transform storeLeavePoint;
        [SerializeField] private Transform storageForItems;

        private IStorage shelf;
        private ICashStorage cashStorage;
        private List<IStoreable> boughtItems;
        private List<IStorage> shelves;
        private float waitBeforeShop = 0f;
        private float pickTimer = 0f;
        private int itemsToBuy = 0;

        void Start()
        {
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
                Debug.Log($"[AI]: Idle state");
                movement.SetDestination(storeEnterPoint.position);
            }
            if (newState == NPCState.MovingToStorage)
            {
                Debug.Log($"[AI]: Moving to shelf");
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
                    HandleWaiting();
                    return;
                }

                shelves.Clear();

                foreach (var category in npcNeeds)
                {
                    var foundShelves = StorageRegistry.Instance.GetStorageByNeeds(category);
                    Debug.Log($"[AI: {gameObject.name}] I want to buy...{category.ToString()}!");

                    if (foundShelves == null)
                    {
                        Debug.Log($"[AI] Can't find needed shelf. Leaving...");
                        ChangeState(NPCState.Leaving);
                        return;
                    }

                    if (foundShelves.Count > 0)
                    {
                        shelves.Add(foundShelves[0]);
                    }
                }

                Debug.Log($"[AI]: Try get shelf");

                if (shelves.Count > 0)
                {
                    shelf = shelves[0];
                    var peekedStoreable = shelf.PeekItem().GetComponent<IStoreable>();
                    Debug.Log($"[AI - {gameObject.name}] Moving to {peekedStoreable.Category}");
                    itemsToBuy = Random.Range(1, buyPool + 1);
                    boughtItems.Clear();
                    ChangeState(NPCState.MovingToStorage);
                }
                else
                {
                    HandleWaiting();
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
            Debug.Log($"[AI]: Taking from shelf");

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
                Debug.Log($"[AI]: Shelf is empty");
                ChangeState(NPCState.Leaving);
                return;
            }

            if (shelves.Count == 0) Debug.LogWarning($"[AI] I have no shelves!");
            shelves.RemoveAt(0);

            if (shelves.Count > 0)
            {
                shelf = shelves[0];
                Debug.Log($"[AI]: Shelves is not empty, moving to next storage...");
                ChangeState(NPCState.MovingToStorage);
                return;
            }

            cashStorage = GetCashStorage();

            Debug.Log($"[AI]: Try to find cash storage");

            if (cashStorage != null)
            {
                ChangeState(NPCState.Buying);
                return;
            }
            else
            {
                Debug.Log($"[AI]: Cash Storage is null, waiting...");
                HandleWaiting();
            }
        }

        private void HandleBuying()
        {
            Debug.Log($"[AI]: Moving to cash storage");

            if (movement.HasReached)
            {
                Debug.Log($"[AI]: Try to buy item");

                while (cashStorage.IsAvailable && boughtItems.Count != 0)
                {
                    if (!wallet.CanAfford(PricesManager.Instance.GetPriceForItem(boughtItems[0].Data)))
                    {
                        Debug.LogWarning($"[AI]: Unexpected drop at cashier - {boughtItems[0].Data.ItemName}. Check HaveEnoughMoney logic.");
                        HandleDropItem(boughtItems[0]);
                        continue;
                    }

                    Debug.Log($"[AI]: Item bought succesfully");
                    cashStorage.BuyItem(boughtItems[0], wallet);
                    psycho.IncreaseParameters(boughtItems[0].Data.FoodRestore, boughtItems[0].Data.ThirstRestore);
                    boughtItems.RemoveAt(0);
                }
                if (boughtItems.Count != 0)
                {
                    Debug.Log($"[AI]: Waiting player to buy item");
                    HandleWaiting();
                    return;
                }
                else
                {
                    ChangeState(NPCState.Leaving);
                    return;
                }
            }

        }

        private void HandleLeaving()
        {
            Debug.Log($"[AI]: Leaving store");

            shelf = null;
            cashStorage = null;
            boughtItems.Clear();

            if (movement.HasReached)
            {
                ChangeState(NPCState.Idle);
                return;
            }
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

                    Debug.Log($"[AI]: take storeable - {boughtGO.name}");
                    return true;
                }

                if (!HaveEnoughMoney(storeable)) Debug.Log($"[AI] Have not enough money!");
                // else if(storeable.Category != wantedProducts) Debug.Log($"[AI] Different categories! \n Wanter category: {wantedProducts.ToString()}. Item Category: {storeable.Category}");
                else Debug.LogWarning($"[AI]: Item has no storeable component!");
                return false;
            }

            Debug.Log($"[AI]: Can't take item. Shelf is empty");
            return false;
        }

        private bool HaveEnoughMoney(IStoreable storeable)
        {
            float newItemPrice = PricesManager.Instance.GetPriceForItem(storeable.Data);
            float alreadyReserved = GetTotalCost(boughtItems);
            return wallet.CanAfford(newItemPrice + alreadyReserved);
        }

        private float GetTotalCost(List<IStoreable> items)
        {
            float total = 0f;
            foreach (var item in items)
                total += PricesManager.Instance.GetPriceForItem(item.Data);
            return total;
        }

        private void HandleWaiting()
        {
            Debug.Log($"[AI]: Waiting state...");

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
