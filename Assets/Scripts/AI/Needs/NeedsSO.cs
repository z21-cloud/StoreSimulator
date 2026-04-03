using UnityEngine;

[CreateAssetMenu(fileName = "NeedsConfig", menuName = "Configs/NeedsConfig")]
public class NeedsSO : ScriptableObject
{
    [Header("Needs")]
    public float startHunger = 100f;
    public float startThirst = 100f;
    public float thirstDecreaseRate = 1f;
    public float hungerDecreaseRate = 1f;
    public float decreaseTickRate = 1f;
}
