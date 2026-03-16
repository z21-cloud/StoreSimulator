using StoreSimulator.MoneySystem;
using TMPro;
using UnityEngine;

public class BalanceUI : MonoBehaviour
{
    [SerializeField] private TMP_Text balanceText;
    [SerializeField] private GameObject balanceUI;
    [SerializeField] private PlayerWallet playerWallet;

    // use start method because of player wallet initialization priority
    private void Start()
    {
        balanceText.text = $"Current Balance: \n {playerWallet.Balance}$";
    }

    public void BalanceUISetActive(bool value)
    {
        balanceUI.SetActive(value);
    }

    public void UpdateBalanceUI()
    {
        balanceText.text = $"Current Balance: \n {playerWallet.Balance}$";
    }
}
