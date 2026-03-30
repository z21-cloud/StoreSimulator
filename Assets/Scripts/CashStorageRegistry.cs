using UnityEngine;
using System.Collections.Generic;

public class CashStorageRegistry : MonoBehaviour
{
    public static CashStorageRegistry Instance { get; private set; }
    private List<ICashStorage> cashStorages = new List<ICashStorage>();

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    public void RegisterCashStorage(ICashStorage cashStorage)
    {
        if (cashStorages.Contains(cashStorage)) return;

        cashStorages.Add(cashStorage);
    }

    public void UnregisterCashStorage(ICashStorage cashStorage)
    {
        if (!cashStorages.Contains(cashStorage)) return;

        cashStorages.Remove(cashStorage);
    }

    public ICashStorage GetRandomCashStorage()
    {
        if (cashStorages.Count <= 0)
        {
            Debug.Log($"[StorageRegistry]: No cash storages left! Return null");
            return null;
        }
        int randomIndex = Random.Range(0, cashStorages.Count);

        if (!cashStorages[randomIndex].IsOccupied)
        {
            return cashStorages[randomIndex];
        }
        else
        {
            Debug.Log($"[StorageRegistry]: No cash storages left! Return null");
            return null;
        }
    }
}
