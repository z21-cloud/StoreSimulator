using StoreSimulator.ArtificialIntelligence;

public class ShopAction : IUtilityAction
{
    public bool CanBeInterrupted => false;

    public float Score(NPCWorldState state)
    {
        return state.Hunger * state.Hunger;
    }

    public void Execute(NPCController ctx)
    {
        ctx.StateMachine.SetState(ctx.PickStoreState);
    }
}
