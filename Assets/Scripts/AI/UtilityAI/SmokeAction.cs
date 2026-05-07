using StoreSimulator.ArtificialIntelligence;
using UnityEngine;

public class SmokeAction : IUtilityAction
{
    private const float THRESHOLD = 1f;
    public bool CanBeInterrupted => true;

    public float Score(NPCWorldState state)
    {
        return Mathf.Max(0f, THRESHOLD - state.Hunger);
    }

    public void Execute(NPCController ctx)
    {
        ctx.StateMachine.SetState(ctx.SmokingState);
    }
}
