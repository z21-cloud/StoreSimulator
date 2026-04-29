using UnityEngine;

public interface IStore
{
    public IPriceProvider PriceProvider { get; }
    public CashStorageRegistry CashStorageRegistry { get; }
    public StorageRegistry StorageRegistry { get; }

    public Transform StoreEnterPoint { get; }
    public Transform StoreLeavePoint { get; }
    public Transform DeliveryPoint { get; }

    public string StoreID { get; }
    public bool IsOpen { get; }
    public bool IsAuto { get; }
}
