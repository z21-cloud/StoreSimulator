using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ShellfSlot : MonoBehaviour
{
    public Transform SpawnPoint { get; private set; }
    public bool IsOccupied { get; private set; }

    private void Start()
    {
        SpawnPoint = transform;
        IsOccupied = false;
    }

    public void Occupy() => IsOccupied = true;

    public void Release() => IsOccupied = false;
}
