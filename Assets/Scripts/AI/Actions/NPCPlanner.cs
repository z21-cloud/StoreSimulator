using System.Collections.Generic;
using UnityEngine;

public class NPCPlanner
{
    public List<INPCAction> CreatePlan(MonoBehaviour npc, IGoal goal)
    {
        if(!goal.CanPursue(npc)) return null;

        var plan = goal.CreatePlan(npc);
        return plan.Count > 0 ? plan : null;
    }
}
