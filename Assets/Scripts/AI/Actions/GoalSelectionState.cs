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
        Debug.Log($"[AI - {_ctx.name}] Доступно целей: {goals.Count}");

        IGoal best = null;
        float bestPriority = -1f;

        foreach (var goal in goals)
        {
            bool canPursue = goal.CanPursue(_ctx);
            float priority = canPursue ? goal.GetPriority(_ctx) : -1f;

            Debug.Log($"[AI - {_ctx.name}] Goal '{goal.Name}': CanPursue={goal.CanPursue(_ctx)}, Priority={priority}");

            if (!canPursue) continue;

            if (priority > bestPriority)
            {
                bestPriority = priority;
                best = goal;
            }
        }

        if (best == null)
        {
            Debug.LogWarning($"[AI - {_ctx.name}] НЕТ ДОСТУПНЫХ ЦЕЛЕЙ!");
            return;
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
