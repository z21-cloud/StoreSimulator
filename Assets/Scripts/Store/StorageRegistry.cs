using System.Collections.Generic;
using StoreSimulator.InteractableObjects;
using UnityEngine;

public class StorageRegistry : MonoBehaviour
{
    private List<IStorage> storages = new List<IStorage>();

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

        //Debug.Log($"[StorageRegistry]: Registered {((MonoBehaviour)storage).gameObject.name}");

        storages.Add(storage);
    }

    public void UnregisterStorage(IStorage storage)
    {
        if (!storages.Contains(storage)) return;

        //Debug.Log($"[StorageRegistry]: Unregistered {((MonoBehaviour)storage).gameObject.name}");

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
}
