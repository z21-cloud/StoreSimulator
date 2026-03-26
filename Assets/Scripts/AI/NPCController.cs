using System;
using System.Collections.Generic;
using StoreSimulator.InteractableObjects;
using UnityEngine;

public class NPCController : MonoBehaviour
{
    [Header("Parameters")]
    [SerializeField] private float waitTime = 5f;

    [Header("NPC states")]
    [SerializeField] private NPCStates states;

    [Header("Movement logic")]
    [SerializeField] private NPCMovement movement;

    [Header("Position of each path for NPC")]
    [SerializeField] private Transform storeEnterPoint;
    [SerializeField] private Transform storeLeavePoint;
    [SerializeField] private Transform storageForItems;

    private IStorage shelf;
    private ICashStorage cashStorage;
    private IStoreable boughtItem;
    private float waitBeforeShop;


    void Start()
    {
        waitBeforeShop = waitTime;
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
            movement.SetDestination(storeEnterPoint.position);
        }
        if (newState == NPCState.MovingToStorage)
        {
            movement.SetDestination(shelf.InteractionPoint);
        }
        if (newState == NPCState.Buying)
        {
            movement.SetDestination(cashStorage.InteractionPoint);
        }
        if (newState == NPCState.Leaving)
        {
            Debug.Log($"[AI] Catch bug there. Can't find path - 2!");
            movement.SetDestination(storeLeavePoint.position);
        }

        states.SetState(newState);
    }

    private void HandleIdle()
    {
        Debug.Log($"[AI]: Idle state");
        if (movement.HasReached)
        {
            shelf = GetStorage();

            Debug.Log($"[AI]: Try get shelf");

            if (shelf != null)
            {
                ChangeState(NPCState.MovingToStorage);
            }
            else
            {
                ChangeState(NPCState.Leaving);
            }
            return;
        }
    }

    private void HandleMoving()
    {
        Debug.Log($"[AI]: Moving to shelf");

        if (movement.HasReached)
        {
            ChangeState(NPCState.TakingFromShelf);
            return;
        }
    }

    private void HandleTakingFromShelf()
    {
        Debug.Log($"[AI]: Taking from shelf");

        if (!TryTakingFromShelf())
        {
            Debug.Log($"[AI] Catch bug there. Can't find path! - 1");
            ChangeState(NPCState.Leaving);
            return;
        }

        cashStorage = GetCashStorage();

        Debug.Log($"[AI]: Try to find cash storage");

        if (cashStorage != null)
        {
            ChangeState(NPCState.Buying);
        }
        else
        {
            Debug.Log($"[AI]: Cash Storage is null, waiting...");
            HandleWaiting();
            return;
        }
    }

    private void HandleBuying()
    {
        Debug.Log($"[AI]: Moving to cash storage");

        if (movement.HasReached)
        {
            Debug.Log($"[AI]: Try to buy item");

            if (cashStorage.IsAvailable)
            {
                Debug.Log($"[AI]: Item bought succesfully");
                cashStorage.BuyItem(boughtItem);
                boughtItem = null;
                ChangeState(NPCState.Leaving);
            }
            else
            {
                Debug.Log($"[AI]: Waiting player to buy item");
                HandleWaiting();
            }
            return;
        }

    }

    private void HandleLeaving()
    {
        Debug.Log($"[AI]: Leaving store");

        shelf = null;

        if (movement.HasReached)
        {
            ChangeState(NPCState.Idle);
            return;
        }
    }

    private IStorage GetStorage()
    {
        return StorageRegistry.Instance.GetRandomStorage();
    }

    private ICashStorage GetCashStorage()
    {
        return StorageRegistry.Instance.GetRandomCashStorage();
    }

    private bool TryTakingFromShelf()
    {
        if (boughtItem != null)
        {
            Debug.Log("[AI]: Can't take item, don't have enough slots");
            return false;
        }

        transform.LookAt(shelf.InteractionPoint);

        if (shelf.CanTakeItem())
        {
            GameObject go = shelf.TakeItem(transform.position);
            if (go.TryGetComponent<IStoreable>(out var storeable))
            {
                boughtItem = storeable;

                GameObject boughtGO = storeable.OnPickedFromStore();
                boughtGO.transform.position = storageForItems.position;
                boughtGO.transform.parent = storageForItems;

                Debug.Log($"[AI]: take storeable");
                return true;
            }

            Debug.Log($"[AI]: Taken item is not StoreableItem!");
            return false;
        }

        Debug.Log($"[AI]: Can't take item. Shelf is empty");
        return false;
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
}
