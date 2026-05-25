using StoreSimulator.ArtificialIntelligence;
using UnityEngine;

public class NPCPooling : MonoBehaviour
{
    [SerializeField] private NPCController prefab;
    [SerializeField] private Transform parent;
    [SerializeField] private int initialSize = 25;
    
    private ObjectPooling<NPCController> pool;

    private void Awake()
    {
        pool = new ObjectPooling<NPCController>(prefab, initialSize, parent);
    }

    public NPCController GetNpc() => pool.Get();

    public void ReturnNpc(NPCController npc)
    {
        pool.Release(npc);
    }
}
