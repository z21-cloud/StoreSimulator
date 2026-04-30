using StoreSimulator.StoreableItems;
using UnityEngine;

[CreateAssetMenu(fileName = "VendingConfig", menuName = "StoreSimulator/Vending Config")]
public class VendingMachineConfig : ScriptableObject
{
    [Header("Vending machine capacity")]
    [Tooltip("Максимум единиц товара до следующей доставки")]
    [SerializeField] private int capacity = 20;
    [Header("Category")]
    [Tooltip("Что продает автомат, разрешенные категории")]
    [SerializeField] private ItemCategory allowedCategory;
    [Header("IStore settings")]
    [SerializeField] private string storeID;
    [SerializeField] private bool isAuto;

    public int Capacity => capacity;
    public string StoreID => storeID;
    public bool IsAuto => isAuto;
    public ItemCategory AllowedCategory => allowedCategory;
}
