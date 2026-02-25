using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using StoreSimulator.PickableObjects;

public class PickableObject : MonoBehaviour, IPickable
{
    public string GetDescription()
    {
        throw new System.NotImplementedException();
    }

    public void Pick()
    {
        gameObject.SetActive(false);
    }
}
