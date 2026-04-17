using StoreSimulator.MoneySystem;
using StoreSimulator.PlayerInput;
using UnityEngine;

public class TryFindCashier : MonoBehaviour
{
    public bool FindCashier { get; private set; }
    public IWallet CashierWallet { get; private set; }

    void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent<ICanSell>(out _))
        {
            FindCashier = true;
        }
    }

    void OnTriggerStay(Collider other)
    {
        if (other.TryGetComponent<IWallet>(out var wallet))
        {
            Debug.Log($"TryFindCashier: CashierWallet is {wallet}");
            CashierWallet = wallet;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.TryGetComponent<ICanSell>(out _))
        {
            FindCashier = false;
            CashierWallet = null;
        }
    }
}
