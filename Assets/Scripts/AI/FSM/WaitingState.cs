using StoreSimulator.ArtificialIntelligence;
using UnityEngine;

public class WaitingState : INPCState
{
    private readonly NPCController _ctx;
    private INPCState _returnState;
    private float _timer;
    public WaitingState(NPCController ctx) => _ctx = ctx;
    public void SetReturn(INPCState returnTo) => _returnState = returnTo;

    public void Enter()
    {
        _timer = _ctx.WaitTime;
    }

    public void Exit()
    {
        
    }

    public void Tick()
    {
        _timer -= Time.deltaTime;
        if(_timer <= 0f)
        {
            _ctx.StateMachine.SetState(_returnState);
        }
    }
}
