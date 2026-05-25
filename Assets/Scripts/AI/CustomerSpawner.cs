using System.Collections.Generic;
using StoreSimulator.ArtificialIntelligence;
using UnityEngine;

public class CustomerSpawner : MonoBehaviour
{
    [SerializeField] private NPCFactory factory;
    [SerializeField] private List<Transform> transforms;
    [SerializeField] private int minimalSpawnCount = 1;
    [SerializeField] private int maxSpawnCount = 10;

    private float timer = 0f;
    private const float SPAWN_DELAY_THRESHOLD = 2f;

    void OnEnable()
    {
        TimeManager.Instance.OnPhaseChanged += HandlePhaseChange;
    }

    void OnDisable()
    {
        TimeManager.Instance.OnPhaseChanged -= HandlePhaseChange;
    }

    private void HandlePhaseChange(DayPhase phase)
    {
        if (phase == DayPhase.Morning)
        {
            int spawnCount = Random.Range(minimalSpawnCount, maxSpawnCount);
            for (int i = 0; i < spawnCount; i++)
            {
                SpawnCustomer();
                // DelayBetweenSpawn();
            }
        }
    }

    // private void DelayBetweenSpawn()
    // {
    //     if (timer <= SPAWN_DELAY_THRESHOLD)
    // }

    public NPCController SpawnCustomer()
    {
        int randomIndex = Random.Range(0, transforms.Count);
        Transform spawnTransform = transforms[randomIndex];

        return factory.Spawn(spawnTransform.position);
    }
}
