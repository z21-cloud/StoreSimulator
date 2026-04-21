using StoreSimulator.Boxes;
using UnityEngine;

public interface IDeliverable
{
    public void Initialize(DeliveryOrder order);
    public void SetOwner(IBoxOwner onwer);

    public Transform transform {get;}
    public GameObject gameObject {get;}
}
