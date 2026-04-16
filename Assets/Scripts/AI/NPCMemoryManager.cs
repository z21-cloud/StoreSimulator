using System.Collections.Generic;
using UnityEngine;

public class NPCMemoryManager : MonoBehaviour
{
    public static NPCMemoryManager Instance { get; private set; }

    private Dictionary<string, NPCMemoryData> _memories = new();

    private void Awake()
    {
        if(Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        // Dont destroyon load(gameObject);
    }

    public void Register(string id, NPCMemoryData memory)
    {
        _memories[id] = memory;
    }

    public NPCMemoryData GetMemoryData(string id)
    {
        return _memories.TryGetValue(id, out NPCMemoryData value) ? value : null;
    }

    public void RecordVisit(string id, VisitRecord visit)
    {
        NPCMemoryData memoryData = GetMemoryData(id);
        if(memoryData == null) return;

        memoryData.AddVisit(visit);
    }
}
