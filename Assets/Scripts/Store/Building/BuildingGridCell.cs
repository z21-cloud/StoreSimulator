using UnityEngine;

namespace StoreSimulator.BuildingSystem
{
    public class BuildingGridCell
    {
        public Vector2Int gridPosition;
        public Vector3 worldPosittion;
        public bool IsOccupied;
        public IBuildable buildable;

        public BuildingGridCell(Vector2Int gridPosition, Vector3 worldPosittion, bool IsOccupied, IBuildable buildable)
        {
            this.gridPosition = gridPosition;
            this.worldPosittion = worldPosittion;
            this.IsOccupied = IsOccupied;
            this.buildable = buildable;
        }
    }
}
