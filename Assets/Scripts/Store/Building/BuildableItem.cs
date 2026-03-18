using StoreSimulator.InteractableObjects;
using UnityEngine;

public class BuildableItem : MonoBehaviour, IBuildable, IInteractable
{
    [SerializeField] private Vector2Int itemSize;
    [SerializeField] private GameObject ghostPrefab;
    public Vector2Int Size => itemSize;

    public GameObject GhostPrefab => ghostPrefab;

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
