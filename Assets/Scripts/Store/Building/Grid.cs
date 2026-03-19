using System;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;

public class Grid : MonoBehaviour
{
    [SerializeField] private Vector2Int gridSize;
    [SerializeField] private Vector2 slotSize = new Vector2(1, 1);
    [SerializeField] private GameObject invisiblePlane;
    [SerializeField] private GameObject gridCellImage;
    [SerializeField] private bool debug;
    [SerializeField] private List<GameObject> buildablePrefabs;
    public Vector2 SlotSize => slotSize;
    private Dictionary<string, GameObject> _prefabRegistry;
    private Dictionary<Vector2Int, GridCell> grid;

    void Awake()
    {
        CreateGrid();
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
        grid = new Dictionary<Vector2Int, GridCell>();

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

                GridCell newCell = new GridCell
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
    }

    public GridCell GetCell(Vector3 position)
    {
        Vector2Int cellPositon = WorldToGrid(position);
        if (grid.TryGetValue(cellPositon, out GridCell cell))
            return cell;

        return null;
    }

    public Vector2Int WorldToGrid(Vector3 position)
    {
        int x = Mathf.FloorToInt((position.x - transform.position.x) / slotSize.x);
        int y = Mathf.FloorToInt((position.z - transform.position.z) / slotSize.y);
        return new Vector2Int(x, y);
    }

    public bool TryGetCell(Vector2Int coords, out GridCell cell)
    {
        return grid.TryGetValue(coords, out cell);
    }

    void OnDrawGizmos()
    {
        if (!debug) return;

        Gizmos.color = Color.red;
        foreach (var cell in grid.Values)
        {
            Gizmos.DrawWireCube(cell.worldPosittion, new Vector3(1f, 1f, 1f));
        }
    }
}
