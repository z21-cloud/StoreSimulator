using UnityEngine;

public class PathNode
{
    public Vector3 worldPosition;
    public Vector2Int gridIndex;
    public bool isWalkable;
    public float nodeCost; // temp cost for future A* implementation
    public PathNode parent;

    public PathNode(Vector3 worldPosition, 
                    Vector2Int gridIndex, 
                    bool isWalkable, 
                    float nodeCost, 
                    PathNode parent)
    {
        this.worldPosition = worldPosition;
        this.gridIndex = gridIndex;
        this.isWalkable = isWalkable;
        this.nodeCost = nodeCost;
        this.parent = parent;
    }
}

