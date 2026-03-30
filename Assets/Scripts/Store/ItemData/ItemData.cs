using UnityEngine;

namespace StoreSimulator.StoreableItems
{
    [CreateAssetMenu(fileName = "Item Data", menuName = "Store Items/StorableItems")]
    public class ItemData : ScriptableObject
    {
        [SerializeField] private string itemName;
        [SerializeField] private float basePrice;
        [SerializeField] private float foodRestore;
        [SerializeField] private float thirstRestore;
        [SerializeField] private ItemCategory category;
        [SerializeField] private ItemSubCategory subCategory;
        [SerializeField] private GameObject prefab;
        [SerializeField] private Sprite icon;

        public string ItemName => itemName;
        public float BasePrice => basePrice;
        public float FoodRestore => foodRestore;
        public float ThirstRestore => thirstRestore;
        public ItemCategory Category => category;
        public ItemSubCategory SubCategory => subCategory;
        public GameObject Prefab => prefab;
        public Sprite Icon => icon;
    }
}
