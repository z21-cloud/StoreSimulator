using StoreSimulator.InteractableObjects;
using UnityEngine;

public class BuildableItem : MonoBehaviour, IBuildable, IInteractable
{
    [SerializeField] private Vector2 itemSize;
    [SerializeField] private GameObject ghostPrefab;
    [SerializeField] private float wallOffset = 0f;
    [SerializeField] private float yOffset = .5f;

    public Vector2 Size => itemSize;
    public GameObject GhostPrefab => ghostPrefab;
    public float WallOffset => wallOffset;
    public float YOffset => yOffset;

    private void Start()
    {
        BuildingService.Instance.RegisterBuildable(this);
    }
    
    public string GetDescription()
    {
        throw new System.NotImplementedException();
    }

    public void OnPlaced()
    {
        throw new System.NotImplementedException();
    }

    public void OnRemoved()
    {
        throw new System.NotImplementedException();
    }
}
