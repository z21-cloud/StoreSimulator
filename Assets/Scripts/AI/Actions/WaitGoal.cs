using System.Collections.Generic;
using UnityEngine;

public class WaitGoal : IGoal
{
    public string Name => "Wait";

    public float GetPriority(MonoBehaviour npc) => 1f; // самый низкий приоритет
    public bool IsSatisfied(MonoBehaviour npc) => false;
    public bool CanPursue(MonoBehaviour npc) => true;  // ← ВСЕГДА true

    public List<INPCAction> CreatePlan(MonoBehaviour npc)
    {
        return new List<INPCAction>
        {
            new WaitAction(2f) // просто стоит 2 секунды и перепроверяет цели
        };
    }
}
