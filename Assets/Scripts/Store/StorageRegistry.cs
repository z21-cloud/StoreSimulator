using System.Collections.Generic;
using StoreSimulator.InteractableObjects;
using UnityEngine;

public class StorageRegistry : MonoBehaviour
{
    private List<IStorage> storages = new List<IStorage>();

    public static StorageRegistry Instance {get; private set;}

    void Awake()
    {
        if(Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    public void RegisterStorage(IStorage storage)
    {
        if(storages.Contains(storage) || !storage.CanTakeItem()) return;

        storages.Add(storage);
    }

    public void UnregisterStorage(IStorage storage)
    {
        if(!storages.Contains(storage)) return;

        storages.Remove(storage);
    }

    public IStorage GetRandomStorage()
    {
        if(storages.Count <= 0)
        {
            Debug.Log($"[StorageRegistry]: No storages left! Return null");
            return null;
        }
        int randomIndex = Random.Range(0, storages.Count);

        if(storages[randomIndex].CanTakeItem())
        {
            return storages[randomIndex];
        }
        else
        {
            storages.Remove(storages[randomIndex]);
        }

        return GetRandomStorage();
    }
}
