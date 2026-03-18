using UnityEngine;

public interface IBuildable
{
    public Vector2Int Size { get; }
    public GameObject GhostPrefab { get; }
    public void OnPlaced();
    public void OnRemoved();
}
