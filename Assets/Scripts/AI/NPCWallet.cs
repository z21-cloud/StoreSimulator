using StoreSimulator.MoneySystem;
using UnityEngine;

public class NPCWallet : MonoBehaviour, IWallet
{
    [SerializeField] private float startBalance = 500f;
        public float Balance { get; private set; }

        private void Awake()
        {
            Balance = startBalance;
        }

        public void Add(float amount)
        {
            Balance += amount;
        }

        public bool CanAfford(float amount)
        {
            if ((Balance - amount) >= 0) return true;
            return false;
        }

        public void Spend(float amount)
        {
            Debug.Log($"[NPCWallet]: spended {amount}$");
            Balance -= amount;
            Balance = Mathf.Max(0, Balance);
        }

        public void SetMoney(float amount)
        {
            Balance = amount;
        }
}
