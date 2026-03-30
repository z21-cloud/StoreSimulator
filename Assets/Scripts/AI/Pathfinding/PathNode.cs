using UnityEngine;

namespace StoreSimulator.Pathfinding
{
    public class PathNode
    {
        public Vector3 worldPosition;
        public Vector2Int gridIndex;
        public bool isWalkable;
        public PathNode parent;
        // path, that agent has reached
        public float gCost;
        // heuristic cost 
        public float hCost;
        // final cost
        public float fCost => gCost + hCost;

        public float cost;

        public PathNode(Vector3 worldPosition,
                        Vector2Int gridIndex,
                        bool isWalkable,
                        float cost,
                        PathNode parent)
        {
            this.worldPosition = worldPosition;
            this.gridIndex = gridIndex;
            this.isWalkable = isWalkable;
            this.parent = parent;

            this.cost = cost;

            gCost = float.MaxValue;
            hCost = 0f;
        }
    }
}

