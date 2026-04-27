using System.Collections.Generic;
using StoreSimulator.InteractableObjects;
using UnityEngine;

public class StoreablePooling : MonoBehaviour
{
    [SerializeField] private List<StoreableItem> prefabs;
    [SerializeField] private Transform parent;
    [SerializeField] private int initialSize = 100;

    public static StoreablePooling Instance { get; private set; }

    private Dictionary<StoreableItem, ObjectPooling<StoreableItem>> _pools;

    private void Awake()
    {
        if(Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        _pools = new Dictionary<StoreableItem, ObjectPooling<StoreableItem>>();

        foreach (var prefab in prefabs)
        {
            _pools[prefab] = new ObjectPooling<StoreableItem>(prefab, initialSize, parent);
        }
    }

    public IStoreable GetStoreable(StoreableItem prefab)
    {
        if (_pools.TryGetValue(prefab, out var pool))
        {
            return pool.Get();
        }

        Debug.LogError($"[StoreablePooling]: Префаб {prefab.name} не зарегистрирован в пуле!");
        return null;
    }


    public void ReturnStoreable(IStoreable storeable, StoreableItem prefab)
    {
        if (storeable is StoreableItem concreteStoreable)
        {
            if (_pools.TryGetValue(prefab, out var pool))
            {
                Debug.Log($"[StoreablePool]: returns to pool {((MonoBehaviour)prefab).gameObject}");
                pool.Release(concreteStoreable);
            }
        }
        else
        {
            Debug.LogError($"[StoreablePooling]: Объект не является StoreableItem");
        }
    }
}
