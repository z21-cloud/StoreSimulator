using StoreSimulator.ArtificialIntelligence;
using UnityEngine;

public class SmokeAction : IUtilityAction
{
    public string Name => "ShopAction";
    private const float THRESHOLD_SCORE = 0.3f;
    public bool CanBeInterrupted => false;

    public float Score(NPCWorldState state)
    {
        return THRESHOLD_SCORE;
    }

    public void Execute(NPCController ctx)
    {
        Debug.Log($"[{ctx.gameObject.name}] Smoking: wanna smoke");
        ctx.StateMachine.SetState(ctx.SmokingState);
    }
}
