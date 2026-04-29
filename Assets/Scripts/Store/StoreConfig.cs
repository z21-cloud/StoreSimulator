using UnityEngine;

[CreateAssetMenu(fileName = "StoreConfig", menuName = "StoreSimulator/Store Config")]
public class StoreConfig : ScriptableObject
{
    [SerializeField] private string storeID;
    [SerializeField] private bool isAuto;
    // [SerializeField] private float priceMultiplyer = 1f;

    public string StoreID => storeID;
    public bool IsAuto => isAuto;
}
