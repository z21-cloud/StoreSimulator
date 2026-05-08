using System.Collections.Generic;
using StoreSimulator.ArtificialIntelligence;
using StoreSimulator.InteractableObjects;
using StoreSimulator.StoreableItems;
using UnityEngine;

public class GoToMachine : INPCState
{
    private readonly VendingNPCController _ctx;
    
    public GoToMachine(VendingNPCController ctx) => _ctx = ctx;

    public void Enter()
    {
        if(_ctx.CurrentStore == null)
        {
            Leaving(); return;
        }

        _ctx.Movement.SetDestination(_ctx.CurrentStore.StoreEnterPoint.position);
    }

    public void Tick()
    {
        if(!_ctx.Movement.HasReached) return;

        _ctx.StateMachine.SetState(_ctx.BuyVendingMachine);
    }

    private void Leaving()
    {
        _ctx.Movement.SetDestination(_ctx.CurrentStore.StoreLeavePoint.position);
        _ctx.StateMachine.SetState(_ctx.LeavingVendingMachine);
    }

    public void Exit()
    {
        
    }
}
