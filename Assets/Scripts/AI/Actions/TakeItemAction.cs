using StoreSimulator.InteractableObjects;
using StoreSimulator.StoreManager;
using Unity.VisualScripting.Dependencies.NCalc;
using UnityEngine;

public class TakeItemAction : INPCAction
{
    private readonly IStorage _storage;
    private IStoreable _storeable;
    private bool _success;

    public string Name => "TakeItem";

    public TakeItemAction(IStorage storage)
    {
        _storage = storage;
    }

    public bool CanExecute(MonoBehaviour npc)
    {
        return npc.TryGetComponent(out IMovable _)
            && npc.TryGetComponent(out IPickupPointOwner _)
            && npc.TryGetComponent(out IStoreVisitor _)
            && npc.TryGetComponent(out ITransformProvider _)
            && _storage != null
            && _storage.CanTakeItem();
    }

    public void OnStart(MonoBehaviour npc)
    {
        var movable = npc.GetComponent<IMovable>();
        movable.Movement.SetDestination(_storage.InteractionPoint);
        _success = false;
    }

    public bool OnUpdate(MonoBehaviour npc)
    {
        var movable = npc.GetComponent<IMovable>();
        if (!movable.Movement.HasReached) return false;

        var npcTransform = npc.GetComponent<ITransformProvider>();
        npcTransform.Transform.LookAt(_storage.InteractionPoint);

        if (!_storage.CanTakeItem()) return true;

        GameObject go = _storage.PeekItem();

        if (!go.TryGetComponent<IStoreable>(out var storeable)) return true;

        GameObject itemGO = storeable.OnPickedFromStore();
        _storeable = storeable;

        var pickUp = npc.GetComponent<IPickupPointOwner>();
        itemGO.transform.position = pickUp.PickUpPoint.position;
        itemGO.transform.SetParent(pickUp.PickUpPoint);

        var visitor = npc.GetComponent<IStoreVisitor>();
        float playerPrice = storeable.LockedPrice;
        float marketPrice = PricesManager.Instance.GetMarketPriceForItem(storeable.Data);
        
        var psychology = npc.GetComponent<NPCPsycho>();
        bool wantBuy = psychology.BuyItemOrNot(playerPrice, marketPrice);

        if (wantBuy && npc.TryGetComponent(out IWalletOwner walletOwner) && walletOwner.Wallet.CanAfford(playerPrice))
        {
            visitor.Inventory.Add(storeable);
            _success = true;
        }
        else
        {
            if (_storage.CanPlaceItem(storeable)) _storage.PlaceItem(itemGO);
            else HandleDropItem(npc, storeable);
        }

        return true;
    }

    public void OnCancel(MonoBehaviour npc)
    {
        if (_storeable != null && !_success && npc.TryGetComponent(out CustomerController _))
        {
            HandleDropItem(npc, _storeable);
        }
    }

    public void OnComplete(MonoBehaviour npc) { }

    private void HandleDropItem(MonoBehaviour npc, IStoreable item)
    {
        if(npc.TryGetComponent(out IStoreVisitor visitor))
        {
            visitor.Inventory.Remove(item);
        }

        if(item is MonoBehaviour mb)
        {
            mb.transform.SetParent(null);
            mb.gameObject.SetActive(false);
        }
    }
}
