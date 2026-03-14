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

    void Update()
    {
        Debug.Log($"[PlayerWallet]: Balance {Balance}$");
    }

    public void Add(float amount)
    {
        Balance += amount;
    }

    public bool CanAfford(float amount)
    {
        if((Balance - amount) >= 0) return true;
        return false;
    }

    public void Spend(float amount)
    {
        Debug.Log($"[PlayerWallet]: spended {amount}$");
        Balance -= amount;
        Balance = Mathf.Max(0, Balance);
    }
}
