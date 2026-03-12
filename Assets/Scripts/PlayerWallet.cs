using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class PlayerWallet : MonoBehaviour, IWallet
{
    [SerializeField] private float startBalance = 500f;
    public float Balance {get; private set;}

    void Awake()
    {
        Balance = startBalance;
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
