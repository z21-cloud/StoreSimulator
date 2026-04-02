using System.Collections.Generic;
using StoreSimulator.Pathfinding;
using UnityEngine;

namespace StoreSimulator.BuildingSystem
{
    public class BuildingManager : MonoBehaviour
    {
        [SerializeField] private BuildingGrid grid;
        [SerializeField] private BuildingUIController buyMenu;
        [SerializeField] private PathfindingGrid pathfindingGrid;

        private bool _activeMenu = false;
        private Dictionary<IBuildable, List<Vector2Int>> _occupiedCells = new();

        void Update()
        {
            if (Input.GetKeyDown(KeyCode.F))
            {
                Debug.Log($"[BuildingManager] BuildingMenu is active: {_activeMenu}");
                _activeMenu = !_activeMenu;
                buyMenu.Open();
            }
        }

        public bool CanPlace(Vector2Int baseCoords, Vector2Int size)
        {
            for (int x = 0; x < size.x; x++)
            {
                for (int y = 0; y < size.y; y++)
                {
                    Vector2Int coord = baseCoords + new Vector2Int(x, y);
                    if (!grid.TryGetCell(coord, out BuildingGridCell cell) || cell.IsOccupied)
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        public bool CanTake(Vector3 position)
        {
            BuildingGridCell cell = GetCell(position);
            if (cell == null || !cell.IsOccupied) return false;

            return true;
        }

        public void PlaceBuildable(Vector2Int baseCoord, IBuildable buildable, Vector2Int size)
        {
            List<Vector2Int> coords = new List<Vector2Int>();
            for (int x = 0; x < size.x; x++)
            {
                for (int y = 0; y < size.y; y++)
                {
                    Vector2Int coord = baseCoord + new Vector2Int(x, y);
                    if (grid.TryGetCell(coord, out BuildingGridCell cell))
                    {
                        cell.buildable = buildable;
                        cell.IsOccupied = true;
                        coords.Add(coord);
                    }
                }
            }
            _occupiedCells[buildable] = coords;

            // Debug.Log($"[BuildingManager]: Placed - {((MonoBehaviour)buildable).gameObject.name}");

            if (pathfindingGrid != null) pathfindingGrid.UpdateGrid();
            else Debug.LogWarning($"[BuildingManager]: Pathfinding grid is null! Can't update");
        }

        public IBuildable TakeBuildable(Vector3 position)
        {
            BuildingGridCell cell = GetCell(position);
            if (cell == null || !cell.IsOccupied) return null;

            IBuildable buildable = cell.buildable;
            if (_occupiedCells.TryGetValue(buildable, out var coords))
            {
                foreach (var coord in coords)
                {
                    if (grid.TryGetCell(coord, out BuildingGridCell c))
                    {
                        c.IsOccupied = false;
                        c.buildable = null;
                    }
                }

                _occupiedCells.Remove(buildable);
            }

            Debug.Log($"[BuildingManager]: Placed - {((MonoBehaviour)buildable).gameObject.name}");

            if (pathfindingGrid != null) pathfindingGrid.UpdateGrid();
            else Debug.LogWarning($"[BuildingManager]: Pathfinding grid is null! Can't update");

            return buildable;
        }

        private BuildingGridCell GetCell(Vector3 position)
        {
            return grid.GetCell(position);
        }
    }
}