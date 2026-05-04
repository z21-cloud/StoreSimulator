using UnityEngine;

public class PlanExecutionState : INPCState
{
    private readonly MonoBehaviour _ctx;
    private readonly IGoal _goal;
    private INPCAction _currentAction;

    public PlanExecutionState(MonoBehaviour ctx, IGoal goal)
    {
        _ctx = ctx;
        _goal = goal;
    }

    public void Enter() => StartNextAction();

    public void Tick()
    {
        if (_goal.IsSatisfied(_ctx))
        {
            var log = _ctx.GetComponent<ILogProvider>();
            log.Log($"Цель {_goal.Name} удовлетворена досрочно");

            var actionQueue = _ctx.GetComponent<IActionProvider>();
            actionQueue.ActionQueue.Clear();

            var stateMachine = _ctx.GetComponent<IStateMachineProvider>();
            stateMachine.StateMachine.SetState(new GoalSelectionState(_ctx));
            return;
        }

        if (_currentAction == null) return;

        bool done = _currentAction.OnUpdate(_ctx);

        if (done)
        {
            _currentAction.OnComplete(_ctx);
            _currentAction = null;
            StartNextAction();
        }
    }

    private void StartNextAction()
    {
        var actionQueue = _ctx.GetComponent<IActionProvider>();
        if (!actionQueue.ActionQueue.HasActions)
        {
            var stateMachine = _ctx.GetComponent<IStateMachineProvider>();
            stateMachine.StateMachine.SetState(new GoalSelectionState(_ctx));
            return;
        }

        _currentAction = actionQueue.ActionQueue.Dequeue();

        var log = _ctx.GetComponent<ILogProvider>();
        if (!_currentAction.CanExecute(_ctx))
        {
            log.Log($"Действие {_currentAction.Name} невозможно! Перепланирование...");

            actionQueue.ActionQueue.Clear();

            var stateMachine = _ctx.GetComponent<IStateMachineProvider>();
            stateMachine.StateMachine.SetState(new GoalSelectionState(_ctx));
            return;
        }

        log.Log($"-> {_currentAction.Name}");
        _currentAction.OnStart(_ctx);
    }

    public void Exit()
    {
        _currentAction?.OnCancel(_ctx);
        _currentAction = null;
    }
}
