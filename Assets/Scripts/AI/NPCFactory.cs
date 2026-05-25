using StoreSimulator.ArtificialIntelligence;
using UnityEngine;

public class NPCFactory : MonoBehaviour
{
    [SerializeField] private NPCController prefab;
    [SerializeField] private NPCPooling pool;
    
    public NPCController Spawn(Vector3 position)
    {
        NPCController nPC = pool.GetNpc();

        nPC.Initialize(position);

        return nPC;
    }
}
