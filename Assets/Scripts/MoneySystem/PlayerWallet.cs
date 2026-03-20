using UnityEngine;

namespace StoreSimulator.MoneySystem
{
    public class PlayerWallet : MonoBehaviour, IWallet
    {
        [SerializeField] private BalanceUI balanceUI;
        [SerializeField] private float startBalance = 500f;
        public float Balance { get; private set; }

        private void Awake()
        {
            Balance = startBalance;
        }

        public void Add(float amount)
        {
            Balance += amount;
            balanceUI.UpdateBalanceUI();
        }

        public bool CanAfford(float amount)
        {
            if ((Balance - amount) >= 0) return true;
            return false;
        }

        public void Spend(float amount)
        {
            Debug.Log($"[PlayerWallet]: spended {amount}$");
            Balance -= amount;
            Balance = Mathf.Max(0, Balance);
            balanceUI.UpdateBalanceUI();
        }

        public void SetMoney(float amount)
        {
            Balance = amount;
            balanceUI.UpdateBalanceUI();
        }
    }
}

