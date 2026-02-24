using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using StoreSimulator.PickableObjects;

public class PickableObject : MonoBehaviour, IInteractable
{
    public string GetDescription()
    {
        throw new System.NotImplementedException();
    }

    public void Interact()
    {
        gameObject.SetActive(false);
    }
}
