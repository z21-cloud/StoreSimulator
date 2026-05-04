using System.Collections.Generic;

public class NPCActionQueue
{
    private readonly Queue<INPCAction> _actions = new();

    // Если одно действие
    public void Enqueue(INPCAction action) => _actions.Enqueue(action);
    
    // Построить очередь из плана
    public void EnqueueRange(IEnumerable<INPCAction> actions)
    {
        foreach(var action in actions) _actions.Enqueue(action);
    }

    public INPCAction Dequeue() => _actions.Count > 0 ? _actions.Dequeue() : null;
    public void Clear() => _actions.Clear();
    public bool HasActions => _actions.Count > 0;
    public int Count => _actions.Count;
}
