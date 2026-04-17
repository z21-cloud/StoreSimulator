using StoreSimulator.ArtificialIntelligence;

public class CheckCashBoxState : INPCState
{
    private readonly NPCOwner _ctx;

    public CheckCashBoxState(NPCOwner ctx) => _ctx = ctx;

    public void Enter()
    {
        // if (_ctx.CurrentCashStorage.IsOccupied)
        // {
        //     _ctx.Movement.SetDestination(_ctx.CurrentCashStorage.CashierPoint);
        // }
        // else
        // {
        //     _ctx.StateMachine.SetState(_ctx.StorageState);
        //     return;
        // }
    }

    public void Exit()
    {

    }

    public void Tick()
    {
        if (_ctx.Movement.HasReached)
        {
            if (_ctx.CurrentCashStorage.IsOccupied)
            {
                _ctx.Movement.SetDestination(_ctx.CurrentCashStorage.CashierPoint);
            }
            else
            {
                _ctx.StateMachine.SetState(_ctx.StorageState);
            }
        }
    }
}
