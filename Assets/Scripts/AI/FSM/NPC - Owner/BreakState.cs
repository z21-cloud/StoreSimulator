using StoreSimulator.ArtificialIntelligence;
using UnityEngine;

public class BreakState : INPCState
{
    private readonly NPCOwner _ctx;

    public BreakState(NPCOwner ctx) => _ctx = ctx;

    private const float CHECK_STORAGES_TIMER = 5f;
    private float currentTimer = 0f;
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
        if(_ctx.CurrentCashStorage.IsOccupied) _ctx.StateMachine.SetState(_ctx.CheckCashBoxState);

        if(!_ctx.Movement.HasReached) return;

        if(!CheckStore()) return;

        foreach(var storage in _ctx.Shelves)
        {
            if(!storage.CanTakeItem())
            {
                _ctx.StateMachine.SetState(_ctx.StorageState);
            }
        }
    }

    private bool CheckStore()
    {
        currentTimer += Time.deltaTime;
        if(currentTimer >= CHECK_STORAGES_TIMER)
        {
            currentTimer = 0f;
            return true;
        }

        return false;
    }
}
