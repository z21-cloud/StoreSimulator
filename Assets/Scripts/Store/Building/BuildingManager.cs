using UnityEngine;

public class BuildingManager : MonoBehaviour
{
    [SerializeField] private Grid grid;

    public bool CanPlace(Vector3 position)
    {
        GridCell cell = GetCell(position);
        if(cell == null || cell.IsOccupied) return false;
        
        return true;
    }

    public bool CanTake(Vector3 position)
    {
        GridCell cell = GetCell(position);
        if(cell == null || !cell.IsOccupied) return false;
        
        return true;
    }

    public void PlaceBuildable(Vector3 position, IBuildable buildable)
    {
        GridCell cell = GetCell(position);
        cell.buildable = buildable;
        cell.IsOccupied = true;
    }

    public IBuildable TakeBuildable(Vector3 position)
    {
        GridCell cell = GetCell(position);
        IBuildable buildable = cell.buildable;
        cell.IsOccupied = false;
        cell.buildable = null;
        return buildable;
    }

    private GridCell GetCell(Vector3 position)
    {
        return grid.GetCell(position);
    }
}
