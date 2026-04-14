using StoreSimulator.ArtificialIntelligence;
using StoreSimulator.InteractableObjects;
using StoreSimulator.StoreableItems;

public class StoragesState : INPCState
{
    private readonly NPCOwner _ctx;

    public StoragesState(NPCOwner ctx) => _ctx = ctx;

    private IStorage _currentStorage;
    private StoreableItem _currentStoreable;
    private int _currentStorageIndex = 0;

    public void Enter()
    {
        _currentStorageIndex = (_currentStorageIndex + 1) % _ctx.Shelves.Count;
        _currentStorage = _ctx.Shelves[_currentStorageIndex];
        if (_currentStorage != null)
        {
            _ctx.CurrentShelf = _currentStorage;
            _ctx.Movement.SetDestination(_currentStorage.InteractionPoint);
        }
    }

    public void Tick()
    {
        if (_ctx.Movement.HasReached)
        {
            if (!_currentStorage.CanTakeItem())
            {
                do _currentStoreable = _ctx.GetRandomStoreable();

                while (!_currentStorage.CanPlaceItem(_currentStoreable));

                _ctx.Storeable = _currentStoreable;
                _ctx.StateMachine.SetState(_ctx.OrderItemsState);
                return;
            }
            else
            {
                // _currentStorageIndex = (_currentStorageIndex + 1) % _ctx.Shelves.Count;
                // _currentStorage = _ctx.Shelves[_currentStorageIndex];

                // if (_currentStorage != null)
                // {
                //     _ctx.Movement.SetDestination(_currentStorage.InteractionPoint);
                // }
            }
        }
    }

    public void Exit()
    {

    }
}
