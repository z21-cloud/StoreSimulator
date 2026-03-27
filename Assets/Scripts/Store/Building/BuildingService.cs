using System.Collections.Generic;
using UnityEngine;

public class BuildingService : MonoBehaviour
{
    [SerializeField] BuildingManager buildingManager;

    public static BuildingService Instance;

    private List<IBuildable> buildables = new List<IBuildable>();
    
    private void Awake()
    {
        if(Instance != null && Instance != this)
        {
            Destroy(this);
            return;
        }

        buildables = new List<IBuildable>();

        Instance = this;
    }

    public void RegisterBuildable(IBuildable buildable)
    {
        if(buildables.Contains(buildable)) return;

        Debug.Log($"[BuildingService]: Registere - {((MonoBehaviour)buildable).gameObject.name}");

        buildables.Add(buildable);
    }

    public List<IBuildable> GetAllBuildable()
    {
        if(buildables.Count <= 0) return null;

        Debug.Log($"[BuildingService]: Returns - {buildables.Count}");

        return buildables;
    }

    public void UnregisterBuildable(IBuildable buildable)
    {
        if(!buildables.Contains(buildable)) return;

        Debug.Log($"[BuildingService]: Unregistere - {((MonoBehaviour)buildable).gameObject.name}");

        buildables.Remove(buildable);
    }
}
