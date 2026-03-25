using System;
using System.Collections.Generic;
using StoreSimulator.InteractableObjects;
using UnityEngine;

public enum NPCState
{
    Idle,
    MovingToStorage,
    TakingFromShelf,
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
    [SerializeField] private AStar pathfinding;

    private IStorage goalShelf;
    private ICashStorage goalCashStorage;
    private IStoreable boughtItem;
    private NPCState _currentState;
    private float waitBeforeShop;

    private Queue<Vector3> _currentPath = new Queue<Vector3>();

    void Start()
    {
        _currentState = NPCState.Idle;
        waitBeforeShop = waitTime;
        goalShelf = null;
    }

    void Update()
    {
        switch (_currentState)
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
        if (newState == NPCState.MovingToStorage)
        {
            var path = pathfinding.FindPath(transform.position, ((MonoBehaviour)goalShelf).transform.position);
            _currentPath = new Queue<Vector3>(path);
        }
        if(newState == NPCState.Buying)
        {
            var path = pathfinding.FindPath(transform.position, ((MonoBehaviour)goalCashStorage).transform.position);
            _currentPath = new Queue<Vector3>(path);
        }
        if (newState == NPCState.Leaving)
        {
            var path = pathfinding.FindPath(transform.position, leavePoint.position);
            _currentPath = new Queue<Vector3>(path);
        }

        _currentState = newState;
    }

    private void HandleIdle()
    {
        if (_currentPath.Count == 0)
        {
            goalShelf = GetStorage();

            if (goalShelf != null)
            {
                ChangeState(NPCState.MovingToStorage);
            }
            else
            {
                ChangeState(NPCState.Leaving);
            }
            return;
        }

        GoThrowPath();
    }

    private void GoThrowPath()
    {
        Vector3 target = _currentPath.Peek();
        mover.MoveTo(target, interactionDistance);
        transform.LookAt(target);

        if (!mover.IsMoving) _currentPath.Dequeue();
    }

    private void HandleMoving()
    {
        if (_currentPath.Count == 0)
        {
            ChangeState(NPCState.TakingFromShelf);
            return;
        }

        GoThrowPath();
    }

    private void HandleTakingFromShelf()
    {
        BuyItem();
        
        goalCashStorage = GetCashStorage();

        if(goalCashStorage != null)
        {
            ChangeState(NPCState.Buying);
        }
        else
        {
            HandleWaiting();
        }
    }

    private void HandleBuying()
    {
        if (_currentPath.Count == 0)
        {
            if(goalCashStorage.IsAvailable)
            {
                goalCashStorage.BuyItem(boughtItem);
                boughtItem = null;
                ChangeState(NPCState.Leaving);
            }
            else
            {
                HandleWaiting();
            }
            return;
        }

        GoThrowPath();
    }

    private void HandleLeaving()
    {
        goalShelf = null;

        if (_currentPath.Count == 0)
        {
            ChangeState(NPCState.Idle);
            return;
        }

        GoThrowPath();
    }

    private IStorage GetStorage()
    {
        return StorageRegistry.Instance.GetRandomStorage();
    }

    private ICashStorage GetCashStorage()
    {
        return StorageRegistry.Instance.GetRandomCashStorage();
    }

    private void BuyItem()
    {
        if (boughtItem != null) return;

        GameObject go = goalShelf.TakeItem(transform.position);
        transform.LookAt(((MonoBehaviour)goalShelf).transform.position);
        if (go.TryGetComponent<IStoreable>(out var storeable))
        {
            boughtItem = storeable;
            storeable.OnPickedFromStore();
            ((MonoBehaviour)storeable).transform.position = storageForItems.position;
            ((MonoBehaviour)storeable).transform.parent = storageForItems;
        }
        Debug.Log($"[NPCConrtoller]: taken");
    }

    private void HandleWaiting()
    {
        waitBeforeShop -= Time.deltaTime;
        if (waitBeforeShop <= 0f)
        {
            ChangeState(_currentState);
            waitBeforeShop = waitTime;
        }
    }
}
