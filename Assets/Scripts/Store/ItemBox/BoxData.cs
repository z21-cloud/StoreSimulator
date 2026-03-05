using UnityEngine;

namespace StoreSimulator.StoreableItems
{
    [CreateAssetMenu(fileName = "BoxData", menuName = "Store Items/BoxData")]
    public class BoxData : ScriptableObject
    {
        [SerializeField] private ItemData itemData;
        [SerializeField] private int maxCapacity;
        [SerializeField] private GameObject itemPrefab;
        [SerializeField] private ItemCategory allowedCategory;

        public ItemData BoxItemData => itemData;
        public GameObject ItemPrefab => itemPrefab;
        public int BoxMaxCapacity => maxCapacity;
        public ItemCategory AllowedCategory => allowedCategory;
    }
}
