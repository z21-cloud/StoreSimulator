using System.Collections.Generic;
using UnityEngine;

public class NPCMemoryManager : MonoBehaviour
{
    public static NPCMemoryManager Instance { get; private set; }

    private Dictionary<string, Dictionary<string, NPCMemoryData>> _data = new();

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        // Dont destroyon load(gameObject);
    }

    public NPCMemoryData GetOrCreateMemoryData(string npcId, string storeId)
    {
        if(!_data.TryGetValue(npcId, out var npcStores))
        {
            npcStores = new Dictionary<string, NPCMemoryData>();
            _data[npcId] = npcStores;
        }

        if(!npcStores.TryGetValue(storeId, out var memory))
        {
            memory = ScriptableObject.CreateInstance<NPCMemoryData>();
            npcStores[storeId] = memory;
        }

        return memory;
    }

    public void RecordVisit(string npcId, string storeId, VisitRecord visit)
    {
        GetOrCreateMemoryData(npcId, storeId).AddVisit(visit);
    }
}
