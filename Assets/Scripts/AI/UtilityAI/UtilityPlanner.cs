using System;
using System.Collections.Generic;
using StoreSimulator.ArtificialIntelligence;
using UnityEngine;

public class UtilityPlanner
{
    private readonly NPCController _ctx;
    private readonly List<IUtilityAction> _actions;

    private IUtilityAction _currentAction;
    private bool _actionRunning = false;
    private float _timer = 0f;

    private const float TICK_INTERVAL = 1f;

    public UtilityPlanner(NPCController ctx)
    {
        _ctx = ctx;
        _actions = new List<IUtilityAction>
        {
            new ShopAction(),
            new SmokeAction()
        };
    }

    public void Tick()
    {
        if(_actionRunning) return;

        _timer -= Time.deltaTime;
        if(_timer > 0f) return;
        _timer = TICK_INTERVAL;

        PickBestAction();
    }

    public void OnCurrentActionDone()
    {
        _actionRunning = false;
        _currentAction = null;
        _timer = 0f;
    }

    private void PickBestAction()
    {
        NPCWorldState state = BuildWorldState();

        IUtilityAction best = null;
        float bestScore = -1f;

        foreach(var action in _actions)
        {
            float score = action.Score(state);
            if(score > bestScore)
            {
                bestScore = score;
                best = action;
            }
        }

        if(best == null || best == _currentAction) return;

        _currentAction = best;
        _actionRunning = true;
        best.Execute(_ctx);
    }

    private NPCWorldState BuildWorldState()
    {
        return new NPCWorldState
        {
            Money = _ctx.Wallet.Balance,
            Hunger = GetHunger(),
            Thirst = GetThirst()
        };
    }

    private float GetHunger()
    {
        int hunger = (int)_ctx.Psycho.HungerState;
        return (hunger * 25f / 100f) + 0.25f;
    }

    private float GetThirst()
    {
        int thirst = (int)_ctx.Psycho.ThirstState;
        return (thirst * 25f / 100f) + 0.25f;
    }
}
