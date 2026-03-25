using StoreSimulator.PlayerInput;
using UnityEngine;

public class TryFindPlayer : MonoBehaviour
{
    public bool FindPlayer { get; private set; }

    void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent<IInputProvider>(out _))
        {
            FindPlayer = true;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.TryGetComponent<IInputProvider>(out _))
        {
            FindPlayer = false;
        }
    }
}
