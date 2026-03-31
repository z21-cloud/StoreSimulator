using System;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;

namespace StoreSimulator.BuildingSystem
{
    public class BuildingGrid : MonoBehaviour
    {
        [SerializeField] private Vector2Int gridSize;
        [SerializeField] private Vector2 slotSize = new Vector2(1, 1);
        [SerializeField] private GameObject invisiblePlane;
        [SerializeField] private GameObject gridCellImage;
        [SerializeField] private List<GameObject> buildablePrefabs;
        [SerializeField] private bool debug;
        [SerializeField] private BuildingManager buildingManager;
        public Vector2 SlotSize => slotSize;
        private Dictionary<string, GameObject> _prefabRegistry;
        private Dictionary<Vector2Int, BuildingGridCell> grid;

        void Awake()
        {
            CreateGrid();
        }

        void Start()
        {
            RegisterPlacedObjects();
        }

        private void RegisterPlacedObjects()
        {
            List<IBuildable> buildables = BuildingRegisterService.Instance.GetAllBuildable();

            if (buildables == null)
            {
                Debug.Log($"[BuildingGrid]: No buildables registered");
                return;
            }

            foreach (var buildable in buildables)
            {
                Vector2Int coords = WorldToGrid(buildable.BuildPosition);

                if (!TryGetCell(coords, out _))
                {
                    Debug.Log($"[BuildingGrid]: can't find grid by next coords: {coords}");
                    continue;
                }

                // Debug.Log($"[BuildingGrid]: Place buildable: {buildable}");

                Vector2Int size = PlacementSystem.GetRotatedSize(buildable, buildable.BuildRotationY);
                buildingManager.PlaceBuildable(coords, buildable, size);
            }
        }

        public void CreateGrid()
        {
            InitialGrid();

            invisiblePlane.transform.localScale = new Vector3(gridSize.x * slotSize.x, 1f, gridSize.y * slotSize.y);

            invisiblePlane.transform.position = new Vector3
            (
                transform.position.x + gridSize.x / 2f * slotSize.x,
                transform.position.y,
                transform.position.z + gridSize.y / 2f * slotSize.y
            );

            _prefabRegistry = new Dictionary<string, GameObject>();
            foreach (var prefab in buildablePrefabs)
                _prefabRegistry[prefab.name] = prefab;
        }

        private void InitialGrid()
        {
            grid = new Dictionary<Vector2Int, BuildingGridCell>();

            for (int x = 0; x < gridSize.x; x++)
            {
                for (int y = 0; y < gridSize.y; y++)
                {
                    float posX = transform.position.x + x * slotSize.x + slotSize.x / 2f;
                    float posY = transform.position.y;
                    float posZ = transform.position.z + y * slotSize.y + slotSize.y / 2f;

                    Vector3 worldPosition = new Vector3
                    (
                        posX,
                        posY,
                        posZ
                    );

                    BuildingGridCell newCell = new BuildingGridCell
                    (
                        new Vector2Int(x, y),
                        worldPosition,
                        false,
                        null
                    );

                    grid[new Vector2Int(x, y)] = newCell;
                    Vector3 offset = new Vector3(0f, 0.03f, 0f);
                    GameObject vis = Instantiate(gridCellImage, worldPosition + offset, Quaternion.Euler(90, 0, 0));
                    vis.transform.localScale = new Vector3(slotSize.x, 1f, slotSize.y);
                    vis.transform.parent = transform;
                }
            }

            Debug.Log($"[BuildableGrid]: Created {grid.Count}");
        }

        public BuildingGridCell GetCell(Vector3 position)
        {
            Vector2Int cellPositon = WorldToGrid(position);
            if (grid.TryGetValue(cellPositon, out BuildingGridCell cell))
                return cell;

            return null;
        }

        public Vector2Int WorldToGrid(Vector3 position)
        {
            int x = Mathf.FloorToInt((position.x - transform.position.x) / slotSize.x);
            int y = Mathf.FloorToInt((position.z - transform.position.z) / slotSize.y);
            return new Vector2Int(x, y);
        }

        public bool TryGetCell(Vector2Int coords, out BuildingGridCell cell)
        {
            return grid.TryGetValue(coords, out cell);
        }

        private void OnDrawGizmos()
        {
            if (!debug) return;

            foreach (var cell in grid.Values)
            {
                Gizmos.color = cell.IsOccupied ? Color.red : Color.green;
                Vector3 size = new Vector3(0.9f, 0.9f, 0.9f);
                Gizmos.DrawWireCube(cell.worldPosittion, size);
            }
        }
    }
}
