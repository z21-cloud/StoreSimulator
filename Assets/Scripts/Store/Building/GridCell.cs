using UnityEngine;
using UnityEngine.UI;

public class GridCell
{
    public Vector2Int gridPosition;
    public Vector3 worldPosittion;
    public bool IsOccupied;
    public IBuildable buildable;

    public GridCell(Vector2Int gridPosition, Vector3 worldPosittion, bool IsOccupied, IBuildable buildable)
    {
        this.gridPosition = gridPosition;
        this.worldPosittion = worldPosittion;
        this.IsOccupied = IsOccupied;
        this.buildable = buildable;
    }
}
