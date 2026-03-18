using StoreSimulator.StoreableItems;
using UnityEngine;

public class RecycleBin : MonoBehaviour
{
    [SerializeField] private BoxPooling boxPooling;

    void OnTriggerEnter(Collider other)
    {
        if(other.TryGetComponent<BoxStorage>(out var boxStorage) && !boxStorage.CanTakeItem())
        {
            boxPooling.ReturnBoxStorage(boxStorage);
        }
    }
}
