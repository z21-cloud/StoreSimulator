public class NPCStateMachine
{
    private INPCState _current;

    public void SetState(INPCState newState)
    {
        _current?.Exit();
        _current = newState;
        _current.Enter();
    }

    public void Tick() => _current?.Tick();
}
