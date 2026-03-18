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
    private Dictionary<string, GameObject> _prefabRegistry;
    private Dictionary<Vector2Int, GridCell> grid;

    void Awake()
    {
        InitialGrid();
        invisiblePlane.transform.localScale = new Vector3(gridSize.x, 1f, gridSize.y);

        invisiblePlane.transform.position = new Vector3
        (
            transform.position.x + gridSize.x / 2f,
            transform.position.y,
            transform.position.z + gridSize.y / 2f
        );

        _prefabRegistry = new Dictionary<string, GameObject>();
        foreach (var prefab in buildablePrefabs)
            _prefabRegistry[prefab.name] = prefab;

        Load();
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

    private Vector2Int WorldToGrid(Vector3 position)
    {
        int x = Mathf.FloorToInt((position.x - transform.position.x) / slotSize.x);
        int y = Mathf.FloorToInt((position.z - transform.position.z) / slotSize.y);
        return new Vector2Int(x, y);
    }

    public void Save()
    {
        GridSaveData saveData = new GridSaveData();

        foreach (var kvp in grid)
        {
            if (!kvp.Value.IsOccupied) continue;

            saveData.cells.Add(new CellSaveData
            {
                x = kvp.Key.x,
                y = kvp.Key.y,
                buildableID = ((MonoBehaviour)kvp.Value.buildable).gameObject.name.Replace("(Clone)", "").Trim()
            });
        }

        string json = JsonUtility.ToJson(saveData);
        PlayerPrefs.SetString("GridSave", json);
        PlayerPrefs.Save();
        Debug.Log($"Grid Saved: {saveData.cells.Count} objects");
    }

    public void Load()
    {
        if (!PlayerPrefs.HasKey("GridSave")) return;

        string json = PlayerPrefs.GetString("GridSave");
        GridSaveData saveData = JsonUtility.FromJson<GridSaveData>(json);

        foreach (var cellData in saveData.cells)
        {
            if (!_prefabRegistry.TryGetValue(cellData.buildableID, out var prefab)) continue;

            Vector2Int coord = new Vector2Int(cellData.x, cellData.y);
            if (!grid.TryGetValue(coord, out GridCell cell)) continue;

            GameObject obj = Instantiate(prefab, cell.worldPosittion, Quaternion.identity);
            if (obj.TryGetComponent<IBuildable>(out var buildable))
            {
                cell.buildable = buildable;
                cell.IsOccupied = true;
            }
        }
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
