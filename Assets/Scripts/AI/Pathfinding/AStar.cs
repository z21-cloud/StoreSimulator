using System.Collections.Generic;
using UnityEngine;

public class AStar : MonoBehaviour
{
    [SerializeField] PathfindingGrid grid;
    
    private PriorityQueue<PathNode> queue;

    // directions
    private Vector2Int[] directions =
    {
        new Vector2Int(1, 0),
        new Vector2Int(0, -1),
        new Vector2Int(-1, 0),
        new Vector2Int(0, 1),
        new Vector2Int(1, 1),
        new Vector2Int(1, -1),
        new Vector2Int(-1, -1),
        new Vector2Int(-1, 1),
    };

    // A*
    public List<Vector3> FindPath(Vector3 startPosition, Vector3 goalPosition)
    {
        if (startPosition == Vector3.zero || goalPosition == Vector3.zero)
        {
            Debug.LogWarning($"[Pathfinding]: start or goal position is null");
            return null;
        }

        if (startPosition == goalPosition)
        {
            return new List<Vector3> { startPosition };
        }

        // resets grid every pathfinding
        grid.ResetNodes();
        queue = new PriorityQueue<PathNode>();

        PathNode startNode = grid.WorldToGrid(startPosition);
        PathNode goalNode = grid.WorldToGrid(goalPosition);

        if(!goalNode.isWalkable)
        {
            List<PathNode> goalNeighbours = FindNeighbours(goalNode);
            foreach(var goalNeighbour in goalNeighbours)
            {
                if(goalNeighbour.isWalkable) 
                {
                    goalNode = goalNeighbour;
                    break;
                } 
            }
        }

        startNode.gCost = 0f;
        startNode.hCost = grid.GetHeuristics(startNode, goalNode);
        startNode.parent = null;

        queue.Enqueue(startNode, startNode.fCost);

        while (queue.Count > 0)
        {
            PathNode currentNode = queue.Dequeue();

            if (currentNode == goalNode) return RetracePath(goalNode);

            List<PathNode> neighbourds = FindNeighbours(currentNode);

            foreach (var neighbour in neighbourds)
            {
                if (!neighbour.isWalkable) continue;

                float movementCost = grid.GetMovementCost(currentNode, neighbour);
                float tempGCost = currentNode.gCost + movementCost;

                if (tempGCost < neighbour.gCost)
                {
                    neighbour.gCost = tempGCost;
                    neighbour.hCost = grid.GetHeuristics(neighbour, goalNode); 
                    neighbour.parent = currentNode;
                    
                    queue.Enqueue(neighbour, neighbour.fCost);
                }
            }
        }

        Debug.LogError($"Can't find path");
        return null;
    }

    public List<Vector3> RetracePath(PathNode goalNode)
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

    private List<PathNode> FindNeighbours(PathNode node)
    {
        List<PathNode> neighbourds = new List<PathNode>();
        foreach (Vector2Int direction in directions)
        {
            Vector2Int checkCoords = node.gridIndex + direction;
            if (grid.TryGetNode(checkCoords, out var neighbour)) neighbourds.Add(neighbour);
        }

        return neighbourds;
    }
}
