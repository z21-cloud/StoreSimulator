using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class PlayerWallet : MonoBehaviour, IWallet
{
    [SerializeField] private float balance;
    public float Balance { get; private set; }

    private void Awake()
    {
        Balance = balance;
    }

    public void Add(float amount)
    {
        Balance += amount;
    }

    public bool CanAfford(float amount)
    {
        return (Balance -= amount) >= 0;
    }

    public bool Spend(float amount)
    {
        if (Balance < amount) return false;
        
        Balance -= amount;
        return true;
    }
}
