using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using StoreSimulator.InteractableObjects;
using StoreSimulator.StoreableItems;
using StoreSimulator.Boxes;

public class PCInteractor : MonoBehaviour, IInteractable
{
    [SerializeField] private DeliveryOrder waterOrder;
    [SerializeField] private GameObject boxPrefab;
    [SerializeField] private Transform spawnPoint;

    public void Interact()
    {
        Debug.Log("Interact");
        GameObject go = Instantiate(boxPrefab, spawnPoint.position, Quaternion.identity);
        var boxStorage = go.GetComponent<BoxStorage>();
        boxStorage.Initialize(waterOrder);
    }
    public string GetDescription()
    {
        throw new System.NotImplementedException();
    }
}
