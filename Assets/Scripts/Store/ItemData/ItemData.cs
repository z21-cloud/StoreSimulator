using UnityEngine;

namespace StoreSimulator.StoreableItems
{
    [CreateAssetMenu(fileName = "Item Data", menuName = "Store Items/StorableItems")]
    public class ItemData : ScriptableObject
    {
        [SerializeField] private string itemName;
        [SerializeField] private float basePrice;
        [SerializeField] private ItemCategory category;
        [SerializeField] private ItemSubCategory subCategory;

        public string ItemName => itemName;
        public float BasePrice => basePrice;
        public ItemCategory Category => category;
        public ItemSubCategory SubCategory => subCategory;
    }
}
