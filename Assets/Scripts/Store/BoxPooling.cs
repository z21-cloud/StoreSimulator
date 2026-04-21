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

    public IDeliverable GetBoxStorage() => pool.Get();
    public void ReturnBoxStorage(IDeliverable box) 
    { 
        if(box is BoxStorage concreteBox)
        {
            pool.Release(concreteBox);
        }
        else
        {
            Debug.LogError($"[BoxPooling]: Trying to return object that is not BoxStorage");
        }
    } 
}
