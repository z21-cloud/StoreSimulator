using System;
using StoreSimulator.InteractableObjects;
using UnityEngine;

public class NPCController : MonoBehaviour
{
    [SerializeField] private float interactionDistance = 2f;
    [SerializeField] private NPCMover mover;
    [SerializeField] private Transform goal;
    [SerializeField] private Transform storageForItems;

    private IStorage goalStorage;
    private GameObject boughtObj;

    void Update()
    {
        if(goalStorage == null)
        {
            goalStorage = GetStorage();  
            return;  
        }

        Moving();

        if(mover.IsMoving) return;
        BuyItem();
    }

    private void Moving()
    {
        if(!mover.IsMoving) return;
        
        mover.MoveTo(((MonoBehaviour)goalStorage).transform.position, interactionDistance);
    }

    private IStorage GetStorage()
    {
        return StorageRegistry.Instance.GetRandomStorage();
    }

    private void BuyItem()
    {
        if(boughtObj != null) return;

        GameObject go = goalStorage.TakeItem(transform.position);
        if(go.TryGetComponent<IStoreable>(out var storeable))
        {
            boughtObj = storeable.OnPickedFromStore();
            boughtObj.transform.position = storageForItems.position;
            boughtObj.transform.parent = storageForItems;
        }
        //Destroy(go);
        Debug.Log($"[NPCConrtoller]: taken");
    }
}
