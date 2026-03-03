using UnityEngine;

namespace StoreSimulator.StoreableItems
{
    [CreateAssetMenu(fileName = "Item Data", menuName = "Store Items/StorableItems")]
    public class ItemData : ScriptableObject
    {
        [SerializeField] private string itemName;
        [SerializeField] private ItemCategory category;
        //[SerializeField] private GameObject prefab;

        public string ItemName => itemName;
        public ItemCategory Category => category;
    }
}
