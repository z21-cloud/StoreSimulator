using System.Collections.Generic;
using StoreSimulator.StoreUtility;
using UnityEngine;

public class StoreRegistry : MonoBehaviour
{
    public static StoreRegistry Instance { get; private set; }

    private List<Store> _stores = new List<Store>();

    private void Awake()
    {
        if(Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    } 

    public void RegisterStore(Store store)
    {
        if(_stores.Contains(store)) return;

        _stores.Add(store);
    }

    public Store GetRandomStore()
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
