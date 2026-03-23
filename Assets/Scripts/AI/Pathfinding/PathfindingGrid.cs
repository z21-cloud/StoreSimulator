using System.Collections.Generic;
using UnityEngine;

public class PathfindingGrid : MonoBehaviour
{
    [SerializeField] private Vector2Int gridSize = new Vector2Int(10, 10);
    [SerializeField] private Vector2 nodeSize = new Vector2(1, 1);
    [SerializeField] private LayerMask unwalkableMask;
    [SerializeField] private bool debug;

    // index : node
    private Dictionary<Vector2Int, PathNode> grid;

    // re write priority queueu
    private Queue<PathNode> queue;
    // unique checked nodes
    private HashSet<PathNode> hashset;
    private Vector3 gridOrigin; // minimal grid position

    private Vector2Int[] directions =
    {
        new Vector2Int(1, 1),
        new Vector2Int(1, -1),
        new Vector2Int(1, 0),
        new Vector2Int(0, -1),
        new Vector2Int(-1, -1),
        new Vector2Int(-1, 0),
        new Vector2Int(-1, 1),
        new Vector2Int(0, 1),
    };

    void Awake()
    {
        gridOrigin = transform.position;

        CreateGrid();
    }

    private void CreateGrid()
    {
        grid = new Dictionary<Vector2Int, PathNode>();

        for (int x = 0; x < gridSize.x; x++)
        {
            for (int y = 0; y < gridSize.y; y++)
            {
                float posX = gridOrigin.x + x * nodeSize.x + nodeSize.x / 2f;
                float posZ = gridOrigin.z + y * nodeSize.y + nodeSize.y / 2f;
                float posY = gridOrigin.y + 1f; // 1f = offset

                Vector3 worldPosition = new Vector3
                (
                    posX,
                    posY,
                    posZ
                );

                Vector2Int coords = new Vector2Int(x, y);
                PathNode node = new PathNode
                (
                    worldPosition,
                    coords,
                    true,
                    1f,
                    null
                );

                if (PhysicsCheck(worldPosition)) node.isWalkable = false;
                grid[coords] = node;
            }
        }
    }

    public List<Vector3> FindPath(Vector3 startPosition, Vector3 goalPosition)
    {
        ResetNodes();

        PathNode startNode = WorldToGrid(startPosition);
        PathNode goalNode = WorldToGrid(goalPosition);

        hashset = new HashSet<PathNode>();
        queue = new Queue<PathNode>();

        PathNode currentNode = null;
        queue.Enqueue(startNode);
        hashset.Add(startNode);

        while (queue.Count > 0)
        {
            currentNode = queue.Dequeue();

            if (currentNode == goalNode) break;

            List<PathNode> neighbourds = FindNeighbours(currentNode);

            foreach (var neighbour in neighbourds)
            {
                if (hashset.Contains(neighbour)) continue;
                if (!neighbour.isWalkable) continue;

                neighbour.parent = currentNode;
                queue.Enqueue(neighbour);
                hashset.Add(neighbour);
            }
        }

        return RetracePath(goalNode, startNode);
    }

    private List<PathNode> FindNeighbours(PathNode node)
    {
        List<PathNode> neighbourds = new List<PathNode>();
        foreach (Vector2Int direction in directions)
        {
            Vector2Int checkCoords = node.gridIndex + direction;
            if (grid.TryGetValue(checkCoords, out var neighbour)) neighbourds.Add(neighbour);
        }

        return neighbourds;
    }

    private bool PhysicsCheck(Vector3 position)
    {
        Collider[] cols = Physics.OverlapSphere(position, nodeSize.x, unwalkableMask);
        return cols.Length > 0;
    }

    private PathNode WorldToGrid(Vector3 position)
    {
        int posX = Mathf.FloorToInt((position.x - gridOrigin.x) / nodeSize.x);
        int posZ = Mathf.FloorToInt((position.z - gridOrigin.z) / nodeSize.y);
        Vector2Int cords = new Vector2Int(posX, posZ);
        if (grid.TryGetValue(cords, out var node)) return node;
        return null;
    }

    private void ResetNodes()
    {
        foreach (var node in grid.Values)
        {
            node.nodeCost = 0f;
            node.parent = null;
        }
    }

    public List<Vector3> RetracePath(PathNode goalNode, PathNode startNode)
    {
        List<PathNode> temp = new List<PathNode>();
        PathNode current = goalNode;

        while (current != null)
        {
            temp.Add(current);
            current = current.parent;
        }

        temp.Reverse();

        List<Vector3> result = new List<Vector3>();
        foreach (var node in temp)
        {
            result.Add(node.worldPosition);
        }

        return result;
    }

    private void OnDrawGizmos()
    {
        if (grid == null) return; // грида ещё не создана

        foreach (var node in grid.Values)
        {
            // цвет ВНУТРИ цикла — вот где чаще всего ошибка
            Gizmos.color = node.isWalkable ? Color.green : Color.red;

            Vector3 size = new Vector3(nodeSize.x, 0.1f, nodeSize.y);
            Gizmos.DrawWireCube(node.worldPosition, size * 0.9f); // 0.9f чтобы зазор между нодами
        }
    }
}
