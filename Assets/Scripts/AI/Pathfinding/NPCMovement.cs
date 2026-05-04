using System.Collections.Generic;
using StoreSimulator.Pathfinding;
using UnityEngine;

namespace StoreSimulator.ArtificialIntelligence
{
    public class NPCMovement : MonoBehaviour
    {
        [Header("Movement algorithm")]
        [SerializeField] private NPCMover mover;
        [Header("Pathfinding algorithm")]
        [SerializeField] private AStar pathfinding;

        [Header("Parameters")]
        [SerializeField] private float stopDistance = 2f;

        public bool HasReached => _currentPath.Count == 0 && !mover.IsMoving;

        private const float STOP_DISTANCE = 2f; 

        private Queue<Vector3> _currentPath = new Queue<Vector3>();

        public void SetDestination(Vector3 goalPosition, float stopDistance = STOP_DISTANCE)
        {
            var path = pathfinding.FindPath(transform.position, goalPosition);
            _currentPath = new Queue<Vector3>(path);
        }

        public void Tick()
        {
            if (_currentPath.Count == 0) return;

            Vector3 target = _currentPath.Peek();
            mover.MoveTo(target, stopDistance);
            transform.LookAt(target);

            if (!mover.IsMoving) _currentPath.Dequeue();
        }
    }
}
