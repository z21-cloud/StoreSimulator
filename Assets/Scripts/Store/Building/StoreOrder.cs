using UnityEngine;

namespace StoreSimulator.BuildingSystem
{
    [CreateAssetMenu(fileName = "StoreOrder", menuName = "Store/StoreOrder")]
    public class StoreOrder : ScriptableObject
    {
        // shelf SO
        [SerializeField] private float cost;
        [SerializeField] private GameObject prefab;
        [SerializeField] private Sprite icon;
        [SerializeField] private string itemName;

        public float Cost => cost;
        public GameObject Prefab => prefab;
        public Sprite Icon => icon;
        public string ItemName => itemName;
    }
}
