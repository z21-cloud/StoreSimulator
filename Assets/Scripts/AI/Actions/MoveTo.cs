using UnityEngine;

public class MoveTo : INPCAction
{
    private readonly Vector3 _targetPosition;
    private readonly Transform _targetTransform;
    private readonly float _stopDistance;

    public string Name => "MoveTo";

    public MoveTo(Vector3 target, float stopDistance = 2f)
    {
        _targetPosition = target;
        _stopDistance = stopDistance;
    }

    public MoveTo(Transform target, float stopDistance = 2f)
    {
        _targetTransform = target;
        _stopDistance = stopDistance;
    }

    public bool CanExecute(MonoBehaviour  npc)
    {
        return npc.TryGetComponent(out IMovable _);
    }

    public void OnStart(MonoBehaviour  npc)
    {
        var movable = npc.GetComponent<IMovable>();
        Vector3 destination = _targetTransform ? _targetTransform.position : _targetPosition;
        movable.Movement.SetDestination(destination, _stopDistance);
    }

    public bool OnUpdate(MonoBehaviour  npc)
    {
        var movable = npc.GetComponent<IMovable>();

        if (_targetTransform != null)
        {
            Vector3 destination = _targetTransform ? _targetTransform.position : _targetPosition;
            movable.Movement.SetDestination(destination, _stopDistance);
        }

        return movable.Movement.HasReached;
    }

    public void OnCancel(MonoBehaviour  npc) { }
    public void OnComplete(MonoBehaviour  npc) { }
}