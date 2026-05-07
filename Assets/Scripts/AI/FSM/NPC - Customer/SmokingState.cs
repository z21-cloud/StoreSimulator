using StoreSimulator.ArtificialIntelligence;
using UnityEngine;

public class SmokingState : INPCState
{
    private readonly NPCController _ctx;

    private const float SMOKE_TIME = 5f;
    private float _timer;

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

        _timer += Time.deltaTime;
        if (_timer > SMOKE_TIME)
        {
            _timer = 0f;
            _ctx.OnActionCompleted();
            Debug.Log($"[AI - {_ctx.gameObject.name} - SmokingState]: Finished smoking...");
        }

        Debug.Log($"[AI - {_ctx.gameObject.name} - SmokingState]: Smoking...");
    }
}
