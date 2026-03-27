using UnityEngine;

public interface IBuildable
{
    public Vector2 Size { get; }
    public GameObject GhostPrefab { get; }
    public float WallOffset { get; }
    public float YOffset { get; }

    public void OnPlaced();
    public void OnRemoved();
}
