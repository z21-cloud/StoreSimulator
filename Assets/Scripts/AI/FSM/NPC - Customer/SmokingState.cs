using StoreSimulator.ArtificialIntelligence;
using UnityEngine;

public class SmokingState : INPCState
{
    private readonly NPCController _ctx;

    public SmokingState(NPCController ctx) => _ctx = ctx;

    public void Enter()
    {
        Vector3 smokePosition = _ctx.SmokingArea.GetRandomPointInZone();
        _ctx.Movement.SetDestination(smokePosition);
    }

    public void Exit()
    {

    }

    public void Tick()
    {
        if (!_ctx.Movement.HasReached) return;

        if (_ctx.NPCNeeds.Count != 0)
        {
            Debug.Log($"[AI - {_ctx.gameObject.name} - SmokingState]: I have needs. Going to store...");
            _ctx.StateMachine.SetState(_ctx.IdleState);
            return;
        }

        if (_ctx.Psycho.WantBuyProducts)
        {
            Debug.Log($"[AI - {_ctx.gameObject.name} - SmokingState]: I want to buy products. Going to store...");
            _ctx.StateMachine.SetState(_ctx.IdleState);
            return;
        }

        Debug.Log($"[AI - {_ctx.gameObject.name} - SmokingState]: Smoking...");
    }
}
