using System.Collections.Generic;
using Unity.Burst.Intrinsics;
using UnityEngine;

public class BuildingManager : MonoBehaviour
{
    [SerializeField] private Grid grid;

    private Dictionary<IBuildable, List<Vector2Int>> _occupiedCells = new();

    public bool CanPlace(Vector2Int baseCoords, Vector2 size)
    {
        for (int x = 0; x < size.x; x++)
        {
            for (int y = 0; y < size.y; y++)
            {
                Vector2Int checkCoords = baseCoords + new Vector2Int(x, y);
                if (!grid.TryGetCell(checkCoords, out GridCell cell) || cell.IsOccupied)
                {
                    return false;
                }
            }
        }

        return true;
    }

    public bool CanTake(Vector3 position)
    {
        GridCell cell = GetCell(position);
        if (cell == null || !cell.IsOccupied) return false;

        return true;
    }

    public void PlaceBuildable(Vector2Int baseCoord, IBuildable buildable, Vector2 size)
    {
        List<Vector2Int> coords = new List<Vector2Int>();
        for (int x = 0; x < size.x; x++)
        {
            for (int y = 0; y < size.y; y++)
            {
                Vector2Int coord = baseCoord + new Vector2Int(x, y);
                if(grid.TryGetCell(coord, out GridCell cell))
                {
                    cell.buildable = buildable;
                    cell.IsOccupied = true;
                    coords.Add(coord);
                }
            }
        }
        _occupiedCells[buildable] = coords;
    }
    public IBuildable TakeBuildable(Vector3 position)
    {
        GridCell cell = GetCell(position);
        if(cell == null || !cell.IsOccupied) return null;

        IBuildable buildable = cell.buildable;
        if(_occupiedCells.TryGetValue(buildable, out var coords))
        {
            foreach(var coord in coords)
            {
                if(grid.TryGetCell(coord, out GridCell c))
                {
                    c.IsOccupied = false;
                    c.buildable = null;
                }
            }

            _occupiedCells.Remove(buildable);
        }

        return buildable;
    }

    private GridCell GetCell(Vector3 position)
    {
        return grid.GetCell(position);
    }
}
