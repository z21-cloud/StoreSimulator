using System;
using System.Collections.Generic;
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
    [SerializeField] private PathfindingGrid pathfinding;

    private IStorage goalStorage;
    private GameObject boughtObj;
    private NPCState _currentState;
    private float waitBeforeShop;

    private Queue<Vector3> _currentPath = new Queue<Vector3>();

    void Start()
    {
        _currentState = NPCState.Idle;
        waitBeforeShop = waitTime;
        goalStorage = null;
    }

    void Update()
    {
        //Debug.Log($"[NPC Conrtoller]: Current state is {_currentState}");

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
        if (newState == NPCState.MovingToStorage)
        {
            var path = pathfinding.FindPath(transform.position, goalStorage.InteractionPoint.position);
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
            goalStorage = GetStorage();

            if (goalStorage != null)
            {
                ChangeState(NPCState.MovingToStorage);
            }
            else
            {
                ChangeState(NPCState.Leaving);
            }
            return;
        }

        Vector3 target = _currentPath.Peek();
        mover.MoveTo(target, interactionDistance);
        transform.LookAt(target);

        if (!mover.IsMoving) _currentPath.Dequeue();
    }

    private void HandleMoving()
    {
        if (_currentPath.Count == 0)
        {
            ChangeState(NPCState.Buying);
            return;
        }

        Vector3 target = _currentPath.Peek();
        mover.MoveTo(target, interactionDistance);
        transform.LookAt(target);
        if (!mover.IsMoving) _currentPath.Dequeue();
    }

    private void HandleBuying()
    {
        BuyItem();
        ChangeState(NPCState.Leaving);
    }

    private void HandleLeaving()
    {
        goalStorage = null;

        if (_currentPath.Count == 0)
        {
            Destroy(boughtObj);
            ChangeState(NPCState.Waiting);
            return;
        }

        Vector3 target = _currentPath.Peek();
        mover.MoveTo(target, interactionDistance);
        transform.LookAt(target);

        if (!mover.IsMoving) _currentPath.Dequeue();
    }

    private IStorage GetStorage()
    {
        return StorageRegistry.Instance.GetRandomStorage();
    }

    private void BuyItem()
    {
        if (boughtObj != null) return;

        GameObject go = goalStorage.TakeItem(transform.position);
        transform.LookAt(((MonoBehaviour)goalStorage).transform.position);
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
