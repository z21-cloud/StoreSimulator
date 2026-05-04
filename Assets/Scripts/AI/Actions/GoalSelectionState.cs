using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

public class GoalSelectionState : INPCState
{
    private readonly MonoBehaviour _ctx;
    private float _reevaluateTimer;

    public GoalSelectionState(MonoBehaviour ctx) => _ctx = ctx;

    public void Enter()
    {
        _reevaluateTimer = 0f;
    }

    public void Tick()
    {
        _reevaluateTimer -= Time.deltaTime;
        if (_reevaluateTimer > 0f) return;
        _reevaluateTimer = 1f;

        var goals = GetGoals(_ctx);

        IGoal best = null;
        float bestPriority = -1f;

        foreach (var goal in goals)
        {
            if (!goal.CanPursue(_ctx)) continue;
            float priority = goal.GetPriority(_ctx);
            if (priority > bestPriority)
            {
                bestPriority = priority;
                best = goal;
            }
        }


        if (best != null)
        {
            var log = _ctx.GetComponent<ILogProvider>();
            log.Log($"Выбрана цель: {best.Name} (приоритет: {bestPriority})");

            var planner = _ctx.GetComponent<IPlannerProvider>();
            var plan = planner.Planner.CreatePlan(_ctx, best);

            if (plan != null)
            {
                var actionQueue = _ctx.GetComponent<IActionProvider>();
                actionQueue.ActionQueue.Clear();
                actionQueue.ActionQueue.EnqueueRange(plan);

                var stateMachine = _ctx.GetComponent<IStateMachineProvider>();
                stateMachine.StateMachine.SetState(new PlanExecutionState(_ctx, best));
            }
        }
    }

    public void Exit() { }

    private List<IGoal> GetGoals(MonoBehaviour npc)
    {
        if (npc.TryGetComponent(out CustomerController customerController)) return customerController.GetAvailableGoals();
        return new List<IGoal>();
    }
}
