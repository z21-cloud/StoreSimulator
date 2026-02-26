using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using StoreSimulator.InteractableObjects;
using System;

public class PickableObject : MonoBehaviour, IPickable, IInteractable
{
    // for UI or Inventory
    public event Action OnPicked;

    public string GetDescription()
    {
        throw new System.NotImplementedException();
    }

    public void Pick()
    {
        gameObject.SetActive(false);
        
        //OnPicked?.Invoke();
    }
}
