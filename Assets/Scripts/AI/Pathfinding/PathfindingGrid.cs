using System.Collections.Generic;
using Mono.Cecil.Cil;
using UnityEngine;

namespace StoreSimulator.Pathfinding
{
    public class PathfindingGrid : MonoBehaviour
    {
        [SerializeField] private Vector2Int gridSize = new Vector2Int(10, 10);
        [SerializeField] private Vector2 nodeSize = new Vector2(1, 1);
        [SerializeField] private LayerMask unwalkableMask;
        [SerializeField] private float nodeCost = 1f;

        // index : node
        private Dictionary<Vector2Int, PathNode> grid;

        private Vector3 gridOrigin; // minimal grid position

        void Awake()
        {
            gridOrigin = transform.position;

            CreateGrid();
        }

        // grid creation
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

                    // world position
                    Vector3 worldPosition = new Vector3
                    (
                        posX,
                        posY,
                        posZ
                    );

                    Vector2Int coords = new Vector2Int(x, y);

                    // node creation
                    PathNode node = new PathNode
                    (
                        worldPosition,
                        coords,
                        true,
                        nodeCost,
                        null
                    );

                    // walkable or not
                    if (PhysicsCheck(worldPosition))
                    {
                        node.isWalkable = false;
                        node.hCost = float.MaxValue;
                    }

                    // add to dictionary
                    grid[coords] = node;
                }
            }

            Debug.Log($"[PathfindingGird]: Created; {grid.Count}");
        }

        // Creates sphere at every node position to check if it's walkable or not
        private bool PhysicsCheck(Vector3 position)
        {
            Collider[] cols = Physics.OverlapSphere(position, nodeSize.x / 2, unwalkableMask);
            return cols.Length > 0;
        }

        public PathNode WorldToGrid(Vector3 position)
        {
            int posX = Mathf.FloorToInt((position.x - gridOrigin.x) / nodeSize.x);
            int posZ = Mathf.FloorToInt((position.z - gridOrigin.z) / nodeSize.y);
            Vector2Int cords = new Vector2Int(posX, posZ);
            if (grid.TryGetValue(cords, out var node)) return node;
            return null;
        }

        public float GetMovementCost(PathNode from, PathNode to)
        {
            int dx = Mathf.Abs(from.gridIndex.x - to.gridIndex.x);
            int dy = Mathf.Abs(from.gridIndex.y - to.gridIndex.y);

            bool isDiagonal = dx == 1 && dy == 1;

            if (isDiagonal) return nodeCost * 1.4f;
            else return nodeCost;
        }

        public float GetHeuristics(PathNode from, PathNode to)
        {
            int dx = Mathf.Abs(from.gridIndex.x - to.gridIndex.x);
            int dy = Mathf.Abs(from.gridIndex.y - to.gridIndex.y);

            float diagonal = 1.4f;
            float straight = nodeCost;

            return straight * (dx + dy) + (diagonal - 2 * straight) * Mathf.Min(dx, dy);
        }

        public void ResetNodes()
        {
            foreach (var node in grid.Values)
            {
                node.gCost = float.MaxValue;
                node.hCost = 0f;
                node.parent = null;
            }
        }

        public bool TryGetNode(Vector2Int coords, out PathNode node)
        {
            if (grid.ContainsKey(coords))
            {
                node = grid[coords];
                return true;
            }
            else
            {
                node = null;
                return false;
            }
        }

        public void UpdateGrid()
        {
            foreach (var node in grid.Values)
            {
                node.isWalkable = !PhysicsCheck(node.worldPosition);
            }
        }

        private void OnDrawGizmos()
        {
            if (grid == null) return;

            foreach (var node in grid.Values)
            {
                Gizmos.color = node.isWalkable ? Color.green : Color.red;

                Vector3 size = new Vector3(nodeSize.x, 0.1f, nodeSize.y);
                Gizmos.DrawWireCube(node.worldPosition, size * 0.9f);
            }
        }
    }
}
