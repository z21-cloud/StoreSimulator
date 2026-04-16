using System;
using StoreSimulator.InteractableObjects;
using StoreSimulator.MoneySystem;
using StoreSimulator.StoreManager;
using StoreSimulator.StoreUtility;
using UnityEngine;

public class CashStorage : MonoBehaviour, ICashStorage
{
    [SerializeField] private TryFindCashier findCashier;
    [SerializeField] private Transform raycastPosition;
    [SerializeField] private float raycastDistance;
    [SerializeField] private LayerMask interactableMask;
    [SerializeField] private Transform cashierPoint;
    [SerializeField] private Transform interactionPoint;
    [SerializeField] private bool auto;

    private Store _storeOwner;

    public bool IsOccupied { get; private set; }
    public bool IsAvailable { get; private set; }
    public Vector3 InteractionPoint => interactionPoint.position;
    public Vector3 CashierPoint => cashierPoint.position;

    public void BuyItem(IStoreable storeable, IWallet wallet)
    {
        if (storeable == null) return;

        findCashier.CashierWallet.Add(PricesManager.Instance.GetPlayerPriceForItem(storeable.Data));
        wallet.Spend(PricesManager.Instance.GetPlayerPriceForItem(storeable.Data));
        Destroy(((MonoBehaviour)storeable).gameObject);
    }

    void Update()
    {
        if(!auto) IsAvailable = findCashier.FindCashier;
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

    private void Awake()
    {
        _storeOwner = GetComponentInParent<Store>();
    }

    void OnEnable()
    {
        _storeOwner.CashStorageRegistry.RegisterCashStorage(this);
    }

    private void OnDisable()
    {
        _storeOwner.CashStorageRegistry.UnregisterCashStorage(this);
    }
}
