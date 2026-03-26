using UnityEngine;

public enum NPCState
{
    Idle,
    MovingToStorage,
    TakingFromShelf,
    Buying,
    Leaving,
    Waiting
}
public class NPCStates : MonoBehaviour
{
    public NPCState CurrentState { get; private set; } = NPCState.Idle;

    public void SetState(NPCState newState)
    {
        CurrentState = newState;
    }
}
