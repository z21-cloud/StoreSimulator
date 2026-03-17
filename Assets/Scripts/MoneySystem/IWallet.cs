using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace StoreSimulator.MoneySystem
{
    public interface IWallet
    {
        public float Balance { get; }
        public void Add(float amount);
        public void Spend(float amount);
        public bool CanAfford(float amount);
    }
}
