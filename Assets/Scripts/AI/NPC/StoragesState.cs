using StoreSimulator.ArtificialIntelligence;
using StoreSimulator.InteractableObjects;
using UnityEngine;

public class StoragesState : INPCState
{
    private readonly NPCOwner _ctx;

    public StoragesState(NPCOwner ctx) => _ctx = ctx;

    private IStorage _currentStorage;
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
                StoreableItem found = null;

                foreach (var item in _ctx.StoreableItems)
                {
                    if (_currentStorage.CanPlaceItem(item))
                    {
                        found = item;
                        break;
                    }
                }

                if (found == null)
                {
                    _ctx.StateMachine.SetState(_ctx.StorageState);
                    return;
                }

                _ctx.Storeable = found;
                _ctx.StateMachine.SetState(_ctx.OrderItemsState);
                return;
            }
            else
            {
                _ctx.StateMachine.SetState(_ctx.CheckCashBoxState);

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
