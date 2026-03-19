using StoreSimulator.InteractableObjects;
using UnityEngine;

public class BuildableItem : MonoBehaviour, IBuildable, IInteractable
{
    [SerializeField] private Vector2 itemSize;
    [SerializeField] private GameObject ghostPrefab;
    [SerializeField] private float wallOffset = 0f;

    public Vector2 Size => itemSize;
    public float WallOffset => wallOffset;
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
