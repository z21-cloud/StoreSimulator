using StoreSimulator.ArtificialIntelligence;
using UnityEngine;

public class LeavingState : INPCState
{
    private readonly NPCController _ctx;

    public LeavingState(NPCController ctx) => _ctx = ctx;

    public void Enter()
    {
        Debug.Log($"[AI - {_ctx.gameObject.name} - LeavingState]: Leaving store");
    }

    public void Exit()
    {
        while (_ctx.BoughtItems.Count != 0)
        {
            _ctx.Psycho.IncreaseParameters(_ctx.BoughtItems[0].Data.FoodRestore, _ctx.BoughtItems[0].Data.ThirstRestore);
            var storeable = _ctx.BoughtItems[0];
            ((MonoBehaviour)storeable).gameObject.SetActive(false);
            _ctx.BoughtItems.RemoveAt(0);
        }

        _ctx.Psycho.ResetReaction();

        _ctx.CurrentShelf = null;
        _ctx.CurrentCashStorage = null;
        _ctx.BoughtItems.Clear();
    }

    public void Tick()
    {
        if (_ctx.Movement.HasReached)
        {
            Debug.Log($"[AI - {_ctx.gameObject.name} - LeavingState]: Gone from store. Wanna smoke");
            _ctx.StateMachine.SetState(_ctx.SmokingState);
        }
    }
}
