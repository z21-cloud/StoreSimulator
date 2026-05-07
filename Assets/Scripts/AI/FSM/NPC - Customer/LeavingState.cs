using StoreSimulator.ArtificialIntelligence;
using StoreSimulator.InteractableObjects;
using UnityEngine;

public class LeavingState : INPCState
{
    private readonly NPCController _ctx;

    public LeavingState(NPCController ctx) => _ctx = ctx;

    public void Enter()
    {
        if (_ctx.BoughtItems.Count > 0)
        {
            float totalSpent = _ctx.GetTotalCost(_ctx.BoughtItems);
            _ctx.RecordVisit(totalSpent);
        }
    }

    public void Exit()
    {
        _ctx.Psycho.ResetReaction();
        _ctx.CurrentShelf = null;
        _ctx.CurrentCashStorage = null;
        _ctx.BoughtItems.Clear();
    }

    public void Tick()
    {
        if (!_ctx.Movement.HasReached) return;
        
        while (_ctx.BoughtItems.Count != 0)
        {
            IStoreable storeable = _ctx.BoughtItems[0];
            _ctx.Psycho.IncreaseParameters(storeable.Data.FoodRestore, storeable.Data.ThirstRestore);

            if (storeable is StoreableItem storeableItem)
            {
                storeableItem.ReturnToPool();

                // GameObject storeableItemGO = storeableItem.Data.Prefab;
                // StoreablePooling.Instance.ReturnStoreable(storeable, storeableItemGO.GetComponent<StoreableItem>());
            }
            else
            {
                ((MonoBehaviour)storeable)?.gameObject.SetActive(false);
            }

            // ((MonoBehaviour)storeable).gameObject.SetActive(false);
            _ctx.BoughtItems.RemoveAt(0);
        }

        _ctx.UtilityPlanner.OnCurrentActionDone();
    }
}
