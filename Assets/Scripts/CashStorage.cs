using System;
using StoreSimulator.InteractableObjects;
using UnityEngine;

public class CashStorage : MonoBehaviour, ICashStorage
{
    [SerializeField] private TryFindPlayer findPlayer;
    [SerializeField] private Transform raycastPosition;
    [SerializeField] private float raycastDistance;
    [SerializeField] private LayerMask interactableMask;
    [SerializeField] private bool auto;

    public bool IsOccupied { get; private set; }
    public bool IsAvailable { get; private set; }
    public Vector3 InteractionPoint => transform.position;

    public void BuyItem(IStoreable storeable)
    {
        if (storeable == null) return;

        BuyManager.Instance.IncreasePlayerWallet(storeable);
        Destroy(((MonoBehaviour)storeable).gameObject);
    }

    void Update()
    {
        if(!auto) IsAvailable = findPlayer.FindPlayer;
        else IsAvailable = true;

        TryFindNPC();
    }

    private void TryFindNPC()
    {
        Vector3 origin = raycastPosition.position;
        Vector3 direction = Vector3.forward;

        if (Physics.Raycast(origin, direction, out _, raycastDistance, interactableMask))
        {
            IsOccupied = true;
        }
        else
        {
            IsOccupied = false;
        }

        Debug.DrawRay(origin, direction, Color.red);
    }

    private void Start()
    {
        CashStorageRegistry.Instance.RegisterCashStorage(this);
    }

    private void OnDisable()
    {
        CashStorageRegistry.Instance.UnregisterCashStorage(this);
    }
}
