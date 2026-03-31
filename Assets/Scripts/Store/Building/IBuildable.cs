using UnityEngine;

namespace StoreSimulator.BuildingSystem
{
    public interface IBuildable
    {
        public Vector2Int Size { get; }
        public GameObject GhostPrefab { get; }
        public Vector3 BuildPosition { get; }
        public float BuildRotationY { get; }
        public float WallOffset { get; }
        public float YOffset { get; }

        public void OnPlaced();
        public void OnRemoved();
    }
}
