using UnityEngine;

public class BuyAtCashierAction : INPCAction
{
    private readonly ICashStorage _cashStorage;
    public string Name => "BuyAtCashier";

    public BuyAtCashierAction(ICashStorage cashier)
    {
        _cashStorage = cashier;
    }

    public bool CanExecute(MonoBehaviour npc)
    {
        return _cashStorage != null &&
                _cashStorage.IsAvailable &&
                npc.TryGetComponent(out IWalletOwner _) &&
                npc.TryGetComponent(out IMovable _);
    }

    public void OnStart(MonoBehaviour npc)
    {
        var movable = npc.GetComponent<IMovable>();
        movable.Movement.SetDestination(_cashStorage.InteractionPoint);
    }

    public bool OnUpdate(MonoBehaviour npc)
    {
        var movable = npc.GetComponent<IMovable>();
        if (!movable.Movement.HasReached) return false;
        if (!_cashStorage.IsAvailable) return false;

        if (!npc.TryGetComponent(out CustomerController customerController)) return true;
        var walletOwner = npc.GetComponent<IWalletOwner>();

        for (int i = customerController.Inventory.Count - 1; i >= 0; i--)
        {
            var item = customerController.Inventory[i];
            if (walletOwner.Wallet.CanAfford(item.LockedPrice))
            {
                _cashStorage.BuyItem(item, walletOwner.Wallet);
                
                var psychology = npc.GetComponent<NPCPsycho>();
                psychology.IncreaseParameters(item.Data.FoodRestore, item.Data.ThirstRestore);
            }
            else
            {
                customerController.HandleDropItem(item);

            }

            customerController.Inventory.RemoveAt(i);
        }

        return true;
    }

    public void OnCancel(MonoBehaviour npc) { }

    public void OnComplete(MonoBehaviour npc) { }
}
