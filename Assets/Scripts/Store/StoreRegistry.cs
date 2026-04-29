using System.Collections.Generic;
using StoreSimulator.StoreUtility;
using UnityEngine;

public class StoreRegistry : MonoBehaviour
{
    public static StoreRegistry Instance { get; private set; }

    private List<IStore> _stores = new List<IStore>();

    private void Awake()
    {
        if(Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    } 

    public void RegisterStore(IStore store)
    {
        if(_stores.Contains(store)) return;

        _stores.Add(store);
    }

    public IStore GetRandomStore()
    {
        if(_stores.Count > 0) 
        {
            var num = Random.Range(0, _stores.Count);
            return _stores[num];
        }
        else
        {
            Debug.LogError($"No Stores registered! return null!");
            return null;
        }
    }
}
