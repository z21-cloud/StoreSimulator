using System.Collections.Generic;
using UnityEngine;

public class PathfindingGrid : MonoBehaviour
{
    [SerializeField] private Vector2Int gridSize = new Vector2Int(10, 10);
    [SerializeField] private Vector2Int nodeSize = new Vector2Int(1, 1);
    [SerializeField] private LayerMask walkableMask;

    // index : node
    private Dictionary<Vector2Int, PathNode> grid;

    // re write priority queueu
    private Queue<PathNode> queue;
    // unique checked nodes
    private HashSet<PathNode> hashset; 

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
        grid = new Dictionary<Vector2Int, PathNode>();
        hashset = new HashSet<PathNode>();
        queue = new Queue<PathNode>();
    }

    private bool PhysicsCheck(Vector3 position)
    {
        Collider[] cols = Physics.OverlapSphere(position, nodeSize.x, walkableMask);
        if(cols.Length > 0) return false;
        return true;
    }
}
