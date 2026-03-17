using StoreSimulator.StoreableItems;
using UnityEngine;

public class BoxPooling : MonoBehaviour
{
    [SerializeField] private BoxStorage prefab;
    [SerializeField] private Transform parent;
    [SerializeField] private int initialSize = 100;

    private ObjectPooling<BoxStorage> pool;

    private void Awake()
    {
        pool = new ObjectPooling<BoxStorage>(prefab, initialSize, parent);
    }

    public BoxStorage GetBoxStorage() => pool.Get();
    public void ReturnBoxStorage(BoxStorage box) 
    { 
        pool.Release(box);
    } 
}
