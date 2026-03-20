using System;
using StoreSimulator.InteractableObjects;
using UnityEngine;

public enum NPCState
{
    Idle,
    MovingToStorage,
    Buying,
    Leaving,
    Waiting
}

public class NPCController : MonoBehaviour
{
    [SerializeField] private float interactionDistance = 2f;
    [SerializeField] private NPCMover mover;
    [SerializeField] private Transform goal;
    [SerializeField] private Transform store;
    [SerializeField] private Transform leavePoint;
    [SerializeField] private Transform storageForItems;
    [SerializeField] private float waitTime = 5f;

    private IStorage goalStorage;
    private GameObject boughtObj;
    private NPCState _currentState;
    private float waitBeforeShop;

    void Start()
    {
        _currentState = NPCState.Idle;
        waitBeforeShop = waitTime;
        goalStorage = null;
    }

    void Update()
    {
        Debug.Log($"[NPC Conrtoller]: Current state is {_currentState}");

        switch (_currentState)
        {
            case NPCState.Idle: HandleIdle(); break;
            case NPCState.MovingToStorage: HandleMoving(); break;
            case NPCState.Buying: HandleBuying(); break;
            case NPCState.Leaving: HandleLeaving(); break;
            case NPCState.Waiting: HandleWaiting(); break;
        }
    }

    private void ChangeState(NPCState newState)
    {
        _currentState = newState;
    }

    private void HandleIdle()
    {
        mover.MoveTo(store.position, interactionDistance);

        if (!mover.IsMoving)
        {
            goalStorage = GetStorage();

            if (goalStorage != null)
            {
                ChangeState(NPCState.MovingToStorage);
            }
            else
            {
                ChangeState(NPCState.Leaving);
            }
        }
    }

    private void HandleMoving()
    {
        mover.MoveTo(((MonoBehaviour)goalStorage).transform.position, interactionDistance);

        if (!mover.IsMoving) ChangeState(NPCState.Buying);
    }

    private void HandleBuying()
    {
        BuyItem();
        ChangeState(NPCState.Leaving);
    }

    private void HandleLeaving()
    {
        goalStorage = null;

        mover.MoveTo(leavePoint.position, interactionDistance);

        if (!mover.IsMoving)
        {
            Destroy(boughtObj);
            ChangeState(NPCState.Waiting);
        }
    }

    private IStorage GetStorage()
    {
        return StorageRegistry.Instance.GetRandomStorage();
    }

    private void BuyItem()
    {
        if (boughtObj != null) return;

        GameObject go = goalStorage.TakeItem(transform.position);
        if (go.TryGetComponent<IStoreable>(out var storeable))
        {
            boughtObj = storeable.OnPickedFromStore();
            boughtObj.transform.position = storageForItems.position;
            boughtObj.transform.parent = storageForItems;
            BuyManager.Instance.IncreasePlayerWallet(storeable);
        }
        Debug.Log($"[NPCConrtoller]: taken");
    }

    private void HandleWaiting()
    {
        waitBeforeShop -= Time.deltaTime;
        if (waitBeforeShop <= 0f)
        {
            ChangeState(NPCState.Idle);
            waitBeforeShop = waitTime;
        }
    }
}
