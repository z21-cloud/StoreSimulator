using StoreSimulator.MoneySystem;
using UnityEngine;

public interface IWalletOwner
{
    public IWallet Wallet { get; }
}
