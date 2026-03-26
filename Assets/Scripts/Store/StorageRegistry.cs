using System.Collections.Generic;
using StoreSimulator.InteractableObjects;
using UnityEngine;

public class StorageRegistry : MonoBehaviour
{
    private List<IStorage> storages = new List<IStorage>();
    private List<ICashStorage> cashStorages = new List<ICashStorage>();

    public static StorageRegistry Instance { get; private set; }

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    public void RegisterStorage(IStorage storage)
    {
        if (storages.Contains(storage)) return;

        GameObject temp = ((MonoBehaviour)storage).gameObject;

        Debug.Log($"[StorageRegistry]: Registered {temp.name}");

        storages.Add(storage);
    }

    public void UnregisterStorage(IStorage storage)
    {
        if (!storages.Contains(storage)) return;

        GameObject temp = ((MonoBehaviour)storage).gameObject;

        Debug.Log($"[StorageRegistry]: Registered {temp.name}");

        storages.Remove(storage);
    }

    public IStorage GetRandomStorage()
    {
        if (storages.Count <= 0)
        {
            Debug.Log($"[StorageRegistry]: No storages left! Return null");
            return null;
        }

        foreach (var storage in storages)
        {
            if (storage.CanTakeItem())
            {
                Debug.Log($"[StorageRegistry]: Returning not empty storage");
                return storage;
            }
        }

        int randomIndex = Random.Range(0, storages.Count);
        Debug.Log($"[StorageRegistry]: Returning empty storage");
        return storages[randomIndex];
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
