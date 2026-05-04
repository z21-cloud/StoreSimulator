using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class SmokeGoal : IGoal
{
    public string Name => "Smoke";

    public float GetPriority(MonoBehaviour npc)
    {
        var psychology = npc.GetComponent<NPCPsycho>();
        bool hasNeeds = psychology.GetPriorityNeeds().Count > 0;
        return hasNeeds ? 5f : 40f;
    }

    public bool IsSatisfied(MonoBehaviour npc)
    {
        return false;
    }

    public bool CanPursue(MonoBehaviour npc)
    {
        return true;
    }

    public List<INPCAction> CreatePlan(MonoBehaviour npc)
    {
        var plan = new List<INPCAction>();

        Vector3 smokePos = Vector3.zero;
        if(npc.TryGetComponent(out CustomerController customerController) && customerController.SmokingArea != null)
        {
            smokePos = customerController.SmokingArea.GetRandomPointInZone();
        }
        /*else if(npc is IStoreOwner storeOwner)
        {
            smokePos = storeOwner.SmokingArea.GetRandomPointInZone();
        }*/

        plan.Add(new MoveTo(smokePos));
        // plan.Add(new WaitAction(5f));

        return plan;
    }
}
